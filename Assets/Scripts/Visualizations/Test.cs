using UnityEngine;
using System.Collections;


/**
 * Testfile for testing the influence the spectrum has on the cubes
 **/ 
public class Test : MonoBehaviour {
    public float newScale = 20;
    public GameObject[] bass;
    public int number = 5;
    public int s = 1;

	
	void Start () {
        bass = GameObject.FindGameObjectsWithTag("Bass");
	}
	
	
	void Update () {
        float[] spectrum = AudioListener.GetSpectrumData(1024, 0, FFTWindow.Hamming);

        Vector3 previousScale = bass[number].transform.localScale;
        if (spectrum[s] * newScale > 1.3)
        {
            previousScale.y = Mathf.Lerp(previousScale.y, spectrum[s] * newScale, Time.deltaTime * 30);

        }
        else
        {
            previousScale.y = 1;

        }
        bass[number].transform.localScale = previousScale;
        
    }
}
