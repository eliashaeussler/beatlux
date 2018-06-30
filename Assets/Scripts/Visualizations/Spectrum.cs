/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

public class Spectrum : MonoBehaviour
{
    private readonly ColorSchemeObj _colorScheme = Settings.Active.ColorScheme;
    public GameObject[] Cubes;
    public Material Mat;
    public float NewScale = 20; //scales the height of the cubes while changing
    public int NumberOfObjects = 100; //number of cubes that are generated when scene is started
    public GameObject Prefab;
    public float Radius = 5f;


    private void Start()
    {
        /**
         * Initializing the scene, creating a circle of cubes in the radius set above.
         **/
        for (var i = 0; i < NumberOfObjects; i++)
        {
            var angle = i * Mathf.PI * 2 / NumberOfObjects;
            var pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * Radius;
            Instantiate(Prefab, pos, Quaternion.identity, Settings.MenuManager.VizContents);
        }

        Cubes = GameObject.FindGameObjectsWithTag("Cubes");
    }


    private void Update()
    {
        // Reading the spectrum from the song put into the AudioListener
        var spectrum = new float[1024];
        Settings.MenuManager.Audio.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);

        /**
         * Scaling the height of each cube, based on the value in the spectrum-array
         **/
        var t = 0f;
        for (var i = 0; i < NumberOfObjects; i++)
        {
            var previousScale = Cubes[i].transform.localScale;


            if (i < NumberOfObjects / 3)
            {
                Cubes[i].GetComponent<Renderer>().material.color =
                    Color.Lerp(_colorScheme.Colors[0], _colorScheme.Colors[1], t);
                t += 1f / (NumberOfObjects / 3f);
            }
            else if (i == NumberOfObjects / 3)
            {
                t = 0;
            }

            if (i >= NumberOfObjects / 3 && i < NumberOfObjects / 3 * 2)
            {
                Cubes[i].GetComponent<Renderer>().material.color =
                    Color.Lerp(_colorScheme.Colors[1], _colorScheme.Colors[2], t);
                t += 1f / (NumberOfObjects / 3f);
            }
            else if (i == NumberOfObjects / 3 * 2)
            {
                t = 0;
            }

            if (i >= NumberOfObjects / 3 * 2)
            {
                Cubes[i].GetComponent<Renderer>().material.color =
                    Color.Lerp(_colorScheme.Colors[2], _colorScheme.Colors[0], t);
                t += 1f / (NumberOfObjects / 3f);
            }


            if (spectrum[i] * NewScale > 2.3) //Only update the cube if the height is above set value (1.3)
                previousScale.y = Mathf.Lerp(previousScale.y, spectrum[i] * NewScale, Time.deltaTime * 30);
            else
                previousScale.y = 2;
            Cubes[i].transform.localScale = previousScale;
        }
    }
}