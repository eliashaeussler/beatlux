using UnityEngine;
using System.Collections;

public class Bubbles : MonoBehaviour {

   public GameObject sphere;
    public int height;
    //Colors
    Color color1 = new Color(0,1,0,1);
    Color color2 = new Color(1,0,0,1);
    Color color3 = new Color(0,0,1,1);
    Color color4 = new Color(1,1,0,1);

    GameObject[] spheres;



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

        for (int i = 0; i < 20; i++)
            
        {
           
            if (spectrum[i] >= 0.1 && i <= 20)
            {
                int posX = Random.Range(0, 200);
                int posZ = Random.Range(0, 200);
                int col = Random.Range(0, 4);
                Vector3 pos = new Vector3(posX, -10, posZ);
                GameObject kugel = (GameObject)Instantiate(sphere, pos, Quaternion.identity);
                if(col == 0)
                {
                    kugel.GetComponent<Renderer>().material.color = color1;
                }
                if(col == 1)
                {
                    kugel.GetComponent<Renderer>().material.color = color2;
                }
                if (col == 2)
                {
                    kugel.GetComponent<Renderer>().material.color = color3;
                }
                if (col == 3)
                {
                    kugel.GetComponent<Renderer>().material.color = color4;
                }
                spheres = GameObject.FindGameObjectsWithTag("Spheres");

                for(int j = 0; j < spheres.Length; j++)
                {
                    spheres[j].GetComponent<Rigidbody>().AddForce(0,25,0);

                    if(spheres[j].transform.position.y >= height)
                    {
                        Destroy(spheres[j]);

                    }
                   

                }
            }
        }


    }
}
