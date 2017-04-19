using UnityEngine;
using System.Collections;

public class Kette : MonoBehaviour
{

    


    public GameObject prefab; // used for loading an Object to clone
    public int numberOfObjects = 5; //number of cubes that are generated for one chain
    public int numberOfChains = 4; // number of chains that are generated
    public int numberOfCubes; //int to display the total number of cubes
    public GameObject[] cubes; // array to save the cubes
    public int[] randSpec;  //array to save the random Range of Spectrum
    Color color = new Color(0,0,1,1); // a color(test)
    int randomSpec; //int for Spectrum




    void Start()
    {

        numberOfCubes = numberOfChains * numberOfObjects;

        /**
         * creating chains of cubes
         *  for every chain create numberOfObjects cubes
         *  x Position of cube = chain number *2
         *  y Position of cube = Cube number *3 -10
         *  z position of cube = 0
         **/

        for (int i = 0; i < numberOfChains; i++)
        {
            for (int j = 0; j < numberOfObjects; j++)
            {
                Vector3 pos = new Vector3(i*2, -10 + j * 3, 0);
                Instantiate(prefab, pos, Quaternion.identity);

            }
           
        }

        //save created cubes in array "cubes"
        cubes = GameObject.FindGameObjectsWithTag("Cubes");
        //instantiate rndSpec with the length of "cubes"
        randSpec = new int[cubes.Length];
       
        
        /*
         * get random numbers to determine the displayed spectrum of a chain
         * for every element in randSpec:
         * if i is dividable by 5 (Because of 5 Cubes per chain) or 0:
         * save a random number between 0 and 200 at this place
         * else the number of the previous element +1 
         **/
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
         * change the color and rim color of certain cubes(test)
         * change the rim Power(Glow) in reaction to the Spectrum
         **/ 
        for (int i = 0; i < numberOfCubes; i++)
        {

 
            if( i % 3 == 0)
            {
                Color color =new Color (5F, 5F,0.5F,0F);
                cubes[i].GetComponent<Renderer>().material.SetColor("_ColorTint", color);
            }
            if(i % 2 == 1)
            {
                Color color2 = new Color(0F, 0.8F, 0.5F, 0F);
                cubes[i].GetComponent<Renderer>().material.SetColor("_RimColor", color2);
            }
            cubes[i].GetComponent<Renderer>().material.SetFloat("_RimPower", 1 / (spectrum[randSpec[i]] * 10));
            cubes[i].GetComponent<Light>().intensity = spectrum[randSpec[i]]*10;
            cubes[i].GetComponent<Light>().bounceIntensity = spectrum[randSpec[i]]*10;

        }
    }


   

}
