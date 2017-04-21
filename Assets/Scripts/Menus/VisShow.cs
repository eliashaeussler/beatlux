using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using System.Linq;

public class VisShow : MonoBehaviour {

    // Database connection
    private SqliteConnection db;
    public List<DataObj> dataList = new List<DataObj>();
    int i = 1;
    public static int SceneId =-1;
    public List<DataObj> cloneList = new List<DataObj>();
    String vizScheme="";
	// Use this for initialization
	void Start () {

        DbConnect();

        // Select Visualisations from database
        Load();

        // Display Visualisations
        Display();

        // Close database connection
        DbClose();
	
	}

    // Searching for the specific value converting everything to lower case
    void Update()
    {
        if (MenuFunctions.viz == null)
        {
            dataList = MenuFunctions.cloneList;
            Display();
        }
        else if (MenuFunctions.viz != "")
        {

            vizScheme = MenuFunctions.viz.ToLower();

            foreach (DataObj data in MenuFunctions.cloneList)
            {
                if (data.Name.ToLower().Contains(vizScheme))
                {
                    cloneList.Add(data);
                }
            }
            dataList = cloneList;
            Display();
            MenuFunctions.viz = "";
            cloneList.Clear();
        }
        
    }

    // creating Items for VisualContent
    void Display()
    {
        float x = -200.0f;
        float y = -50.0f;
        Transform child2 = transform.Find("VisualisationContent");
        foreach (Transform chil in child2)
        {
            GameObject.Destroy(chil.gameObject);
        }
        Transform child = transform.Find("VisualisationContent");
        Text folder = child.GetComponent<Text>();
        GameObject folderObject;

        foreach(DataObj obj in dataList) 
        {
            //creates an object and places it
            folderObject = new GameObject("VisualisationS" + i);
            i++;
            folderObject.transform.SetParent(folder.transform);
            RectTransform trans = folderObject.AddComponent<RectTransform>();
            trans.anchoredPosition = new Vector2(x, y);
            trans.sizeDelta = new Vector2(290, 50);
            trans.localScale = new Vector3(1, 1, 1);

            

            //adds a text to the gameobjects which is filled and modified 
            Text myText = folderObject.AddComponent<Text>();
            myText.color = Color.gray;
            myText.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
            myText.text = obj.Name;
            myText.fontSize = 30;
            y = y - 50.0f;

            //creates and adds an eventtrigger so the text is clickable and editing different color elements
            folderObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            EventTrigger.Entry entry1 = new EventTrigger.Entry();
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry1.eventID = EventTriggerType.PointerEnter;
            entry2.eventID = EventTriggerType.PointerExit;
            int number = obj.BuildNr;
            entry.callback.AddListener((eventData) => { changeScheme(number); });
            entry1.callback.AddListener((eventData) => { myText.color = Color.black; });
            entry2.callback.AddListener((eventData) => { myText.color = Color.gray; });
            folderObject.GetComponent<EventTrigger>().triggers.Add(entry);
            folderObject.GetComponent<EventTrigger>().triggers.Add(entry1);
            folderObject.GetComponent<EventTrigger>().triggers.Add(entry2);
            }
    }

    void Load()
    {
        if (DbConnect())
        {
            // Database command
            SqliteCommand cmd = new SqliteCommand(db);

            // Query statement
            string sql = "SELECT id,name,buildNumber FROM visualization ORDER BY name ASC";
            cmd.CommandText = sql;

            // Get sql results
            SqliteDataReader reader = cmd.ExecuteReader();

            // Read sql results
            while (reader.Read())
            {
                // Create playlist object
                DataObj obj = new DataObj();

                // Set id and name
                obj.Id = reader.GetInt32(0);
                obj.Name = reader.GetString(1);
                obj.BuildNr = reader.GetInt32(2);

                dataList.Add(obj);
            }

            // Close reader
            reader.Close();
            cmd.Dispose();

            // Close database connection
            DbClose();
            MenuFunctions.cloneList = dataList;
        }
    }

    //-- DATABASE CONNECTION

    bool DbConnect()
    {
        if (db == null)
        {
            db = Database.GetConnection();
        }

        return db != null;
    }

    void DbClose()
    {
        // Close database
        Database.Close();

        // Reset database instance
        db = null;
    }

    // changes SceneId 
    void changeScheme(int n)
    {
        SceneId = n;

    }
    
}
