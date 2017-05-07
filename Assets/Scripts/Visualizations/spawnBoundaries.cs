using UnityEngine;
using System.Collections;

public class spawnBoundaries : MonoBehaviour {
    public GameObject boundaries;
    // Use this for initialization
    void Start () {
		Instantiate(boundaries, Vector3.zero, Quaternion.identity, Settings.MenuManager.vizContents);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
