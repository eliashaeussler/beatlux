using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;
using UnityEditor;

public class SourceFolder : MonoBehaviour {

    public static SourceFolder sPath;
    int i;
    public string pathFinal;
    public string pathAll;
    string mainpath;


    // Use this for initialization
    void Start () {
        mainpath = @"C:\Users\u29880\Documents\hey";
        pathFinal = mainpath;
        init(mainpath);
    }
	
	// Update is called once per frame
	void Update () {

        // If the search entry is deleted, init() is called and the strings are set to null so update wont be called   
        if(MenuFunctions.pathF!=null){

            init(mainpath);
            MenuFunctions.pathF = null;
            MenuFunctions.searchResults = null;
        }
            // If something was searched 
        else if(MenuFunctions.searchResults!=null)
        {

            // deletes all previous created objects
            for (int counter = 0; counter <= i; counter++)
            {
                Destroy(GameObject.Find("MyTest" + counter));
            }
            Destroy(GameObject.Find("Back2"));

            // path and objects are initialised 
            i = 0;
            float x = -42.0f;
            float y = 200.0f;
            Transform child2 = transform.Find("Folders");
            Text file = child2.GetComponent<Text>();
            string[] filePaths = MenuFunctions.searchResults;

            foreach (string paths in filePaths)
            {

                i++;
                //creates a gameobject with a recttransform to position it
                GameObject fileObject = new GameObject("MyTest" + i);
                
                fileObject.transform.SetParent(file.transform);
                RectTransform trans = fileObject.AddComponent<RectTransform>();
                trans.anchoredPosition = new Vector2(x, y);
                trans.sizeDelta = new Vector2(290, 50);
                trans.localScale = new Vector3(1, 1, 1);


                //adds a text to the gameobjects which is filled and modified 
                Text myText = fileObject.AddComponent<Text>();
                myText.color = Color.black;
                myText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;

                // if the name is to long, it is shortend
                if (Path.GetFileName(paths).Length > 20)
                {
                    string zS = Path.GetFileName(paths);
                    zS = zS.Substring(0, 17);
                    zS = zS + "...";
                    myText.text = zS;
                }
                else
                {
                    myText.text = Path.GetFileName(paths);
                }
                myText.fontSize = 30;
                y = y - 50.0f;

            }

        }
        
		
	}


    
    // init creates all the objects
    void init(string pathFolder)
    {

        // deletes all previous created objects
        if (i >= 0)
        {
            for (int counter3 = 0; counter3 <= i; counter3++)
            {
                Destroy(GameObject.Find("MyTest" + counter3));
            }
            Destroy(GameObject.Find("Back2"));
        }
        i = 0;


        // path and objects are initialised
        pathAll = pathFolder;
        sPath = this;
        string[] folderPaths = Directory.GetDirectories(@pathAll);
        Transform child = transform.Find("Folders");
        GameObject folderObject;
        float x = -42.0f;
        float y = 200.0f;
        Text folder = child.GetComponent<Text>();

        // for each folder an object is created
        foreach (string s in folderPaths)
        {
            
            i++;
            //creates a gameobject with a recttransform to position it
            folderObject = new GameObject("MyTest" + i);
            folderObject.transform.SetParent(folder.transform);
            RectTransform trans = folderObject.AddComponent<RectTransform>();
            trans.anchoredPosition = new Vector2(x, y);
            trans.sizeDelta = new Vector2(290, 50);
            trans.localScale = new Vector3(1, 1, 1);

            //creates and adds an eventtrigger so the text is clickable
            folderObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            string leitung = s;
            entry.callback.AddListener((eventData) => { init(leitung); });
            folderObject.GetComponent<EventTrigger>().triggers.Add(entry);

            //adds a text to the gameobjects which is filled and modified 
            Text myText = folderObject.AddComponent<Text>();
            myText.color = Color.black;
            myText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            myText.text = Path.GetFileName(s);
            myText.fontSize = 30;
            y = y - 50.0f;


        }


        string[] filerPath = Directory.GetFiles(@pathAll);
        
        // for each file an object is created
        foreach (string p in filerPath)
        {

            i++;
            //creates a gameobject with a recttransform to position it
            folderObject = new GameObject("MyTest" + i);
            folderObject.transform.SetParent(folder.transform);
            RectTransform trans = folderObject.AddComponent<RectTransform>();
            trans.anchoredPosition = new Vector2(x, y);
            trans.sizeDelta = new Vector2(290, 50);
            trans.localScale = new Vector3(1, 1, 1);


            //adds a text to the gameobjects which is filled and modified 
            Text myText = folderObject.AddComponent<Text>();
            myText.color = Color.black;
            myText.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            if (Path.GetFileName(p).Length > 20)
            {
                string zS = Path.GetFileName(p);
                zS = zS.Substring(0, 17);
                zS = zS + "...";
                myText.text = zS;
            }
            else
            {
                myText.text = Path.GetFileName(p);
            }
            
            myText.fontSize = 30;
            y = y - 50.0f;


        }

        // creates back-button if necessary 
        if(pathFolder != mainpath) {

            // creates game object and places it
            GameObject back = new GameObject("Back2");
            back.transform.SetParent(folder.transform);
            RectTransform trans2 = back.AddComponent<RectTransform>();
            trans2.anchoredPosition = new Vector2(x, y);
            trans2.sizeDelta = new Vector2(290, 50);
            trans2.localScale = new Vector3(1, 1, 1);

            // adds text 
            Text myBack = back.AddComponent<Text>();
            back.AddComponent<EventTrigger>();

            // adds event trigger with recursion
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => { init(Path.GetFullPath(Path.Combine(@pathFolder, @".."))); });
            back.GetComponent<EventTrigger>().triggers.Add(entry);
            myBack.color = Color.black;
            myBack.font = Resources.GetBuiltinResource(typeof(Font), "Arial.ttf") as Font;
            myBack.text = "Back";
            myBack.fontSize = 30;

        }
  
    }

}
