/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    private static TutorialManager _instance;

    private Tutorial currentTutorial;
    public Text ExpText;

    public List<Tutorial> Tutorials = new List<Tutorial>();

    //creates an instance of the Tutorialmanager
    public static TutorialManager Instace
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<TutorialManager>();

            if (_instance == null) Debug.Log("There is no TutorialManager");

            return _instance;
        }
    }

    // sets the tutorialstart to tutorial 0
    private void Start()
    {
        SetNextTutorial(0);
    }

    // if a tutorial is there, checks if sth is happening
    private void Update()
    {
        if (currentTutorial) currentTutorial.CheckIfHappening();
    }

    // called if a tutorial is finished
    public void CompletedTutorial()
    {
        SetNextTutorial(currentTutorial.Order + 1);
    }

    // sets next tutorial if possible, else finish
    private void SetNextTutorial(int currentOrder)
    {
        currentTutorial = GetTutorialByOrder(currentOrder);

        if (!currentTutorial)
        {
            CompletedAllTutorials();
            SetNextTutorial(0);
        }

        ExpText.text = Settings.MenuManager.LangManager.getString("MM0" + currentOrder + "");
    }

    // text if every tutorial is finished
    private void CompletedAllTutorials()
    {
        ExpText.text = "You completed all the tutorials";
    }

    // gets the tutorial order
    private Tutorial GetTutorialByOrder(int order)
    {
        return Tutorials.FirstOrDefault(t => t.Order == order);
    }
}