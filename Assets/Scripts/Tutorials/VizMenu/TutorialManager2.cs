using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager2 : MonoBehaviour {

    public List<Tutorial2> Tutorials = new List<Tutorial2>();
    public Text ExpText;
    private static TutorialManager2 instance;

    //creates an instance of the Tutorialmanager
    public static TutorialManager2 Instace
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<TutorialManager2>();
            }

            if (instance == null)
            {
                Debug.Log("There is no TutorialManager");
            }

            return instance;
        }
    }

    private Tutorial2 currentTutorial;

    // sets the tutorialstart to tutorial 0
    void Start()
    {
        SetNextTutorial(0);
    }

    // if a tutorial is there, checks if sth is happening
    void Update()
    {
        if (currentTutorial)
        {
            currentTutorial.CheckIfHappening();
        }
    }

    // called if a tutorial is finished
    public void CompletedTutorial()
    {
        SetNextTutorial(currentTutorial.Order + 1);
    }

    // sets next tutorial if possible, else finish
    public void SetNextTutorial(int currentOrder)
    {
        currentTutorial = GetTutorialByOrder(currentOrder);

        if (!currentTutorial)
        {
            CompletedAllTutorials();
            SetNextTutorial(0);
        }

        ExpText.text = Settings.MenuManager.LangManager.getString("VM0" + currentOrder + "");
    }

    // text if every tutorial is finished
    public void CompletedAllTutorials()
    {
        ExpText.text = "You completed all the tutorials";
    }

    // gets the tutorial order
    public Tutorial2 GetTutorialByOrder(int Order)
    {
        for (int i = 0; i < Tutorials.Count; i++)
        {
            if (Tutorials[i].Order == Order)
            {
                return Tutorials[i];
            }
        }


        return null;
    }
}
