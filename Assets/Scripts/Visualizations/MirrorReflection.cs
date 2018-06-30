/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.Collections;
using UnityEngine;

/// <summary>
///     This is in fact just the Water script from Pro Standard Assets, just with refraction stuff removed.
/// </summary>
[ExecuteInEditMode] // Make mirror live-update even when not in play mode
public class MirrorReflection : MonoBehaviour
{
    private static bool _insideRendering;

    private readonly Hashtable _reflectionCameras = new Hashtable(); // Camera -> Camera table
    private int _oldReflectionTextureSize;

    private RenderTexture _reflectionTexture;
    public float ClipPlaneOffset = 0.07f;
    public bool DisablePixelLights = true;

    public LayerMask ReflectLayers = -1;
    public int TextureSize = 256;

    // This is called when it's known that the object will be rendered by some
    // camera. We render reflections and do other updates here.
    // Because the script executes in edit mode, reflections for the scene view
    // camera will just work!
    public void OnWillRenderObject()
    {
        var rend = GetComponent<Renderer>();
        if (!enabled || !rend || !rend.sharedMaterial || !rend.enabled)
            return;

        var cam = Camera.current;
        if (!cam)
            return;

        // Safeguard from recursive reflections.        
        if (_insideRendering)
            return;
        _insideRendering = true;

        Camera reflectionCamera;
        CreateMirrorObjects(cam, out reflectionCamera);

        // find out the reflection plane: position and normal in world space
        var pos = transform.position;
        var normal = transform.up;

        // Optionally disable pixel lights for reflection
        var oldPixelLightCount = QualitySettings.pixelLightCount;
        if (DisablePixelLights)
            QualitySettings.pixelLightCount = 0;

        UpdateCameraModes(cam, reflectionCamera);

        // Render reflection
        // Reflect camera around reflection plane
        var d = -Vector3.Dot(normal, pos) - ClipPlaneOffset;
        var reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

        var reflection = Matrix4x4.zero;
        CalculateReflectionMatrix(ref reflection, reflectionPlane);
        var oldpos = cam.transform.position;
        var newpos = reflection.MultiplyPoint(oldpos);
        reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

        // Setup oblique projection matrix so that near plane is our reflection
        // plane. This way we clip everything below/above it for free.
        var clipPlane = CameraSpacePlane(reflectionCamera, pos, normal, 1.0f);
        //Matrix4x4 projection = cam.projectionMatrix;
        var projection = cam.CalculateObliqueMatrix(clipPlane);
        reflectionCamera.projectionMatrix = projection;

        reflectionCamera.cullingMask = ~(1 << 4) & ReflectLayers.value; // never render water layer
        reflectionCamera.targetTexture = _reflectionTexture;
        GL.invertCulling = true;
        reflectionCamera.transform.position = newpos;
        var euler = cam.transform.eulerAngles;
        reflectionCamera.transform.eulerAngles = new Vector3(0, euler.y, euler.z);
        reflectionCamera.Render();
        reflectionCamera.transform.position = oldpos;
        GL.invertCulling = false;
        var materials = rend.sharedMaterials;
        foreach (var mat in materials)
            if (mat.HasProperty("_ReflectionTex"))
                mat.SetTexture("_ReflectionTex", _reflectionTexture);

        // Restore pixel light count
        if (DisablePixelLights)
            QualitySettings.pixelLightCount = oldPixelLightCount;

        _insideRendering = false;
    }


    // Cleanup all the objects we possibly have created
    private void OnDisable()
    {
        if (_reflectionTexture)
        {
            DestroyImmediate(_reflectionTexture);
            _reflectionTexture = null;
        }

        foreach (DictionaryEntry kvp in _reflectionCameras)
            DestroyImmediate(((Camera) kvp.Value).gameObject);
        _reflectionCameras.Clear();
    }


    private static void UpdateCameraModes(Camera src, Camera dest)
    {
        if (dest == null)
            return;
        // set camera to clear the same way as current camera
        dest.clearFlags = src.clearFlags;
        dest.backgroundColor = src.backgroundColor;
        if (src.clearFlags == CameraClearFlags.Skybox)
        {
            var sky = src.GetComponent<Skybox>();
            var mysky = dest.GetComponent<Skybox>();
            if (!sky || !sky.material)
            {
                mysky.enabled = false;
            }
            else
            {
                mysky.enabled = true;
                mysky.material = sky.material;
            }
        }

        // update other values to match current camera.
        // even if we are supplying custom camera&projection matrices,
        // some of values are used elsewhere (e.g. skybox uses far plane)
        dest.farClipPlane = src.farClipPlane;
        dest.nearClipPlane = src.nearClipPlane;
        dest.orthographic = src.orthographic;
        dest.fieldOfView = src.fieldOfView;
        dest.aspect = src.aspect;
        dest.orthographicSize = src.orthographicSize;
    }

    // On-demand create any objects we need
    private void CreateMirrorObjects(Object currentCamera, out Camera reflectionCamera)
    {
        reflectionCamera = null;

        // Reflection render texture
        if (!_reflectionTexture || _oldReflectionTextureSize != TextureSize)
        {
            if (_reflectionTexture)
                DestroyImmediate(_reflectionTexture);
            _reflectionTexture =
                new RenderTexture(TextureSize, TextureSize, 16)
                {
                    name = "__MirrorReflection" + GetInstanceID(),
                    isPowerOfTwo = true,
                    hideFlags = HideFlags.DontSave
                };
            _oldReflectionTextureSize = TextureSize;
        }

        // Camera for reflection
        reflectionCamera = _reflectionCameras[currentCamera] as Camera;

        if (reflectionCamera) return;

        // catch both not-in-dictionary and in-dictionary-but-deleted-GO
        var go = new GameObject("Mirror Refl Camera id" + GetInstanceID() + " for " + currentCamera.GetInstanceID(),
            typeof(Camera), typeof(Skybox));
        reflectionCamera = go.GetComponent<Camera>();
        reflectionCamera.enabled = false;
        reflectionCamera.transform.position = transform.position;
        reflectionCamera.transform.rotation = transform.rotation;
        reflectionCamera.gameObject.AddComponent<FlareLayer>();
        go.hideFlags = HideFlags.HideAndDontSave;
        _reflectionCameras[currentCamera] = reflectionCamera;
    }

    // Extended sign: returns -1, 0 or 1 based on sign of a
    private static float sgn(float a)
    {
        if (a > 0.0f) return 1.0f;
        if (a < 0.0f) return -1.0f;
        return 0.0f;
    }

    // Given position/normal of the plane, calculates plane in camera space.
    private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
    {
        var offsetPos = pos + normal * ClipPlaneOffset;
        var m = cam.worldToCameraMatrix;
        var cpos = m.MultiplyPoint(offsetPos);
        var cnormal = m.MultiplyVector(normal).normalized * sideSign;
        return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
    }

    // Calculates reflection matrix around the given plane
    private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
    {
        reflectionMat.m00 = 1F - 2F * plane[0] * plane[0];
        reflectionMat.m01 = -2F * plane[0] * plane[1];
        reflectionMat.m02 = -2F * plane[0] * plane[2];
        reflectionMat.m03 = -2F * plane[3] * plane[0];

        reflectionMat.m10 = -2F * plane[1] * plane[0];
        reflectionMat.m11 = 1F - 2F * plane[1] * plane[1];
        reflectionMat.m12 = -2F * plane[1] * plane[2];
        reflectionMat.m13 = -2F * plane[3] * plane[1];

        reflectionMat.m20 = -2F * plane[2] * plane[0];
        reflectionMat.m21 = -2F * plane[2] * plane[1];
        reflectionMat.m22 = 1F - 2F * plane[2] * plane[2];
        reflectionMat.m23 = -2F * plane[3] * plane[2];

        reflectionMat.m30 = 0F;
        reflectionMat.m31 = 0F;
        reflectionMat.m32 = 0F;
        reflectionMat.m33 = 1F;
    }
}