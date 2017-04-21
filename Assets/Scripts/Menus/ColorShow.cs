using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;

public class ColorShow : MonoBehaviour
{
    // Database connection
    private SqliteConnection db;

    // Playlists
    public List<VisualizationObj> colorList = new List<VisualizationObj>();

    private HashSet<int> hash = new HashSet<int>();

    public static int VizId=0;

    GameObject ColorPicker;

    MenuFunctions men = new MenuFunctions();

    int Null;

    int Laenge;

    private bool check = true;

    GameObject imag;
    Color col = new Color();

    GameObject folderObject;

    // Use this for initialization
    void Start()
    {
        ColorPicker = GameObject.Find("ColorPicker");
        ColorPicker.SetActive(false);

        imag = GameObject.Find("BackgroundImg");
        imag.SetActive(false);
        

        
    }

    void Update()
    {

        if (VizId != VisShow.SceneId && VisShow.SceneId != -1)
        {
            

            DbConnect();

            // sets viz_id to selected viz_id and deletes/clears all objects 
            VizId = VisShow.SceneId;
            colorList.Clear();
            Transform transf = transform.Find("ColorContent");
            

            foreach (Transform child in transf)
            {
                GameObject.Destroy(child.gameObject);
            }

            // Select Visualisations from database
            Load(VizId);

            // Display Visualisations
            Display();

            // Close database connection
            DbClose();

            VisShow.SceneId = -1;

        }

        if (MenuFunctions.color == col)
        {
        }

        else
        {
            GameObject objT = GameObject.Find("Image" + MenuFunctions.num2);
            Image imge = objT.GetComponent<Image>();
            imge.color = MenuFunctions.color;
            MenuFunctions.color = col;
            hash.Add(MenuFunctions.num2);
        }
            
        if (MenuFunctions.stri != "")
        {
            colorList.Clear();
            Transform transf = transform.Find("ColorContent");

            foreach (Transform child in transf)
            {
                GameObject.Destroy(child.gameObject);
            }

            DbConnect();
            if (DbConnect())
            {
                SqliteCommand cmd7 = new SqliteCommand(db);

                // Query statement
                string sql = "SELECT colors FROM color_scheme WHERE viz_id = @param1";
                cmd7.CommandText = sql;
                cmd7.Parameters.Add(new SqliteParameter("@param1",MenuFunctions.num));

                // Get sql results
                SqliteDataReader reader = cmd7.ExecuteReader();
                string value=null;
                // Read sql results
                while (reader.Read())
                {
                    value = reader.GetString(0);
                    

                }
                string[] colorSplit = value.Split(';');
                Laenge = colorSplit.Length;

                // Close reader
                reader.Close();
                cmd7.Dispose();


                
                string sql2 = "INSERT INTO color_scheme (name,viz_id,colors) VALUES (@param2,@param3,@param4)";
                string data="";
                for(int i =0; i<Laenge; i++){
                    data += "255,255,255;";
                }
                data = data.Remove(data.Length - 1);


                SqliteCommand cmd2 = new SqliteCommand(sql2,db);
                cmd2.Parameters.Add(new SqliteParameter("@param2",MenuFunctions.stri));
                cmd2.Parameters.Add(new SqliteParameter("@param3",MenuFunctions.num));
                cmd2.Parameters.Add(new SqliteParameter("@param4",data));
                cmd2.ExecuteNonQuery();
                
                
                cmd2.Dispose();

                // Close database connection
                DbClose();
                
            }
            

            Load(VizId);

            // Display Visualisations
            Display();

            // Close database connection
            DbClose();

            MenuFunctions.stri = "";
        } 

        
        

        
         
    }

    void Display()
    {
        int i = 1;
        float x = 135.0f;
        float y = 0.0f;
        Transform child = transform.Find("ColorContent");
        Text folder = child.GetComponent<Text>();
        

        foreach (VisualizationObj obj in colorList)
        {
            bool active = false;
            String obName = obj.Name;
            int obId = obj.Id;


            //creates an object and places it
            folderObject = new GameObject("Visualisation" + i);

            folderObject.transform.SetParent(folder.transform); 
            RectTransform trans1 = folderObject.AddComponent<RectTransform>();
            trans1.anchoredPosition = new Vector2(x, y);
            trans1.sizeDelta = new Vector2(770, 30);
            trans1.localScale = new Vector3(1, 1, 1);
 
            
            
            
            int w = i;
            string name = obj.Name;
            GameObject betObject = new GameObject("ImgList" + i);
            betObject.transform.SetParent(folderObject.transform);
            RectTransform trans10 = betObject.AddComponent<RectTransform>();
            trans10.anchoredPosition = new Vector2(-300, -40);
            trans10.sizeDelta = new Vector2(50, 20);
            trans10.localScale = new Vector3(1, 1, 1);
            int count = 0;
            for (int t = 0; t < obj.ColorSchemes.Length; t++ )
            {
                // Add Image
                int sum = w*10 + t;
                GameObject imgObject = new GameObject("Image" +sum);
                imgObject.transform.SetParent(betObject.transform);
                RectTransform trans2 = imgObject.AddComponent<RectTransform>();
                trans2.anchoredPosition = new Vector2(-50+(70*t), 0);
                trans2.sizeDelta = new Vector2(50, 20);
                trans2.localScale = new Vector3(1, 1, 1);
                Image img = imgObject.AddComponent<Image>();
                img.color = obj.ColorSchemes[t];
                imgObject.AddComponent<EventTrigger>();
                EventTrigger.Entry entry2 = new EventTrigger.Entry();
                entry2.eventID = EventTriggerType.PointerDown;
                entry2.callback.AddListener((eventData) =>{ ColorChange(sum); });
                imgObject.GetComponent<EventTrigger>().triggers.Add(entry2);
                count = count + 30;
                
            }

            betObject.SetActive(false);

            // Add Img
            GameObject arrowObject = new GameObject("Arrow" + i);
            arrowObject.transform.SetParent(folderObject.transform);
            RectTransform trans6 = arrowObject.AddComponent<RectTransform>();
            trans6.anchoredPosition = new Vector2(-400, 0);
            trans6.sizeDelta = new Vector2(30, 30);
            trans6.localScale = new Vector3(1, 1, 1);
            Image arrowImg = arrowObject.AddComponent<Image>();
            arrowImg.sprite = Resources.Load<Sprite>("Images/arrow-right");
            //creates and adds an eventtrigger so the text is clickable
            GameObject tempObj = arrowObject;
            GameObject temp2Obj = betObject;
            arrowObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener((eventData) => { active = ToggleArrow(tempObj, betObject, active); });
            arrowObject.GetComponent<EventTrigger>().triggers.Add(entry);

            // Add Save
            GameObject saveObject = new GameObject("Save" + i);
            saveObject.transform.SetParent(folderObject.transform);
            RectTransform trans3 = saveObject.AddComponent<RectTransform>();
            trans3.anchoredPosition = new Vector2(170,0);
            trans3.sizeDelta = new Vector2(120, 50);
            trans3.localScale = new Vector3(1, 1, 1);
            Text save = saveObject.AddComponent<Text>();
            save.text = "Speichern";
            save.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
            save.color = Color.black;
            save.fontSize = 30;
            saveObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry3 = new EventTrigger.Entry();
            entry3.eventID = EventTriggerType.PointerDown;
            entry3.callback.AddListener((eventData) => {
                if (EditorUtility.DisplayDialog("Playlist löschen", "Soll das Farbschema \"" + name + "\" wirklich gespeichert werden?", "Ja", "Nein"))
                {
                    Save(w);
                } 
            });
            saveObject.GetComponent<EventTrigger>().triggers.Add(entry3);


            // Add Delete
            GameObject deleteObject = new GameObject("Delete" + i);
            deleteObject.transform.SetParent(folderObject.transform);
            RectTransform trans4 = deleteObject.AddComponent<RectTransform>();
            trans4.anchoredPosition = new Vector2(300, 0);
            trans4.sizeDelta = new Vector2(110, 50);
            trans4.localScale = new Vector3(1, 1, 1);
            Text delete = deleteObject.AddComponent<Text>();
            delete.text = "Löschen";
            delete.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
            delete.color = Color.black;
            delete.fontSize = 30;
            deleteObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry4 = new EventTrigger.Entry();
            entry4.eventID = EventTriggerType.PointerDown;
            entry4.callback.AddListener((eventData) => {
                if (EditorUtility.DisplayDialog("Farbschema löschen", "Soll das Farbschema \"" + name + "\" wirklich gelöscht werden?", "Ja", "Nein"))
                {
                    CannotDelete(w);
                    if (check == true)
                    {
                        Delete(w);
                    }
                    check = true;
                    
                }
            });
            deleteObject.GetComponent<EventTrigger>().triggers.Add(entry4);


            // Add New 
            GameObject newObject = new GameObject("New"+i);
            newObject.transform.SetParent(folderObject.transform);
            RectTransform trans5 = newObject.AddComponent<RectTransform>();
            trans5.anchoredPosition = new Vector2(400, 0);
            trans5.sizeDelta = new Vector2(90, 50);
            trans5.localScale = new Vector3(1, 1, 1);
            Text newText = newObject.AddComponent<Text>();
            newText.text = "Neues";
            newText.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
            newText.color = Color.black;
            newText.fontSize = 30;
            newObject.AddComponent<EventTrigger>();
            EventTrigger.Entry entry5 = new EventTrigger.Entry();
            entry5.eventID = EventTriggerType.PointerDown;
            int num = obj.VizId;
            entry5.callback.AddListener((eventData) => {CreateNew(num);});
            newObject.GetComponent<EventTrigger>().triggers.Add(entry5);

            

            //adds a text to the gameobjects which is filled and modified 
            Text myText = folderObject.AddComponent<Text>();
            myText.color = Color.black;
            myText.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
            myText.text = obj.Name;
            myText.fontSize = 30;
            myText.alignment = TextAnchor.MiddleLeft;

            y = y - 70.0f;


            i++;

            
        }
    }

    void Load(int vId)
    {
        if (DbConnect())
        {
            int id = vId;
            // Database command
            SqliteCommand cmd = new SqliteCommand(db);

            // Query statement
            string sql = "SELECT id,name,viz_id,colors FROM color_scheme WHERE viz_id = @param1";
            cmd.CommandText = sql;
            cmd.Parameters.Add(new SqliteParameter("@param1",id));

            // Get sql results
            SqliteDataReader reader = cmd.ExecuteReader();

            // Read sql results
            while (reader.Read())
            {
                // Create playlist object
                VisualizationObj obj = new VisualizationObj();

                // Set id and name
                obj.Id = reader.GetInt32(0);
                obj.Name = reader.GetString(1);
                obj.VizId = reader.GetInt32(2);
                string value = reader.GetString(3);
                string[] colorSplit = value.Split(';');
                int counter = 0;
                obj.ColorSchemes = new Color[colorSplit.Length];
                foreach(string s in colorSplit)
                {
                    string[] rgbSplit = s.Split(',');
                   
                    obj.ColorSchemes[counter].r = float.Parse(rgbSplit[0]);
                    obj.ColorSchemes[counter].g = float.Parse(rgbSplit[1]);
                    obj.ColorSchemes[counter].b = float.Parse(rgbSplit[2]);
                    obj.ColorSchemes[counter].a = 1;
              
                    counter++;
                    
                }

                colorList.Add(obj);
                
            }


            // Close reader
            reader.Close();
            cmd.Dispose();

            // Close database connection
            DbClose();
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

    void ColorChange(int number)
    {
        ColorPicker.SetActive(true);
        MenuFunctions.num2 = number;
    }

    void Save(int changedColor)
    {

        foreach (int p in hash)
        {

            int i = Math.Abs(p);
            while (i >= 10)
            {
                i /= 10;
            }
            if (i == changedColor)
            {
                colorList[changedColor-1].ColorSchemes[p-(i*10)] = GameObject.Find("Image" + p).GetComponent<Image>().color;
                
            }
            
        }
        String combine = "";
        for(int count =0; count <colorList[changedColor-1].ColorSchemes.Length; count++)
        {
            combine += colorList[changedColor-1].ColorSchemes[count].r + "," + colorList[changedColor-1].ColorSchemes[count].g + "," + colorList[changedColor-1].ColorSchemes[count].b + ";";
        }
        string s = combine.Remove(combine.Length - 1);

        if (DbConnect())
        {
            
            // Query statement
            string sql = "UPDATE color_scheme SET colors = @param1 WHERE id = @param2";


            SqliteCommand cmd2 = new SqliteCommand(sql, db);
            cmd2.Parameters.Add(new SqliteParameter("@param1",s));
            cmd2.Parameters.Add(new SqliteParameter("@param2",colorList[changedColor - 1].Id));

            // Result
            cmd2.ExecuteNonQuery();
            // Close database connection
            cmd2.Dispose();
            DbClose();
        }
    }

    void Delete(int w)
    {
        if (DbConnect())
        {
            // Query statement
            string sql2 = "DELETE FROM color_scheme WHERE id = @param1";


            SqliteCommand cmd3 = new SqliteCommand(sql2, db);
            cmd3.Parameters.Add(new SqliteParameter("@param1",colorList[w - 1].Id));

            // Result
            cmd3.ExecuteNonQuery();
            // Close database connection
            cmd3.Dispose();
            DbClose();
        }
        Destroy(GameObject.Find("Visualisation" + w));
    }

    void CannotDelete(int p)
    {
        if (DbConnect())
        {
            // Database command
            SqliteCommand cmd5 = new SqliteCommand(db);

            // Query statement
            string sql3 = "SELECT viz_id FROM color_scheme WHERE viz_id = @param1";
            cmd5.CommandText = sql3;
            cmd5.Parameters.Add(new SqliteParameter("@param1",colorList[p - 1].VizId));

            // Get sql results
            SqliteDataReader reader2 = cmd5.ExecuteReader();

            // Read sql results
            int count = 0;
            while (reader2.Read())
            {

                int s = reader2.GetInt32(0);
                count++;

            }
            if (count <= 1)
            {
                if (EditorUtility.DisplayDialog("Letztes Farbschema", "Das Farbschema \"" + colorList[p-1].Name + "\" kann nicht gelöscht werden, da es das letzte Schema ist!", "OK"))
                {
                    check = false;
                }
            }

            // Close reader
            reader2.Close();
            cmd5.Dispose();

            // Close database connection
            DbClose();
        }
  

    }
    

    // call the input field
    void CreateNew(int w)
    {
        imag.SetActive(true);
        MenuFunctions.num = w;

    }
    GameObject gObj;
    bool ToggleArrow(GameObject obj,GameObject obj2,bool active)
    {
        active = !active;
        if (active)
        {
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/arrow-down");
            obj2.SetActive(true);
            
        }
        else
        {
            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/arrow-right");
            obj2.SetActive(false);
        }
        return active;
    }

   
}
