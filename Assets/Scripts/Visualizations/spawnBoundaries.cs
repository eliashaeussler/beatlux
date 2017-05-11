using UnityEngine;
using System.Collections;
using System.IO;

public class spawnBoundaries : MonoBehaviour {
    public GameObject boundaries;
    public GameSettings gameSettings;
    public GameObject mirrors;
    // Use this for initialization
    void Start()
    {
        Instantiate(boundaries, Vector3.zero, Quaternion.identity, Settings.MenuManager.vizContents);
        if (File.Exists(Application.persistentDataPath + "/gamesettings.json") == true)
        {
            gameSettings = JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
            switch (gameSettings.mirrors)
            {
                case 0:
                    break;
                case 1:
                    Instantiate(mirrors, new Vector3(0,-10,0), Quaternion.identity, Settings.MenuManager.vizContents);
                    mirrors.GetComponent<MirrorReflection>().m_TextureSize = 256;
                    break;
                case 2:
                    Instantiate(mirrors, mirrors.transform.position, Quaternion.identity, Settings.MenuManager.vizContents);
                    mirrors.GetComponent<MirrorReflection>().m_TextureSize = 512;
                    break;
                default:
                    break;
            }

        }
    }
	// Update is called once per frame
	void Update () {
	
	}
}
