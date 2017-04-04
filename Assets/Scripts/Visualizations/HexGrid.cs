using UnityEngine;
using System.Collections;

public class HexGrid : MonoBehaviour {
    private float hexHeight = 1f;
    private float hexwidth = 1f;
    public GameObject hex;
    private GameObject[] hexes = new GameObject[100];
    private float gap = 1f;
    private Vector3 startPos = new Vector3(0,0,0);
    public int hexNumber = 100;
    public float newScale = 500;
    public float timing = 5f;

    // Use this for initialization
    void Start () {
        Vector3 pos = new Vector3(startPos.x, startPos.y, startPos.z);
        for (int i = 1; i<hexNumber; i++)
        {
            if (i > 1)
            {
                pos = pos + new Vector3(0, 0, -hexHeight - gap);
            }
            for (int j = 0; j < i; j++) {
                if (j == 0)
                {
                    pos = new Vector3(startPos.x, pos.y, pos.z);
                }
                else
                {
                    pos = pos + new Vector3(hexwidth + gap, 0, 0);
                    Instantiate(hex, pos, Quaternion.identity);
                }
            }
        }
        hexes = GameObject.FindGameObjectsWithTag("Hexes");
        Debug.Log(hexes.Length);
    }
	
	// Update is called once per frame
	void Update () {
        float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming); //Reading the spectrum from the song put into the AudioListener 

        /**
         * Scaling the height of each cube, based on the value in the spectrum-array
         **/
        for (int i = 1; i < hexes.Length; i++)
        {
            Vector3 previousScale = hexes[i].transform.localScale;


            if (spectrum[i] * newScale > previousScale.y)   //Only update the cube if the height is above set value (1.3)
            {
                previousScale.y = Mathf.Lerp(previousScale.y, spectrum[i] * newScale*(i/2), Time.deltaTime * timing);

            }
            else
            {
                previousScale.y -= Random.Range(0.1f, 0.5f);

            }
            hexes[i].transform.localScale = previousScale;

        }
    }
}
