using UnityEngine;
using System.Collections;

public class Bubbles : MonoBehaviour {



    public class Kugel
    {
        public GameObject theSphere;
        public int size;
        public int speed;

        public Kugel(GameObject sphere, Vector3 pos, int size, int speed)
        {
            this.theSphere = (GameObject)Instantiate(sphere, pos, Quaternion.identity);
            this.theSphere.GetComponent<Rigidbody>().AddForce(0, speed*500, 0);
            this.size = size;
            this.speed = speed;
        }

        public void groesse(float fl)
        {
            fl = fl / 15;
            Vector3 scale;
            if(fl > 1)
            {
            scale = new Vector3(size * fl, size * fl, size * fl);
            this.theSphere.transform.localScale = scale;

            }
            else
            {
              scale = new Vector3(size, size, size);
              this.theSphere.transform.localScale = scale;
            }
            

        }
    }



   public GameObject sphere;
   public int height;
    //Colors
    Color color1 = new Color(0,1,0,1);
    Color color2 = new Color(1,0,0,1);
    Color color3 = new Color(0,0,1,1);
    Color color4 = new Color(1,1,0,1);

    Kugel[] spheres = new Kugel[5000];
    int[] groeße;



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

        float bass = 0;
        for( int i = 0; i<30; i++)
        {
            if (samples[i] >= 0)
            {
                bass += samples[i];
            }
            else bass -= samples[i];
        }





        for (int i = 0; i < vol/100000; i++)
            
            {
           
            
                int posX = Random.Range(0, 200);
                int posZ = Random.Range(0, 200);
                int col = Random.Range(0, 4);
                int size = Random.Range(0, 10);
                int speed = Random.Range(1, 6);
                Vector3 pos = new Vector3(posX, -10, posZ);
                
                Kugel kug = new Kugel(sphere,pos, size,speed);
            

            if (col == 0)
                {
                    kug.theSphere.GetComponent<Renderer>().material.color = color1;
                }
                if(col == 1)
                {
                    kug.theSphere.GetComponent<Renderer>().material.color = color2;
                }
                if (col == 2)
                {
                    kug.theSphere.GetComponent<Renderer>().material.color = color3;
                }
                if (col == 3)
                {
                    kug.theSphere.GetComponent<Renderer>().material.color = color4;
                }
                
                for(int f = 0; f < spheres.Length; f++)
                {
                    if(spheres[f] == null || spheres[f].theSphere == null)
                    {
                        spheres[f] = kug;
                        break;

                    }
                }

            try
            {
                for (int f = 0; f < spheres.Length; f++)
                {
                    spheres[f].groesse(bass);
                }
            }
            catch{

            }

            GameObject[] look = GameObject.FindGameObjectsWithTag("Spheres");
            for(int j = 0; j< look.Length; j++)
            {
                if(look[j].transform.position.y >= 200)
                {
                    Destroy(look[j]);
                }
            }

            

        }

        

       


    }
}
