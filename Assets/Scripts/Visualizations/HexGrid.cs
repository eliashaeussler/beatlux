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
    public GameObject tempChild;
    private ColorSchemeObj colorScheme;

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
        colorScheme = Settings.Active.ColorScheme;
    }
	
	// Update is called once per frame
	void Update () {
        float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming); //Reading the spectrum from the song put into the AudioListener 

        /**
         * Scaling the height of each cube, based on the value in the spectrum-array
         **/
        for (int i = 1; i < hexes.Length; i++)
        {
            
            tempChild = hexes[i].transform.Find("Cylinder").gameObject;
            tempChild.GetComponent<Renderer>().material.color = colorScheme.Colors[0];
            tempChild = hexes[i].transform.Find("CylinderTop").gameObject;
            tempChild.GetComponent<Renderer>().material.color = colorScheme.Colors[1];
            
            Vector3 previousScale = hexes[i].transform.localScale;
            
            if (spectrum[i] * newScale > previousScale.y)   
            {
                previousScale.y = Mathf.Lerp(previousScale.y, spectrum[i] * newScale*(i/2), Time.deltaTime * timing);

            }
            else
            {
                if (previousScale.y > 1)
                {
                    previousScale.y -= Random.Range(0.01f, 0.2f);
                }

            }
            hexes[i].transform.localScale = previousScale;
            
        }
    }
}
