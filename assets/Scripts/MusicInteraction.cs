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

        /**
         * Scaling the height of each cube, based on the value in the spectrum-array
         */
      
        
            Vector3 previousScale = transform.localScale;
            int anzahl = 10;
            float average = 0;
            for(int i = 0; i< anzahl; i++)
            {
                average += spectrum[i];
            }
            average = average / anzahl;

            if (average * scale > 1)   //Only update the cube if the height is above set value (1.3)
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

            Debug.Log(previousScale);

    }
}
