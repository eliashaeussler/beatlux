using UnityEngine;
using System.Collections;

public class Spectrum1 : MonoBehaviour
{

    


    public GameObject prefab;
    public int numberOfObjects = 20; //number of cubes that are generated when scene is started
    public float radius = 5f;
    public GameObject[] cubes;
    public float newScale = 20; //scales the height of the cubes while changing
    public Material mat;




    void Start()
    {
        mat = Resources.Load("CubeMaterial.mat", typeof(Material)) as Material;
        /**
         * Initializing the scene, creating a circle of cubes in the radius set above.
         **/
        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 pos = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
            Instantiate(prefab, pos, Quaternion.identity);
        }
        cubes = GameObject.FindGameObjectsWithTag("Cubes");

    }


    
    void Update()
    {
        float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming); //Reading the spectrum from the song put into the AudioListener 

        /**
         * Scaling the height of each cube, based on the value in the spectrum-array
         **/ 
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 previousScale = cubes[i].transform.localScale;
            cubes[i].GetComponent<Renderer>().material.SetFloat("_RimPower", 1 / (spectrum[i] * 10));

            if (spectrum[i] * newScale > 1.3)   //Only update the cube if the height is above set value (1.3)
            {
                previousScale.y = Mathf.Lerp(previousScale.y, spectrum[i] * newScale, Time.deltaTime * 30);

            }
            else
            {
                previousScale.y = 1;

            }
            cubes[i].transform.localScale = previousScale;
            
        }
    }


   

}
