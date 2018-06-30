/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

public class Bubbles : MonoBehaviour
{
    private const float Timing = 5f;


    //Array to store the spheres
    private readonly Kugel[] _spheres = new Kugel[5000];
    public int Amount;
    public int Height;
    public int MaxSize;
    public int MinSize;
    public int RangeXmax;
    public int RangeXmin;
    public int RangeYmax;
    public int RangeYmin;


    // Vars to store the prefab, max height of the objects, range of spawning, range of size and amount of bubbles
    public GameObject Sphere;


    // Update is called once per frame
    private void Update()
    {
        // Reading the spectrum from the song put into the AudioListener
        var spectrum = new float[1024];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Hamming);

        var samples = new float[1024];
        AudioListener.GetOutputData(samples, 0);


        //Adding up all the samples, for approximate overall "volume"
        float vol = 0;
        foreach (var sample in samples)
            if (sample >= 0)
                vol += sample;
            else
                vol -= sample;


        //Adding up first 20 floats in spectrum to determine a volume for the bass
        float bass = 0;
        for (var i = 0; i < 30; i++)
            if (spectrum[i] >= 0)
                bass += spectrum[i];
            else
                bass -= spectrum[i];


        /*
         * spawn spheres according to the volume of the music
         * 
         * */
        for (var i = 0; i < vol / 100 * Amount; i++)

        {
            //create Random values for position, size, color and speed

            var posX = Random.Range(RangeXmin, RangeXmax);
            var posZ = Random.Range(RangeYmin, RangeYmax);
            var col = Random.Range(0, 4);
            var size = Random.Range(MinSize, MaxSize);
            var speed = Random.Range(1, 6);
            var pos = new Vector3(posX, -10, posZ);

            //create an Object Kugel with these values
            var kug = new Kugel(Sphere, pos, size, speed);

            // assign color to object
            if (col < Settings.Active.ColorScheme.Colors.Length)
                kug.TheSphere.GetComponent<Renderer>().material.color = Settings.Active.ColorScheme.Colors[col];


            // store kug  in spheres
            for (var f = 0; f < _spheres.Length; f++)
            {
                if (_spheres[f] != null && _spheres[f].TheSphere != null) continue;

                _spheres[f] = kug;
                break;
            }
        }

        //destroy objects if they are too high
        var look = GameObject.FindGameObjectsWithTag("Spheres");
        foreach (var element in look)
            if (element.transform.position.y >= 200)
                Destroy(element);

        //change Size according to bass
        try
        {
            foreach (var sphere in _spheres) sphere.groesse(bass);
        }
        catch
        {
            // ignored
        }
    }

    /*
     * Class to store spheres and different values connected to them
     * 
     * */
    private class Kugel
    {
        private readonly int _size;

        // Vars to store the Sphere, its default-size and its speed
        public readonly GameObject TheSphere;
        private Vector3 _prevScale;
        private int _speed;


        /*
         * Constructor for Kugel
         * 
         * Attributes : sphere to load prefab, pos to determine the spawnposition, int size for default size and speed to determine speed
         * GameObject gets Instantiated 
         * 
         * */
        public Kugel(GameObject sphere, Vector3 pos, int size, int speed)
        {
            TheSphere = Instantiate(sphere, pos, Quaternion.identity, Settings.MenuManager.VizContents);
            TheSphere.GetComponent<Rigidbody>().AddForce(0, speed * 500, 0);
            _size = size;
            _prevScale = new Vector3(size, size, size);
            _speed = speed;
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
            if (fl > 1)
            {
                scale.x = Mathf.Lerp(_prevScale.x, fl * _size * 3, Time.deltaTime * Timing);
                scale.y = Mathf.Lerp(_prevScale.y, fl * _size * 3, Time.deltaTime * Timing);
                scale.z = Mathf.Lerp(_prevScale.z, fl * _size * 3, Time.deltaTime * Timing);
                TheSphere.transform.localScale = scale;
                _prevScale = scale;
            }
            else
            {
                scale.x = Mathf.Lerp(_prevScale.x, _size, Time.deltaTime * Timing);
                scale.y = Mathf.Lerp(_prevScale.y, _size, Time.deltaTime * Timing);
                scale.z = Mathf.Lerp(_prevScale.z, _size, Time.deltaTime * Timing);
                TheSphere.transform.localScale = scale;
                _prevScale = scale;
            }
        }
    }
}