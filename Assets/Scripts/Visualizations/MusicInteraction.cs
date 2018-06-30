/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

public class MusicInteraction : MonoBehaviour
{
    private int _s;
    public int Scale = 30;

    // Use this for initialization
    private void Start()
    {
        _s = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        // Reading the spectrum from the song put into the AudioListener
        var spectrum = new float[1024];
        AudioListener.GetOutputData(spectrum, 0);

        var samples = new float[1024];
        AudioListener.GetOutputData(samples, 0);

        //Adding up all the samples, for approximate overall "volume"
        float vol = 0;
        foreach (var sample in samples)
            if (sample >= 0)
                vol += sample;
            else
                vol -= sample;


        //Rotate logo around the z-axis, based on time of the last frame and volume
        transform.Rotate(0, 0, -(Time.deltaTime * vol));

        /**
         * Scaling the logo, based on the value in the spectrum-array
         */

        var previousScale = transform.localScale;
        const int anzahl = 10;
        float average = 0;
        for (var i = 0; i < anzahl; i++) average += spectrum[i];
        average = average / anzahl;

        if (average * Scale > 1) //Only update if the height is above set value (1)
        {
            previousScale.y = Mathf.Lerp(previousScale.y, average * Scale, Time.deltaTime * 30);
            previousScale.x = Mathf.Lerp(previousScale.x, average * Scale, Time.deltaTime * 30);
        }
        else
        {
            previousScale.y = Mathf.Lerp(previousScale.y, 1, Time.deltaTime * 30);
            previousScale.x = Mathf.Lerp(previousScale.x, 1, Time.deltaTime * 30);
        }

        transform.localScale = previousScale;

//            Debug.Log(previousScale);
    }
}