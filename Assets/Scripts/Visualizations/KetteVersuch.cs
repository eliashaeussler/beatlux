using UnityEngine;
using System.Collections;

public class KetteVersuch : MonoBehaviour
{

    //Game Objects to store the needed Prefabs
    public GameObject ketten;
    public GameObject haenger;

    //integers describing how many of the different Game Objects have to be created
    public int numKetten;
    public int numHaenger;
    
    //arrays to store the lights of the different lightchains
    public GameObject[][] ketAr;
    public GameObject[][] haengAr;

    //arrays to store the random range of spectrum displayed by the different lights
    public int[][] randKet;
    public int[][] randHaeng;

    //Colors
    Color color1 = new Color(1, 0, 0, 1);
    Color color2 = new Color(0, 1, 0, 1);
    Color color3 = new Color(0, 0, 1, 1);
    Color color4 = new Color(1, 1, 0, 1);
    Color color5 = new Color(0, 1, 1, 1);



    void Start()
    {

        //initializing the arrays to store the lights
        ketAr = new GameObject[numKetten][];
        haengAr = new GameObject[numHaenger][];

        /**
         *  creating the chains
         *  assigning colors to lights
         **/

        for (int i = 0; i < numKetten; i++)
        {

            //creating random numbers for x-Position, y-Position and type of coloring
            int posX = Random.Range(0, 200);
            int posZ = Random.Range(0, 200);
            int random = Random.Range(1, 5);

            //Intantiating object and saving lights into an array
            Vector3 pos = new Vector3(posX, -10, posZ);
            Instantiate(ketten, pos, Quaternion.identity);
            GameObject[] lichter = GameObject.FindGameObjectsWithTag("Light");


            //assigning colors to chains


            // chain with colors 1 and 2(alternating)
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


            //chain with colors 3 and 4(alternating)
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

            //chain with color 5
            if (random == 3)
            {
                for (int j = 0; j < lichter.Length; j++)
                {
                    lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color5);
                    lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color5 * 2);
                    lichter[j].GetComponent<Light>().color = color5 * 3;
                }
            }


            //chain with all colors(randomly)
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

            //changing tags of light(necessary for sorting lights)
            for (int j = 0; j < lichter.Length; j++)
            {

                lichter[j].tag = "LightDone";


            }

            //storing lights in ketAr
            ketAr[i] = lichter;
            Debug.Log(ketAr[i].Length);


        }




       /**
         *creating the hanging chains
         *assigning colors to lights
        */


        for (int i = 0; i < numHaenger; i++)
        {
            //creating random numbers for x-Position, y-Position and type of coloring
            int posX = Random.Range(0, 200);
            int posZ = Random.Range(0, 200);
            int random = Random.Range(1, 5);

            //Intantiating object and saving lights into an array
            Vector3 pos = new Vector3(posX, 20, posZ);
            Instantiate(haenger, pos, Quaternion.identity);
            GameObject[] lichter = GameObject.FindGameObjectsWithTag("Light");


            //assigning colors to chains


            // chain with colors 1 and 2(alternating)
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


            //chain with colors 3 and 4(alternating)
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


            //chain with color 5
            if (random == 3)
            {
                for (int j = 0; j < lichter.Length; j++)
                {
                    lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", color5);
                    lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", color5 * 2);
                    lichter[j].GetComponent<Light>().color = color5 * 3;
                }
            }


            //chain with all colors(randomly)
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

            //changing tags of light(necessary for sorting lights)
            for (int j = 0; j < lichter.Length; j++)
            {

                lichter[j].tag = "LightDone";


            }

            //storing lights in ketAr
            haengAr[i] = lichter;
            Debug.Log(haengAr[i].Length);
        }

        
        
        
        
        //instantiate arrays to store which part of the spectrum is displayed by every light
        randKet = new int[ketAr.Length][];
        randHaeng = new int[haengAr.Length][];
        /*
         * get random numbers to determine the displayed spectrum of a chain
         **/
        for (int i = 0; i < ketAr.Length; i++)
        {
            int rand = Random.Range(0, 200);
            int[] zw = new int[ketAr[i].Length];

            for (int j = 0; j < ketAr[i].Length; j++)
            {
                zw[j] = rand;
                rand++;
            }
            randKet[i] = zw;
        }

        for (int i = 0; i < haengAr.Length; i++)
        {
            int rand = Random.Range(0, 200);
            int[] zw = new int[haengAr[i].Length];

            for (int j = 0; j < haengAr[i].Length; j++)
            {
                zw[j] = rand;
                rand++;
            }
            randHaeng[i] = zw;
        }


    }





    void Update()
    {
        float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming); //Reading the spectrum from the song put into the AudioListener 

        /**
         * change the light intensity in reaction to the spectrum
         * change the rim Power(Glow) in reaction to the Spectrum
         **/
        for (int i = 0; i < ketAr.Length; i++)
        {
            for (int j = 0; j < ketAr[i].Length; j++)
            {
                ketAr[i][j].GetComponent<Renderer>().material.SetFloat("_RimPower", 1 / (spectrum[randKet[i][j]] * 10));
                ketAr[i][j].GetComponent<Light>().intensity = spectrum[randKet[i][j]] * 10;
                ketAr[i][j].GetComponent<Light>().bounceIntensity = spectrum[randKet[i][j]] * 10;
            }


        }

        for (int i = 0; i < haengAr.Length; i++)
        {
            for (int j = 0; j < haengAr[i].Length; j++)
            {
                haengAr[i][j].GetComponent<Renderer>().material.SetFloat("_RimPower", 1 / (spectrum[randHaeng[i][j]] * 10));
                haengAr[i][j].GetComponent<Light>().intensity = spectrum[randHaeng[i][j]] * 10;
                haengAr[i][j].GetComponent<Light>().bounceIntensity = spectrum[randHaeng[i][j]] * 10;
            }

        }

    }
}