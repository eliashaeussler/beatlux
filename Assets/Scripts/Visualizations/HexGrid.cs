/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

public class HexGrid : MonoBehaviour
{
    private const float HexHeight = 30f;
    private const float HexWidth = 30f;
    private const float Gap = 10f;
    private readonly Vector3 _startPos = new Vector3(0, 0, 0);
    private ColorSchemeObj _colorScheme;
    private GameObject[] _hexes = new GameObject[100];

    public GameObject Hex;
    public int HexNumber = 100;
    public float NewScale = 500;
    public GameObject TempChild;
    public float Timing = 5f;

    /**
          * Initializing the scene, creating a field of hexagons shaped like a triangle.
          **/
    private void Start()
    {
        var pos = new Vector3(_startPos.x, _startPos.y, _startPos.z);
        for (var i = 1; i < HexNumber; i++)
        {
            if (i > 1) pos = pos + new Vector3(0, 0, -HexHeight - Gap);
            for (var j = 0; j < i; j++)
                if (j == 0)
                {
                    pos = new Vector3(_startPos.x, pos.y, pos.z);
                }
                else
                {
                    pos = pos + new Vector3(HexWidth + Gap, 0, 0);
                    Instantiate(Hex, pos, Quaternion.identity, Settings.MenuManager.VizContents);
                }
        }

        _hexes = GameObject.FindGameObjectsWithTag("Hexes");
        _colorScheme = Settings.Active.ColorScheme;
    }


    private void Update()
    {
        // Reading the spectrum from the song put into the AudioListener
        var spectrum = new float[1024];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);

        /**
         * Scaling the height of each cube, based on the value in the spectrum-array
         **/
        for (var i = 0; i < _hexes.Length; i++)
        {
            if (!_hexes[i]) continue;

            TempChild = _hexes[i].transform.Find("Cylinder").gameObject;
            TempChild.GetComponent<Renderer>().material.color = _colorScheme.Colors[0];
            TempChild = _hexes[i].transform.Find("CylinderTop").gameObject;
            TempChild.GetComponent<Renderer>().material.color = _colorScheme.Colors[1];

            var previousScale = _hexes[i].transform.localScale;

            if (spectrum[i] * NewScale > 1.2)
            {
                previousScale.y = Mathf.Lerp(previousScale.y, spectrum[i] * NewScale * ((i + 4) / 3),
                    Time.deltaTime * Timing);
            }
            else
            {
                if (previousScale.y > 1) previousScale.y -= Random.Range(0.01f, 0.2f);
            }

            _hexes[i].transform.localScale = previousScale;
        }
    }
}