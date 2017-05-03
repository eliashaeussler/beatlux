using UnityEngine;
using System.Collections;

//[RequireComponent (typeof (AudioSource))]
public class AudioPeer : MonoBehaviour {

//    AudioSource aSource;
    public static float[] samples = new float[512];
    public static float[] freqBands = new float[8];


	// Use this for initialization
	void Start () {
//        aSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        GetSpectrumAudioSource();
        MakeFreqBands();
	}

    void GetSpectrumAudioSource()
    {
		AudioListener.GetSpectrumData(samples, 0, FFTWindow.Blackman);
    }

    //creates frequences suitable for the human perception 
    void MakeFreqBands()
    {
        int count = 0;
        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int sCount = (int)Mathf.Pow(2, i) * 2;
            if (i == 7)
            {
                sCount += 2;
            }
            for (int j = 0; j < sCount; j++)
            {
                average += samples[count] * (count + 1);
                count++;
            }

            average /= count;
            freqBands[i] = average*10;
        }
    }

    /*
     * 20-60
     * 60-250
     * 250-500
     * 500-2000
     * 2000-4000
     * 4000-6000
     * 6000-20000
     * 
     * */
}
