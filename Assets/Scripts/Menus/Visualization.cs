using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.SceneManagement;

public class Visualization : MonoBehaviour {

	// Color schemes
    public List<VisualizationObj> Visualizations = new List<VisualizationObj> ();

	// Color scheme object
	public ColorScheme ColorSchemes;



	void Start ()
	{
		// Select visualizations from database
		Load ();

		// Set opened visualization
		if (Settings.Selected.Visualization == null && Settings.Active.Visualization != null) {
			Settings.Selected.Visualization = Settings.Active.Visualization;
		}

		// Display visualizations
		Display ();

		// Display color schemes
		ColorSchemes.Display ();

        // Close database connection
        Database.Close ();
	
	}



	public void Display ()
	{
		if (Visualizations != null)
		{
			// Remove all GameObjects
			for (int i = transform.childCount - 1; i >= 0; i--) {
				GameObject.DestroyImmediate (transform.GetChild (i).gameObject);
			}

			foreach (VisualizationObj viz in Visualizations)
			{
				// Create GameObject
				GameObject gameObject = new GameObject ("#" + viz.ID);
				gameObject.transform.SetParent (transform);

				RectTransform trans = gameObject.AddComponent<RectTransform> ();
				trans.pivot = new Vector2 (0, 0.5f);

				// Add Layout Element
				LayoutElement layoutElement = gameObject.AddComponent<LayoutElement> ();
				layoutElement.minHeight = 30;
				layoutElement.preferredHeight = 30;

				// Add Horizontal Layout Group
				HorizontalLayoutGroup hlg = gameObject.AddComponent<HorizontalLayoutGroup> ();
				hlg.childForceExpandWidth = false;
				hlg.childForceExpandHeight = false;
				hlg.spacing = 10;
				hlg.childAlignment = TextAnchor.MiddleLeft;


				// Create arrow text GameObject
				GameObject mainArrow = new GameObject ("Arrow");
				mainArrow.transform.SetParent (gameObject.transform);

				// Add text
				TextUnicode mainTextArrow = mainArrow.AddComponent<TextUnicode> ();
				mainTextArrow.text = viz.Equals (Settings.Selected.Visualization) ? IconFont.DROPDOWN_CLOSED : "";

				// Set text alignment
				mainTextArrow.alignment = TextAnchor.MiddleLeft;

				// Font settings
				mainTextArrow.font = IconFont.font;
				mainTextArrow.fontSize = 20;

				// Add Layout Element
				LayoutElement mainLayoutElementArrow = mainArrow.AddComponent<LayoutElement> ();
				mainLayoutElementArrow.minWidth = 22;


				// Create Text GameObject
				GameObject mainText = new GameObject ("Text");
				mainText.transform.SetParent (gameObject.transform);

				// Add Text
				Text text = mainText.AddComponent<Text> ();
				text.color = Color.white;
				text.font = Resources.Load<Font> ("Fonts/FuturaStd-Book");
				text.text = viz.Name;
				text.fontSize = 30;

				// Add Button
				Button button = mainText.AddComponent<Button> ();
				button.transition = Selectable.Transition.Animation;

				// Add OnClick Handler
				VisualizationObj currentViz = viz;
				button.onClick.AddListener (delegate {

					Settings.Selected.Visualization = currentViz;
					Settings.Selected.ColorScheme = null;
					ColorSchemes.Display ();
					Display ();

				});

				// Add Animator
				Animator animator = mainText.AddComponent<Animator> ();
				animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");


				// Create active text GameObject
				GameObject mainActive = new GameObject ("Active");
				mainActive.transform.SetParent (gameObject.transform);

				// Add text
				TextUnicode mainTextActive = mainActive.AddComponent<TextUnicode> ();

				if (viz.Equals (Settings.Active.Visualization))
				{
					mainTextActive.text = IconFont.VISUALIZATION;
					mainTextActive.fontSize = 30;
					mainTextActive.color = new Color (0.7f, 0.7f, 0.7f);
				}

				// Set text alignment
				mainTextActive.alignment = TextAnchor.MiddleRight;

				// Font settings
				mainTextActive.font = IconFont.font;

				// Add Layout Element
				LayoutElement mainLayoutElementActive = mainActive.AddComponent<LayoutElement> ();
				mainLayoutElementActive.preferredWidth = 40;
				mainLayoutElementActive.preferredHeight = 30;



				// Set scaling
				gameObject.GetComponent<RectTransform> ().localScale = Vector3.one;
			}
		}
	}



	//-- DATABASE METHODS

	public void Load ()
    {
        if (Database.Connect ())
        {
            // Database command
			SqliteCommand cmd = new SqliteCommand (Database.Connection);

            // Query statement
            string sql = "SELECT id,name,colors,buildNumber,skybox FROM visualization ORDER BY name ASC";
            cmd.CommandText = sql;

            // Get sql results
            SqliteDataReader reader = cmd.ExecuteReader ();

            // Read sql results
            while (reader.Read ())
            {
                // Create visualization object
				VisualizationObj obj = new VisualizationObj ();

                // Set id and name
                obj.ID = reader.GetInt64 (0);
                obj.Name = reader.GetString (1);
				obj.Colors = reader.GetInt32 (2);
                obj.BuildNumber = reader.GetInt32 (3);
				obj.Skybox = reader.IsDBNull (4) ? null : reader.GetString (4);

				// Add visualization to visualizations array
				if (Application.CanStreamedLevelBeLoaded (obj.BuildNumber))
                	Visualizations.Add (obj);
            }

            // Close reader
            reader.Close ();
            cmd.Dispose ();

			// Clone visualizations array
			MenuFunctions.tempViz = Visualizations;
		}

		// Close database connection
		Database.Close ();
    }

	public static VisualizationObj GetVisualization (long id, bool closeConnection)
	{
		if (Database.Connect ())
		{
			// Send database query
			SqliteCommand cmd = new SqliteCommand (Database.Connection);
			cmd.CommandText = "SELECT id,name,colors,buildNumber,skybox FROM visualization WHERE id = @ID";

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("ID", id));

			SqliteDataReader reader = cmd.ExecuteReader ();
			VisualizationObj viz = null;

			// Read and add visualization
			while (reader.Read ())
			{
				viz = new VisualizationObj ();

				viz.ID = reader.GetInt64 (0);
				viz.Name = reader.GetString (1);
				viz.Colors = reader.GetInt32 (2);
				viz.BuildNumber = reader.GetInt32 (3);
				viz.Skybox = reader.IsDBNull (4) ? null : reader.GetString (4);
			}

			// Close reader
			reader.Close ();
			cmd.Dispose ();
			if (closeConnection) Database.Close ();

			return viz;
		}

		// Close database connection
		if (closeConnection) Database.Close ();

		return null;
	}
    
}
