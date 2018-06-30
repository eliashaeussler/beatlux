/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.IO;
using UnityEngine;

public class spawnBoundaries : MonoBehaviour
{
    public GameObject Boundaries;
    public GameSettings GameSettings;
    public GameObject Mirrors;
    public bool MirrorsOnOff;


    /**
     * Spawns invisible walls around the middle of the gameobject it is put on.
     **/
    private void Start()
    {
        Instantiate(Boundaries, Vector3.zero, Quaternion.identity, Settings.MenuManager.VizContents);

        if (!File.Exists(Application.persistentDataPath + "/gamesettings.json") || !MirrorsOnOff) return;

        GameSettings =
            JsonUtility.FromJson<GameSettings>(File.ReadAllText(Application.persistentDataPath + "/gamesettings.json"));
        switch (GameSettings.Mirrors)
        {
            case 1:
                Instantiate(Mirrors, new Vector3(0, -10, 0), Quaternion.identity, Settings.MenuManager.VizContents);
                Mirrors.GetComponent<MirrorReflection>().TextureSize = 256;
                break;
            case 2:
                Instantiate(Mirrors, Mirrors.transform.position, Quaternion.identity, Settings.MenuManager.VizContents);
                Mirrors.GetComponent<MirrorReflection>().TextureSize = 512;
                break;
        }
    }
}