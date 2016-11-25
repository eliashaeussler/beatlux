using UnityEngine;
using System.Collections;

public class Spectrum : MonoBehaviour
{

    // Use this for initialization


    public GameObject prefab;
    public int numberOfObjects = 20;
    public float radius = 5f;
    public GameObject[] cubes;
    public float newScale = 20;
    public Material mat;

    float r;
    float g;
    float b;

    void Start()
    {
        r = mat.color.r;
        g = mat.color.g;
        b = mat.color.b;

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Instantiate(prefab, pos, Quaternion.identity);
        }
        cubes = GameObject.FindGameObjectsWithTag("Cubes");

    }


    // Update is called once per frame
    void Update()
    {
        float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming);

        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 previousScale = cubes[i].transform.localScale;
            if (spectrum[i] * newScale > 1.3)
            {
                previousScale.y = Mathf.Lerp(previousScale.y, spectrum[i] * newScale, Time.deltaTime * 30);

            }
            else
            {
                previousScale.y = 1;

            }
            cubes[i].transform.localScale = previousScale;
            Debug.Log(cubes.Length);
        }
    }

    void OnGUI()
    {
        r = GUI.HorizontalSlider(new Rect(20, 10, Screen.width - 40, 20), r,0.0f, 1.0f);
        g = GUI.HorizontalSlider(new Rect(20, 30, Screen.width - 40, 20), g, 0.0f, 1.0f);
        b = GUI.HorizontalSlider(new Rect(20, 50, Screen.width - 40, 20), b, 0.0f, 1.0f);

        mat.color = new Color(r, g, b);
    }
}
