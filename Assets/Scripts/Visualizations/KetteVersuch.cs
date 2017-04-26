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

    public GameObject[][] ketAr; // array to save the cubes
    public GameObject[] cubes;
    public GameObject[][] haengAr;
    public int[] randSpec;  //array to save the random Range of Spectrum

    //Color color = new Color(0,0,1,1)

    Color color1 = new Color (1,0,0,1);
    Color color2 = new Color (0,1,0,1);
    Color color3 = new Color (0,0,1,1);
    Color color4 = new Color (1,1,0,1);
    Color color5 = new Color (0,1,1,1);

    int randomSpec; //int for Spectrum




    void Start()
    {
        
        ketAr = new GameObject[numKetten][];
        haengAr = new GameObject[numHaenger][];
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
            int random = Random.Range(1, 5);
            Vector3 pos = new Vector3(posX, -10, posZ);
            Instantiate(ketten, pos, Quaternion.identity);
            GameObject[] lichter = GameObject.FindGameObjectsWithTag("Light");


            //random Farbvergabe fuer Lichter
            if (random == 1)
            {
                for(int j = 0; j < lichter.Length; j++)
                {
                    if(j%2 == 0 || j == 0)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color1);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color1 * 2);
                        lichter[j].GetComponent<Light>().color = color1 * 3;
                    }
                    else
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color2);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color2 * 2);
                        lichter[j].GetComponent<Light>().color = color2 * 3;
                    }
                }
            }

            if (random == 2)
            {
                for(int j = 0; j <lichter.Length; j++)
                {
                    if(j%2 == 0 || j == 0)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color3);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color3 * 2);
                        lichter[j].GetComponent<Light>().color = color3 * 3;
                    }
                    else
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color4);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color4 * 2);
                        lichter[j].GetComponent<Light>().color = color4 * 3;
                    }
                }
            }

            if (random == 3)
            {
                for (int j = 0; j < lichter.Length; j++)
                {
                    lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color5);
                    lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color5 * 2);
                    lichter[j].GetComponent<Light>().color = color5 * 3;
                }
            }

            if (random == 4)
            {
                for (int j = 0; j < lichter.Length; j++)
                {
                    int rand = Random.Range(0, 5);

                    if(rand == 0)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color1);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color1 * 2);
                        lichter[j].GetComponent<Light>().color = color1 * 3;
                    }
                    else if (rand == 1)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color2);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color2 * 2);
                        lichter[j].GetComponent<Light>().color = color2 * 3;
                    }
                    else if (rand == 2)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color3);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color3 * 2);
                        lichter[j].GetComponent<Light>().color = color3 * 3;
                    }
                    else if(rand == 3)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color4);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color4 * 2);
                        lichter[j].GetComponent<Light>().color = color4 * 3;
                    }
                    else
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color5);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color5 * 2);
                        lichter[j].GetComponent<Light>().color = color5 * 3;
                    }
                }

            }

            Debug.Log(random);


            for (int j = 0; j < lichter.Length; j++)
            {

                lichter[j].tag = "LightDone";
                

            }
            ketAr[i] = lichter;
            Debug.Log(ketAr[i].Length);


        }









        for (int i = 0; i < numHaenger; i++)
        {
            int posX = Random.Range(0, 200);
            int posZ = Random.Range(0, 200);
            int random = Random.Range(1, 5);
            Vector3 pos = new Vector3(posX, 20, posZ);
            Instantiate(haenger, pos, Quaternion.identity);
            GameObject[] lichter = GameObject.FindGameObjectsWithTag("Light");

            //random Farbvergabe fuer Lichter
            if (random == 1)
            {
                for (int j = 0; j < lichter.Length; j++)
                {
                    if (j % 2 == 0 || j == 0)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color1);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color1 * 2);
                        lichter[j].GetComponent<Light>().color = color1 * 3;
                    }
                    else
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color2);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color2 * 2);
                        lichter[j].GetComponent<Light>().color = color2 * 3;
                    }
                }
            }

            if (random == 2)
            {
                for (int j = 0; j < lichter.Length; j++)
                {
                    if (j % 2 == 0 || j == 0)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color3);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color3 * 2);
                        lichter[j].GetComponent<Light>().color = color3 * 3;
                    }
                    else
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color4);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color4 * 2);
                        lichter[j].GetComponent<Light>().color = color4 * 3;
                    }
                }
            }

            if (random == 3)
            {
                for (int j = 0; j < lichter.Length; j++)
                {
                    lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color5);
                    lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color5 * 2);
                    lichter[j].GetComponent<Light>().color = color5 * 3;
                }
            }

            if (random == 4)
            {
                for (int j = 0; j < lichter.Length; j++)
                {
                    int rand = Random.Range(0, 5);

                    if (rand == 0)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color1);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color1 * 2);
                        lichter[j].GetComponent<Light>().color = color1 * 3;
                    }
                    else if (rand == 1)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color2);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color2 * 2);
                        lichter[j].GetComponent<Light>().color = color2 * 3;
                    }
                    else if (rand == 2)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color3);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color3 * 2);
                        lichter[j].GetComponent<Light>().color = color3 * 3;
                    }
                    else if (rand == 3)
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color4);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color4 * 2);
                        lichter[j].GetComponent<Light>().color = color4 * 3;
                    }
                    else
                    {
                        lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color5);
                        lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color5 * 2);
                        lichter[j].GetComponent<Light>().color = color5 * 3;
                    }
                }

            }

            Debug.Log(random);

            for (int j = 0; j < lichter.Length; j++)
            {

                lichter[j].tag = "LightDone";


            }
            haengAr[i] = lichter;
            Debug.Log(haengAr[i].Length);
        }

        //save created cubes in array "cubes"
        cubes = GameObject.FindGameObjectsWithTag("LightDone");
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
