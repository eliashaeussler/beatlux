/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.Collections.Generic;
using UnityEngine;

public class DelayTutorial : MonoBehaviour
{
    private readonly List<GameObject> _list = new List<GameObject>();

    private bool _temp;

    private void Awake()
    {
        //Debug.Log (Settings.Player.TutorialTog);
        _list.Add(GameObject.Find("FocusShape2"));
        _list.Add(GameObject.Find("TutorialManager"));
        if (Settings.Player.TutorialTog)
        {
            GameObject.Find("FocusShape2").SetActive(true);
            GameObject.Find("TutorialManager").SetActive(true);
            _temp = true;
        }
        else if (GameObject.Find("FocusShape2") || GameObject.Find("TutorialManager"))
        {
            GameObject.Find("FocusShape2").SetActive(false);
            GameObject.Find("TutorialManager").SetActive(false);
            _temp = false;
        }
    }

    private void Update()
    {
        if (Settings.Player.TutorialTog == _temp) return;

        _list[0].SetActive(Settings.Player.TutorialTog);
        _list[1].SetActive(Settings.Player.TutorialTog);

        if (_list[1].GetComponent<TutorialManager3>() && _temp) _list[1].GetComponent<TutorialManager3>().Init();

        _temp = Settings.Player.TutorialTog;
    }
}