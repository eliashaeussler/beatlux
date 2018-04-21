/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using System.Collections;


public class Bubbles : MonoBehaviour {

    public static float timing = 5f;
    /*
     * Class to store spheres and different values connected to them
     * 
     * */
    public class Kugel
    {
        // Vars to store the Sphere, its default-size and its speed
        public GameObject theSphere;
        public int size;
        public int speed;
        public Vector3 prevScale;


        /*
         * Constructor for Kugel
         * 
         * Attributes : sphere to load prefab, pos to determine the spawnposition, int size for default size and speed to determine speed
         * GameObject gets Instantiated 
         * 
         * */
        public Kugel(GameObject sphere, Vector3 pos, int size, int speed)
        {
			this.theSphere = (GameObject)Instantiate(sphere, pos, Quaternion.identity, Settings.MenuManager.vizContents);
            this.theSphere.GetComponent<Rigidbody>().AddForce(0, speed*500, 0);
            this.size = size;
            prevScale = new Vector3 (size, size, size);
            this.speed = speed;
        }


        /*
         * 
         * Method to change the sizen according to the bass
         * if float fl is bigger than 1: multiply size by float
         * else scale with the default size saved 
         * 
         * */
        public void groesse(float fl)
        {
           
            Vector3 scale;
            if(fl > 1)
            {
            scale.x = Mathf.Lerp(prevScale.x, fl*size*3, Time.deltaTime * timing);
            scale.y = Mathf.Lerp(prevScale.y, fl*size*3, Time.deltaTime * timing);
            scale.z = Mathf.Lerp(prevScale.z, fl*size*3, Time.deltaTime * timing);
            this.theSphere.transform.localScale = scale;
            prevScale = scale;
            
            

            }
            else
            {

                    scale.x = Mathf.Lerp(prevScale.x,size, Time.deltaTime * timing);
                    scale.y = Mathf.Lerp(prevScale.y,size, Time.deltaTime * timing);
                    scale.z = Mathf.Lerp(prevScale.z,size, Time.deltaTime * timing);
                    this.theSphere.transform.localScale = scale;
                    prevScale = scale;

                

            }
            

        }
    }


    // Vars to store the prefab, max height of the objects, range of spawning, range of size and amount of bubbles
    public GameObject sphere;
    public int height;
    public int rangeXmin;
    public int rangeXmax;
    public int rangeYmin;
    public int rangeYmax;
    public int minSize;
    public int maxSize;
    public int amount;


    //Array to store the spheres
    Kugel[] spheres = new Kugel[5000];



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

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


        //Adding up first 20 floats in spectrum to determine a volume for the bass
        float bass = 0;
        for( int i = 0; i<30; i++)
        {
            if (spectrum[i] >= 0)
            {
                bass += spectrum[i];
            }
            else bass -= spectrum[i];


        }




        /*
         * spawn spheres according to the volume of the music
         * 
         * */
        for (int i = 0; i < (vol/100) * amount; i++)
            
            {
           
            //create Random values for position, size, color and speed

                int posX = Random.Range(rangeXmin, rangeXmax);
                int posZ = Random.Range(rangeYmin, rangeYmax);
                int col = Random.Range(0, 4);
                int size = Random.Range(minSize,maxSize);
                int speed = Random.Range(1, 6);
                Vector3 pos = new Vector3(posX, -10, posZ);
                
            //create an Object Kugel with these values
                Kugel kug = new Kugel(sphere,pos, size,speed);
            
            // assign color to object
			if (col < Settings.Active.ColorScheme.Colors.Length)
				kug.theSphere.GetComponent<Renderer>().material.color = Settings.Active.ColorScheme.Colors [col];
                

                // store kug  in spheres
                for(int f = 0; f < spheres.Length; f++)
                {
                    if(spheres[f] == null || spheres[f].theSphere == null)
                    {
                        spheres[f] = kug;
                        break;

                    }
                }


           
            

            

        }

        //destroy objects if they are too high
        GameObject[] look = GameObject.FindGameObjectsWithTag("Spheres");
        for (int j = 0; j < look.Length; j++)
        {
            if (look[j].transform.position.y >= 200)
            {
                Destroy(look[j]);
            }
        }

        //change Size according to bass
        try
        {
            for (int f = 0; f < spheres.Length; f++)
            {
                spheres[f].groesse(bass);
            }
        }
        catch
        {

        }






    }
}
