using UnityEngine;
using System.Collections;

public class KetteVersuch : MonoBehaviour
{

    


    public GameObject ketten;
    public GameObject haenger;

    public int numKetten;
    public int numHaenger;

    //public int numberOfObjects = 5; //number of cubes that are generated for one chain
    //public int numberOfChains = 4; // number of chains that are generated
    //public int numberOfCubes; //int to display the total number of cubes

    public GameObject[] ketAr; // array to save the cubes
    public GameObject[] cubes;
    public GameObject[] haengAr;
    public int[] randSpec;  //array to save the random Range of Spectrum

    //Color color = new Color(0,0,1,1); // a color(test)

    int randomSpec; //int for Spectrum




    void Start()
    {
        //numberOfCubes = numberOfChains * numberOfObjects;

        /**
         * creating chains of cubes
         *  for every chain create numberOfObjects cubes
         *  x Position of cube = chain number *2
         *  y Position of cube = Cube number *3 -10
         *  z position of cube = 0
         **/

        for (int i = 0; i < numKetten; i++)
        {
                int posX = Random.Range(0, 200);
                int posZ = Random.Range(0, 200);
                Vector3 pos = new Vector3(posX, -10, posZ);
                Instantiate(ketten, pos, Quaternion.identity);    
        }


        for (int i = 0; i < numHaenger; i++)
        {
            int posX = Random.Range(0, 200);
            int posZ = Random.Range(0, 200);
            Vector3 pos = new Vector3(posX, 20, posZ);
            Instantiate(haenger, pos, Quaternion.identity);
        }

        //save created cubes in array "cubes"
        cubes = GameObject.FindGameObjectsWithTag("Light");
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
        for (int i = 0; i < cubes.Length; i++)
        {

            cubes[i].GetComponent<Renderer>().material.SetFloat("_RimPower", 1 / (spectrum[randSpec[i]] * 10));
            cubes[i].GetComponent<Light>().intensity = spectrum[randSpec[i]]*10;
            cubes[i].GetComponent<Light>().bounceIntensity = spectrum[randSpec[i]]*10;

        }

    }


   

}
