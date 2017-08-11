using UnityEngine;
using System.Collections;

public class HexGrid : MonoBehaviour
{
    private float hexHeight = 30f;
    private float hexwidth = 30f;
    public GameObject hex;
    private GameObject[] hexes = new GameObject[100];
    private float gap = 10f;
    private Vector3 startPos = new Vector3(0, 0, 0);
    public int hexNumber = 100;
    public float newScale = 500;
    public float timing = 5f;
    public GameObject tempChild;
    private ColorSchemeObj colorScheme;

    /**
          * Initializing the scene, creating a field of hexagons shaped like a triangle.
          **/
    void Start()
    {
        Vector3 pos = new Vector3(startPos.x, startPos.y, startPos.z);
        for (int i = 1; i < hexNumber; i++)
        {
            if (i > 1)
            {
                pos = pos + new Vector3(0, 0, -hexHeight - gap);
            }
            for (int j = 0; j < i; j++)
            {
                if (j == 0)
                {
                    pos = new Vector3(startPos.x, pos.y, pos.z);
                }
                else
                {
                    pos = pos + new Vector3(hexwidth + gap, 0, 0);
                    Instantiate(hex, pos, Quaternion.identity, Settings.MenuManager.vizContents);
                }
            }
        }
        hexes = GameObject.FindGameObjectsWithTag("Hexes");
        colorScheme = Settings.Active.ColorScheme;
    }

    
    void Update()
    {
        float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming); //Reading the spectrum from the song put into the AudioListener 

        /**
         * Scaling the height of each cube, based on the value in the spectrum-array
         **/
        for (int i = 0; i < hexes.Length; i++)
        {

            tempChild = hexes[i].transform.Find("Cylinder").gameObject;
            tempChild.GetComponent<Renderer>().material.color = colorScheme.Colors[0];
            tempChild = hexes[i].transform.Find("CylinderTop").gameObject;
            tempChild.GetComponent<Renderer>().material.color = colorScheme.Colors[1];

            Vector3 previousScale = hexes[i].transform.localScale;

            if (spectrum[i] * newScale > 1.2)
            {
                previousScale.y = Mathf.Lerp(previousScale.y, spectrum[i] * newScale *((i+4)/3), Time.deltaTime * timing);

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
