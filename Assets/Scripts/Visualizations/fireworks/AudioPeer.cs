/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;


//[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour
{
    private static readonly float[] Samples = new float[512];
    public static readonly float[] FreqBands = new float[8];


    // Use this for initialization

    // Update is called once per frame
    private void Update()
    {
        GetSpectrumAudioSource();
        MakeFreqBands();
    }

    private static void GetSpectrumAudioSource()
    {
        AudioListener.GetSpectrumData(Samples, 0, FFTWindow.Blackman);
    }

    //creates frequences suitable for the human perception 
    private static void MakeFreqBands()
    {
        var count = 0;
        for (var i = 0; i < 8; i++)
        {
            float average = 0;
            var sCount = (int) Mathf.Pow(2, i) * 2;
            if (i == 7) sCount += 2;
            for (var j = 0; j < sCount; j++)
            {
                average += Samples[count] * (count + 1);
                count++;
            }

            average /= count;
            FreqBands[i] = average * 10;
        }
    }
}