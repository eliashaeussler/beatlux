using UnityEngine;
using System.Collections;

public class Spectrum1 : MonoBehaviour
{

    


    public GameObject prefab;
    public int numberOfObjects = 5; //number of cubes that are generated when scene is started
    public int numberOfChains = 4;
    public int numberOfCubes;
    public GameObject[] cubes;
    public int[] randSpec;
    public Material mat;
    Color color = new Color(0,0,1,1);
    int randomSpec;




    void Start()
    {

        numberOfCubes = numberOfChains * numberOfObjects;
        mat = Resources.Load("CubeMaterial.mat", typeof(Material)) as Material;
        /**
         * Initializing the scene, creating a circle of cubes in the radius set above.
         **/
        for (int i = 0; i < numberOfChains; i++)
        {
            for (int j = 0; j < numberOfObjects; j++)
            {
                Vector3 pos = new Vector3(i*2, -10 + j * 3, 0);
                Instantiate(prefab, pos, Quaternion.identity);

            }
           
        }
        cubes = GameObject.FindGameObjectsWithTag("Cubes");
        randSpec = new int[cubes.Length];
        for (int i = 0; i < cubes.Length; i++)
        {
            if(i % 5 == 0||i==0)
            {
                randSpec[i] = Random.Range(0, 200);
            }
            else
            {
                randSpec[i] = randSpec[i - 1] + 1;
            }
        }
    }




    
    void Update()
    {
        float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming); //Reading the spectrum from the song put into the AudioListener 

        /**
         * Scaling the height of each cube, based on the value in the spectrum-array
         **/ 
        for (int i = 0; i < numberOfCubes; i++)
        {

 
            if( i % 3 == 0)
            {
                Color color =new Color (0F, 0F,0.5F,0F);
                cubes[i].GetComponent<Renderer>().material.SetColor("_ColorTint", color);
            }
            if(i % 2 == 1)
            {
                Color color2 = new Color(0F, 0.8F, 0.5F, 0F);
                cubes[i].GetComponent<Renderer>().material.SetColor("_RimColor", color2);
            }
            cubes[i].GetComponent<Renderer>().material.SetFloat("_RimPower", 1 / (spectrum[randSpec[i]] * 10));

        }
    }


   

}
