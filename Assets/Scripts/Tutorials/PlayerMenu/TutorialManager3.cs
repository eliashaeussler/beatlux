using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class TutorialManager3 : MonoBehaviour {

    public List<Tutorial3> Tutorials = new List<Tutorial3>();
    public Text ExpText;
    private static TutorialManager3 instance;
    public static TutorialManager3 Instace
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<TutorialManager3>();
            }

            if (instance == null)
            {
                Debug.Log("There is no TutorialManager");
            }

            return instance;
        }
    }

    private Tutorial3 currentTutorial;

    // Use this for initialization
    void Start()
    {
        SetNextTutorial(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentTutorial)
        {
            currentTutorial.CheckIfHappening();
        }
    }

    public void CompletedTutorial()
    {
        SetNextTutorial(currentTutorial.Order + 1);
    }

    public void SetNextTutorial(int currentOrder)
    {
        currentTutorial = GetTutorialByOrder(currentOrder);

        if (!currentTutorial)
        {
            CompletedAllTutorials();
            SetNextTutorial(0);
        }

        ExpText.text = currentTutorial.Explanation;
    }

    public void CompletedAllTutorials()
    {
        ExpText.text = "You completed all the tutorials";
    }

    public Tutorial3 GetTutorialByOrder(int Order)
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
