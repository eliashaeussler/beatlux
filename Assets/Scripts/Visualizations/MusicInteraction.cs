using UnityEngine;
using System.Collections;

public class MusicInteraction : MonoBehaviour
    {
    int s;
    public int scale = 30;
    // Use this for initialization
    void Start () {
         s = 1;
      
	}

    // Update is called once per frame
    void Update()
    {
        float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming); //Reading the spectrum from the song put into the AudioListener 

        float[] samples = AudioListener.GetOutputData(1024, 0);

        //Adding up all the samples, for approximate overall "volume"
        float vol = 0;
        foreach (float sample in samples)
        {
            if (sample >= 0)
            {
                vol += sample;
            }
            else vol -= sample;
           
            
        }
        
        
       
        //Rotate logo around the z-axis, based on time of the last frame and volume
        transform.Rotate(0, 0, -(Time.deltaTime*vol));

        /**
         * Scaling the logo, based on the value in the spectrum-array
         */

        Vector3 previousScale = transform.localScale;
            int anzahl = 10;
            float average = 0;
            for(int i = 0; i< anzahl; i++)
            {
                average += spectrum[i];
            }
            average = average / anzahl;

            if (average * scale > 1)   //Only update if the height is above set value (1)
            {
                previousScale.y = Mathf.Lerp(previousScale.y, average * scale, Time.deltaTime * 30);
                previousScale.x = Mathf.Lerp(previousScale.x, average * scale, Time.deltaTime * 30);
            
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
