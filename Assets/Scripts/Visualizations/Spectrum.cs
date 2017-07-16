using UnityEngine;
using System.Collections;

public class Spectrum : MonoBehaviour
{
    public GameObject prefab;
    public int numberOfObjects = 100; //number of cubes that are generated when scene is started
    public float radius = 5f;
    public GameObject[] cubes;
    public float newScale = 20; //scales the height of the cubes while changing
    public Material mat;
    private ColorSchemeObj colorScheme = Settings.Active.ColorScheme;


    void Start()
    {
            
        /**
         * Initializing the scene, creating a circle of cubes in the radius set above.
         **/
        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
			Instantiate(prefab, pos, Quaternion.identity, Settings.MenuManager.vizContents);
        }
        cubes = GameObject.FindGameObjectsWithTag("Cubes");
        
    }



    void Update()
    {
		float[] spectrum = Settings.MenuManager.audio.GetSpectrumData(1024, 0, FFTWindow.Hamming); //Reading the spectrum from the song put into the AudioListener 

        /**
         * Scaling the height of each cube, based on the value in the spectrum-array
         **/
        float t = 0f;
        for (int i = 0; i < numberOfObjects; i++)
        {
            
            Vector3 previousScale = cubes[i].transform.localScale;
            

            if (i < numberOfObjects / 3)
            {
                cubes[i].GetComponent<Renderer>().material.color = Color.Lerp(colorScheme.Colors[0], colorScheme.Colors[1], t);
                t += 1f / (numberOfObjects / 3f);
            }
            else if (i == numberOfObjects / 3)
            {
                t = 0;
            }

            if (i >= numberOfObjects / 3 && i< (numberOfObjects/3)*2)
            {
                cubes[i].GetComponent<Renderer>().material.color = Color.Lerp(colorScheme.Colors[1], colorScheme.Colors[2], t);
                t += 1f / (numberOfObjects / 3f);
            }
            else if (i == (numberOfObjects / 3) *2)
            {
                t = 0;
            }

            if (i >= (numberOfObjects / 3)*2)
            {
                cubes[i].GetComponent<Renderer>().material.color = Color.Lerp(colorScheme.Colors[2], colorScheme.Colors[0], t);
                t += 1f / (numberOfObjects / 3f);
            }


            if (spectrum[i] * newScale > 2.3)   //Only update the cube if the height is above set value (1.3)
            {
                previousScale.y = Mathf.Lerp(previousScale.y, spectrum[i] * newScale, Time.deltaTime * 30);

            }
            else
            {
                previousScale.y = 2;

            }
            cubes[i].transform.localScale = previousScale;
            
        }
    }

   
   
}
