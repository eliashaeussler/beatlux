/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

public class KetteVersuch : MonoBehaviour
{
    //Colors
    private Color _color1 = new Color(1, 0, 0, 1);
    private Color _color2 = new Color(0, 1, 0, 1);
    private Color _color3 = new Color(0, 0, 1, 1);
    private Color _color4 = new Color(1, 1, 0, 1);
    private Color _color5 = new Color(0, 1, 1, 1);

    private Color[] _colors;
    public GameObject[][] HaengAr;
    public GameObject Haenger;

    //arrays to store the lights of the different lightchains
    public GameObject[][] KetAr;

    //Game Objects to store the needed Prefabs
    public GameObject Ketten;
    public int NumHaenger;

    //integers describing how many of the different Game Objects have to be created
    public int NumKetten;
    public int[][] RandHaeng;

    //arrays to store the random range of spectrum displayed by the different lights
    public int[][] RandKet;

    private void Start()
    {
        _colors = Settings.Active.ColorScheme.Colors;

        _color1 = _colors[0];
        _color2 = _colors[1];
        _color3 = _colors[2];
        _color4 = _colors[3];
        _color5 = _colors[4];

        //initializing the arrays to store the lights
        KetAr = new GameObject[NumKetten][];
        HaengAr = new GameObject[NumHaenger][];

        /**
         *  creating the chains
         *  assigning colors to lights
         **/

        for (var i = 0; i < NumKetten; i++)
        {
            //creating random numbers for x-Position, y-Position and type of coloring
            var posX = Random.Range(0, 200);
            var posZ = Random.Range(0, 200);
            var random = Random.Range(1, 5);

            //Intantiating object and saving lights into an array
            var pos = new Vector3(posX, -10, posZ);
            Instantiate(Ketten, pos, Quaternion.identity, Settings.MenuManager.VizContents);
            var lichter = GameObject.FindGameObjectsWithTag("Light");


            //assigning colors to chains


            switch (random)
            {
                // chain with colors 1 and 2(alternating)
                case 1:
                    for (var j = 0; j < lichter.Length; j++)
                        if (j % 2 == 0 || j == 0)
                        {
                            lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", _color1);
                            lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", _color1 * 2);
                            lichter[j].GetComponent<Light>().color = _color1 * 3;
                        }
                        else
                        {
                            lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", _color2);
                            lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", _color2 * 2);
                            lichter[j].GetComponent<Light>().color = _color2 * 3;
                        }

                    break;

                //chain with colors 3 and 4(alternating)
                case 2:
                    for (var j = 0; j < lichter.Length; j++)
                        if (j % 2 == 0 || j == 0)
                        {
                            lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", _color3);
                            lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", _color3 * 2);
                            lichter[j].GetComponent<Light>().color = _color3 * 3;
                        }
                        else
                        {
                            lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", _color4);
                            lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", _color4 * 2);
                            lichter[j].GetComponent<Light>().color = _color4 * 3;
                        }

                    break;

                //chain with color 5
                case 3:
                    foreach (var licht in lichter)
                    {
                        licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color5);
                        licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color5 * 2);
                        licht.GetComponent<Light>().color = _color5 * 3;
                    }

                    break;

                //chain with all colors(randomly)
                case 4:
                    foreach (var licht in lichter)
                    {
                        var rand = Random.Range(0, 5);

                        switch (rand)
                        {
                            case 0:
                                licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color1);
                                licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color1 * 2);
                                licht.GetComponent<Light>().color = _color1 * 3;
                                break;
                            case 1:
                                licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color2);
                                licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color2 * 2);
                                licht.GetComponent<Light>().color = _color2 * 3;
                                break;
                            case 2:
                                licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color3);
                                licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color3 * 2);
                                licht.GetComponent<Light>().color = _color3 * 3;
                                break;
                            case 3:
                                licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color4);
                                licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color4 * 2);
                                licht.GetComponent<Light>().color = _color4 * 3;
                                break;
                            default:
                                licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color5);
                                licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color5 * 2);
                                licht.GetComponent<Light>().color = _color5 * 3;
                                break;
                        }
                    }

                    break;
            }


            //changing tags of light(necessary for sorting lights)
            foreach (var licht in lichter) licht.tag = "LightDone";

            //storing lights in ketAr
            KetAr[i] = lichter;
        }


        /**
          *creating the hanging chains
          *assigning colors to lights
         */


        for (var i = 0; i < NumHaenger; i++)
        {
            //creating random numbers for x-Position, y-Position and type of coloring
            var posX = Random.Range(0, 200);
            var posZ = Random.Range(0, 200);
            var random = Random.Range(1, 5);

            //Intantiating object and saving lights into an array
            var pos = new Vector3(posX, 20, posZ);
            Instantiate(Haenger, pos, Quaternion.identity, Settings.MenuManager.VizContents);
            var lichter = GameObject.FindGameObjectsWithTag("Light");


            //assigning colors to chains


            switch (random)
            {
                // chain with colors 1 and 2(alternating)
                case 1:
                    for (var j = 0; j < lichter.Length; j++)
                        if (j % 2 == 0 || j == 0)
                        {
                            lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", _color1);
                            lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", _color1 * 2);
                            lichter[j].GetComponent<Light>().color = _color1 * 3;
                        }
                        else
                        {
                            lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", _color2);
                            lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", _color2 * 2);
                            lichter[j].GetComponent<Light>().color = _color2 * 3;
                        }

                    break;

                //chain with colors 3 and 4(alternating)
                case 2:
                    for (var j = 0; j < lichter.Length; j++)
                        if (j % 2 == 0 || j == 0)
                        {
                            lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", _color3);
                            lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", _color3 * 2);
                            lichter[j].GetComponent<Light>().color = _color3 * 3;
                        }
                        else
                        {
                            lichter[j].GetComponent<Renderer>().material.SetColor("_ColorTint", _color4);
                            lichter[j].GetComponent<Renderer>().material.SetColor("_RimColor", _color4 * 2);
                            lichter[j].GetComponent<Light>().color = _color4 * 3;
                        }

                    break;

                //chain with color 5
                case 3:
                    foreach (var licht in lichter)
                    {
                        licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color5);
                        licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color5 * 2);
                        licht.GetComponent<Light>().color = _color5 * 3;
                    }

                    break;

                //chain with all colors(randomly)
                case 4:
                    foreach (var licht in lichter)
                    {
                        var rand = Random.Range(0, 5);

                        switch (rand)
                        {
                            case 0:
                                licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color1);
                                licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color1 * 2);
                                licht.GetComponent<Light>().color = _color1 * 3;
                                break;
                            case 1:
                                licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color2);
                                licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color2 * 2);
                                licht.GetComponent<Light>().color = _color2 * 3;
                                break;
                            case 2:
                                licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color3);
                                licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color3 * 2);
                                licht.GetComponent<Light>().color = _color3 * 3;
                                break;
                            case 3:
                                licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color4);
                                licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color4 * 2);
                                licht.GetComponent<Light>().color = _color4 * 3;
                                break;
                            default:
                                licht.GetComponent<Renderer>().material.SetColor("_ColorTint", _color5);
                                licht.GetComponent<Renderer>().material.SetColor("_RimColor", _color5 * 2);
                                licht.GetComponent<Light>().color = _color5 * 3;
                                break;
                        }
                    }

                    break;
            }


            //changing tags of light(necessary for sorting lights)
            foreach (var licht in lichter) licht.tag = "LightDone";

            //storing lights in ketAr
            HaengAr[i] = lichter;
        }


        //instantiate arrays to store which part of the spectrum is displayed by every light
        RandKet = new int[KetAr.Length][];
        RandHaeng = new int[HaengAr.Length][];
        /*
         * get random numbers to determine the displayed spectrum of a chain
         **/
        for (var i = 0; i < KetAr.Length; i++)
        {
            var rand = Random.Range(0, 200);
            var zw = new int[KetAr[i].Length];

            for (var j = 0; j < KetAr[i].Length; j++)
            {
                zw[j] = rand;
                rand++;
            }

            RandKet[i] = zw;
        }

        for (var i = 0; i < HaengAr.Length; i++)
        {
            var rand = Random.Range(0, 200);
            var zw = new int[HaengAr[i].Length];

            for (var j = 0; j < HaengAr[i].Length; j++)
            {
                zw[j] = rand;
                rand++;
            }

            RandHaeng[i] = zw;
        }
    }


    private void Update()
    {
        // Reading the spectrum from the song put into the AudioListener
        var spectrum = new float[1024];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);

        if (KetAr == null)
            Start();

        /**
         * change the light intensity in reaction to the spectrum
         * change the rim Power(Glow) in reaction to the Spectrum
         **/
        for (var i = 0; i < KetAr.Length; i++)
        for (var j = 0; j < KetAr[i].Length; j++)
        {
            KetAr[i][j].GetComponent<Renderer>().material.SetFloat("_RimPower", 1 / (spectrum[RandKet[i][j]] * 7));
            KetAr[i][j].GetComponent<Light>().intensity = spectrum[RandKet[i][j]] * 7;
            KetAr[i][j].GetComponent<Light>().bounceIntensity = spectrum[RandKet[i][j]] * 7;
        }

        for (var i = 0; i < HaengAr.Length; i++)
        for (var j = 0; j < HaengAr[i].Length; j++)
        {
            HaengAr[i][j].GetComponent<Renderer>().material.SetFloat("_RimPower", 1 / (spectrum[RandHaeng[i][j]] * 7));
            HaengAr[i][j].GetComponent<Light>().intensity = spectrum[RandHaeng[i][j]] * 7;
            HaengAr[i][j].GetComponent<Light>().bounceIntensity = spectrum[RandHaeng[i][j]] * 7;
        }
    }
}