using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColorScheme : MonoBehaviour {
	
	// Dialog reference
	public Dialog Dialog;

    // Color schemes
	public List<ColorSchemeObj> ColorSchemes;

	// Color Picker GameObject
	public GameObject ColorPicker;

	// Color Picker Script
	public ColorPicker PickerObj;

	// Current selected color
	public static GameObject ActiveColor;
	public static int ActiveColorID;



	void Start ()
	{
		// Add listener for Color Picker
		PickerObj.onValueChanged.AddListener (col => {
			UpdateColor (Settings.OpenedColorScheme, ActiveColorID, col);
		});
	}



	public void Display ()
	{
		if (Database.Connect ())
		{
			// Load color schemes from database
			Load ();

			// Remove all GameObjects
			for (int i = transform.childCount - 1; i >= 0; i--) {
				GameObject.DestroyImmediate (transform.GetChild (i).gameObject);
			}

			if (ColorSchemes != null && ColorSchemes.Count > 0)
			{
				// Set opened color scheme
				if (Settings.OpenedColorScheme == null && Settings.ActiveColorScheme != null) {
					Settings.OpenedColorScheme = Settings.ActiveColorScheme;
				}

				foreach (ColorSchemeObj cs in ColorSchemes)
				{
					// Display color scheme
					DisplayColorScheme (cs);

					// Display colors
					for (int i=0; i < cs.Colors.Length; i++) {
						DisplayColor (cs, cs.Colors[i], i);
					}

					// Hide colors
					if (transform.Find ("#" + cs.ID + "/Contents") != null && !cs.Equals (Settings.OpenedColorScheme))
						transform.Find ("#" + cs.ID + "/Contents").gameObject.SetActive (false);
				}
			}
		}

		// Close database connection
		Database.Close ();
	}

	public void DisplayColorScheme (ColorSchemeObj colorScheme)
	{
		if (colorScheme != null)
		{
			string name = "#" + colorScheme.ID;

			// Create GameOject
			GameObject gameObject = new GameObject (name);
			gameObject.transform.SetParent (transform);


			// Create main GameObject
			GameObject main = new GameObject ("Main");
			main.transform.SetParent (gameObject.transform);

			// Add Vertical Layout Group
			VerticalLayoutGroup vlg = gameObject.AddComponent<VerticalLayoutGroup> ();
			vlg.spacing = 20;
			vlg.childForceExpandWidth = true;
			vlg.childForceExpandHeight = false;


			// Add Layout Element to GameObject
			LayoutElement mainLayout = main.AddComponent<LayoutElement> ();
			mainLayout.minHeight = 30;
			mainLayout.preferredHeight = mainLayout.minHeight;

			// Add image to GameObject
			Image mainImg = main.AddComponent<Image> ();
			mainImg.color = Color.clear;

			// Set transformations
			mainImg.rectTransform.pivot = new Vector2 (0, 0.5f);

			// Add Horizontal Layout Group
			HorizontalLayoutGroup mainHlg = main.AddComponent<HorizontalLayoutGroup> ();
			mainHlg.spacing = 10;
			mainHlg.childForceExpandWidth = false;
			mainHlg.childForceExpandHeight = false;
			mainHlg.childAlignment = TextAnchor.MiddleLeft;

			// Set padding right of Horizontal Layout Group
			mainHlg.padding = new RectOffset (0, 65, 0, 0);


			// Create arrow text GameObject
			GameObject mainArrow = new GameObject ("Arrow");
			mainArrow.transform.SetParent (main.transform);

			// Add text
			TextUnicode mainTextArrow = mainArrow.AddComponent<TextUnicode> ();
			mainTextArrow.text = colorScheme.Equals (Settings.OpenedColorScheme)
				? IconFont.DROPDOWN_OPENED
				: IconFont.DROPDOWN_CLOSED;

			// Set text alignment
			mainTextArrow.alignment = TextAnchor.MiddleLeft;

			// Font settings
			mainTextArrow.font = IconFont.font;
			mainTextArrow.fontSize = 20;

			// Add Layout Element
			LayoutElement mainLayoutElementArrow = mainArrow.AddComponent<LayoutElement> ();
			mainLayoutElementArrow.minWidth = 22;


			// Create text GameObject
			GameObject mainText = new GameObject ("Text");
			mainText.transform.SetParent (main.transform);

			// Add text
			Text text = mainText.AddComponent<Text> ();
			text.text = colorScheme.Name;

			// Set text alignment
			text.alignment = TextAnchor.MiddleLeft;

			// Set text color
			text.color = Color.white;

			// Font settings
			text.font = Resources.Load<Font> ("Fonts/FuturaStd-Book");
			text.fontSize = 30;

			// Set transformations
			text.rectTransform.pivot = new Vector2 (0.5f, 0.5f);

			// Add button
			Button buttonText = mainText.AddComponent<Button> ();
			buttonText.transition = Selectable.Transition.Animation;

			// Add Event to Button
			buttonText.onClick.AddListener (delegate {
				ToggleColors (colorScheme);
			});

			// Add animator
			Animator animatorText = mainText.AddComponent<Animator> ();
			animatorText.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");


			// Create active text GameObject
			GameObject mainActive = new GameObject ("Active");
			mainActive.transform.SetParent (main.transform);

			// Add text
			TextUnicode mainTextActive = mainActive.AddComponent<TextUnicode> ();

			if (colorScheme.Equals (Settings.ActiveColorScheme))
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
			LayoutElement mainLayoutElementListening = mainActive.AddComponent<LayoutElement> ();
			mainLayoutElementListening.preferredWidth = 40;


			// Create edit icons GameObject
			GameObject editIcons = new GameObject ("Images");
			editIcons.transform.SetParent (main.transform);

			// Set transformations
			RectTransform editIconsTrans = editIcons.AddComponent<RectTransform> ();
			editIconsTrans.anchoredPosition = Vector2.zero;
			editIconsTrans.anchorMin = new Vector2 (1, 0.5f);
			editIconsTrans.anchorMax = new Vector2 (1, 0.5f);
			editIconsTrans.pivot = new Vector2 (1, 0.5f);

			// Add Layout Element
			LayoutElement editIconslayoutElement = editIcons.AddComponent<LayoutElement> ();
			editIconslayoutElement.ignoreLayout = true;

			// Add Content Size Fitter
			ContentSizeFitter editIconsCsf = editIcons.AddComponent<ContentSizeFitter> ();
			editIconsCsf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			editIconsCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

			// Add Layout Group
			HorizontalLayoutGroup editIconsHlgImg = editIcons.AddComponent<HorizontalLayoutGroup> ();
			editIconsHlgImg.childAlignment = TextAnchor.MiddleRight;
			editIconsHlgImg.spacing = 5;
			editIconsHlgImg.childForceExpandWidth = false;
			editIconsHlgImg.childForceExpandHeight = false;

			// Disable edit icons GameObject
			editIcons.SetActive (false);


			// Create edit text GameObject
			GameObject edit = new GameObject ("Edit");
			edit.transform.SetParent (editIcons.transform);

			// Add text
			TextUnicode editText = edit.AddComponent<TextUnicode> ();
			editText.text = IconFont.EDIT;

			// Set text alignment
			editText.alignment = TextAnchor.MiddleRight;

			// Set transformations
			editText.rectTransform.sizeDelta = new Vector2 (20, 30);

			// Font settings
			editText.font = IconFont.font;
			editText.fontSize = 30;

			// Add button
			Button buttonEditEvt = edit.AddComponent<Button> ();
			buttonEditEvt.transition = Selectable.Transition.Animation;

			// Add button onclick event
			buttonEditEvt.onClick.AddListener (delegate {
				ShowDialog ("CS_EDIT", gameObject);
			});

			// Add animator
			Animator animatorEditEvt = edit.AddComponent<Animator> ();
			animatorEditEvt.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");


			// Create delete text GameObject
			GameObject delete = new GameObject ("Delete");
			delete.transform.SetParent (editIcons.transform);

			// Add text
			Text deleteText = delete.AddComponent<Text> ();
			deleteText.text = IconFont.TRASH;

			// Set text alignment
			deleteText.alignment = TextAnchor.MiddleRight;

			// Set transformations
			deleteText.rectTransform.sizeDelta = new Vector2 (20, 30);

			// Font settings
			deleteText.font = IconFont.font;
			deleteText.fontSize = 30;

			// Add button
			Button buttonDeleteEvt = delete.AddComponent<Button> ();
			buttonDeleteEvt.transition = Selectable.Transition.Animation;

			// Add button onclick event
			buttonDeleteEvt.onClick.AddListener (delegate {
				ShowDialog ("CS_DEL", gameObject);
			});

			// Add animator
			Animator animatorDeleteEvt = delete.AddComponent<Animator> ();
			animatorDeleteEvt.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController> ("Animations/MenuButtons");


			// Create GameObject Event Triggers
			EventTrigger evtWrapper = gameObject.AddComponent<EventTrigger> ();

			// Add Hover Enter Event
			EventTrigger.Entry evtHover = new EventTrigger.Entry ();
			evtHover.eventID = EventTriggerType.PointerEnter;
			evtWrapper.triggers.Add (evtHover);

			evtHover.callback.AddListener ((eventData) => {
				editIcons.SetActive (true);
			});

			// Add Hover Exit Event
			EventTrigger.Entry evtExit = new EventTrigger.Entry ();
			evtExit.eventID = EventTriggerType.PointerExit;
			evtWrapper.triggers.Add (evtExit);

			evtExit.callback.AddListener ((eventData) => {
				editIcons.SetActive (false);
			});


			// Add Event Trigger
			EventTrigger evtMain = main.AddComponent<EventTrigger> ();

			// Add Click Event
			EventTrigger.Entry evtClick = new EventTrigger.Entry ();
			evtClick.eventID = EventTriggerType.PointerClick;
			evtMain.triggers.Add (evtClick);

			evtClick.callback.AddListener ((eventData) => {
				ToggleColors (colorScheme);
			});


			// Add Contents GameObject
			GameObject contents = new GameObject ("Contents");
			contents.transform.SetParent (gameObject.transform);

			// Add Grid Layout Group
			GridLayoutGroup glg = contents.AddComponent<GridLayoutGroup> ();

			// Set Grid Layout
			glg.cellSize = new Vector2 (58.5f, 58.5f);
			glg.spacing = new Vector2 (25, 25);
			glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			glg.constraintCount = 10;
		}
	}

	public void DisplayColor (ColorSchemeObj colorScheme, Color color, int id)
	{
		if (colorScheme != null)
		{
			// Get Contents GameObject
			GameObject contents = transform.Find ("#" + colorScheme.ID + "/Contents").gameObject;

			// Create Image GameObject
			GameObject image = new GameObject ("#" + colorScheme.ID + "." + id);
			image.transform.SetParent (contents.transform);

			// Add image
			Image img = image.AddComponent<Image> ();
			img.color = color;

			// Add Event Trigger
			EventTrigger trigger = image.AddComponent<EventTrigger> ();

			// Add Click Event
			EventTrigger.Entry evtClick = new EventTrigger.Entry ();
			evtClick.eventID = EventTriggerType.PointerClick;
			trigger.triggers.Add (evtClick);

			evtClick.callback.AddListener ((eventData) => {
				
				// Set active color
				ActiveColor = image;
				ActiveColorID = id;

				// Set current color
				PickerObj.CurrentColor = colorScheme.Colors [id];

				// Show color picker
				ColorPicker.SetActive (true);

			});
		}
	}

	public void ToggleColors (ColorSchemeObj colorScheme)
	{
		ToggleColors (colorScheme, false);
	}

	public void ToggleColors (ColorSchemeObj colorScheme, bool forceOpen)
	{
		// Set color scheme as active color scheme
		if (!forceOpen) Settings.OpenedColorScheme = colorScheme;

		// Show or hide colors
		bool opened = false;
		foreach (ColorSchemeObj cs in ColorSchemes)
		{
			// Get GameObject for current color
			Transform contents = transform.Find ("#" + cs.ID + "/Contents");
			GameObject main = contents != null ? contents.gameObject : null;

			// Get arrow
			Text arr = transform.Find ("#" + cs.ID + "/Main/Arrow").GetComponent<Text>();

			// Toggle colors for GameObject
			if (main != null)
			{
				if (cs == colorScheme)
				{
					main.SetActive (!forceOpen ? !main.activeSelf : true);
					opened = main.activeSelf;
				}
				else
				{
					main.SetActive (false);

					if (arr.text == IconFont.DROPDOWN_OPENED) {
						arr.text = IconFont.DROPDOWN_CLOSED;
					}
				}
			}

			// Change arrows
			if (!forceOpen && cs != colorScheme) {
				arr.text = IconFont.DROPDOWN_CLOSED;
			}
		}

		// Change arrow image
		Text arrow = transform.Find ("#" + colorScheme.ID + "/Main/Arrow").GetComponent<Text> ();
		if (!forceOpen && arrow != null) {
			arrow.text = opened ? IconFont.DROPDOWN_OPENED : IconFont.DROPDOWN_CLOSED;
		}

		// Set opened and selected color scheme
		Settings.OpenedColorScheme = opened ? colorScheme : null;

		// Scroll to top if scrollbar is hidden
		ScrollToTop ();
	}

	public void ScrollToTop ()
	{
		// TODO funktioniert nicht nicht
		if (!gameObject.transform.parent.Find ("Scrollbar").gameObject.activeSelf)
			gameObject.transform.parent.gameObject.GetComponent<ScrollRect> ().verticalScrollbar.value = 1;
	}

	public long NewColorScheme (string name)
	{
		if (name.Length > 0)
		{
			// Create color scheme object
			ColorSchemeObj colorScheme = new ColorSchemeObj (name, Settings.OpenedVisualization);

			// Create object in database
			long id = Create (colorScheme);

			// Reload playlists
			Load ();
			Display ();

			return id;
		}

		return (long) Database.Constants.QueryFailed;
	}

	public bool Delete (GameObject gameObject, ColorSchemeObj colorScheme)
	{
		if (Delete (colorScheme))
		{
			// Remove from interface
			DestroyImmediate (gameObject);

			// Remove from list
			ColorSchemes.Remove (colorScheme);

			// Unset active and opened color scheme
			if (colorScheme.Equals (Settings.ActiveColorScheme)) Settings.ActiveColorScheme = null;
			if (colorScheme.Equals (Settings.OpenedColorScheme)) Settings.OpenedColorScheme = null;

			return true;
		}

		return false;
	}

	public ColorSchemeObj FindColorScheme (GameObject gameObject)
	{
		if (gameObject != null)
		{
			// Get color scheme id
			string[] name = gameObject.name.Split ('.');
			long csID = Int64.Parse (name [0].Split ('#') [1]);

			// Get color scheme
			return ColorSchemes.Find (x => x.ID == csID);
		}

		return null;
	}

	public void UpdateColor (ColorSchemeObj colorScheme, int id, Color color)
	{
		if (colorScheme != null && colorScheme.Colors.Length >= id + 1)
		{
			// Set color
			colorScheme.Colors [id] = color;

			// Change image
			GameObject.Find ("#" + colorScheme.ID + "." + id).GetComponent<Image> ().color = color;

			// Edit color scheme in database
			Edit (colorScheme);
		}
	}

	public void ShowDialog (string type)
	{
		ShowDialog (type, null);
	}

	public void ShowDialog (string type, GameObject obj)
	{
		if (Dialog != null)
		{
			// Color scheme object
			ColorSchemeObj colorScheme = FindColorScheme (obj);

			// Content elements
			Transform header = Dialog.wrapper.transform.Find ("Header");
			Transform main = Dialog.wrapper.transform.Find ("Main");
			Transform footer = Dialog.wrapper.transform.Find ("Footer");

			// Header elements
			Text heading = header.Find ("Heading").GetComponent<Text> ();

			// Main elements
			Text text;
			InputField inputField;
			Text inputText;

			// Footer elements
			Button buttonOK = footer.Find ("Button_OK").GetComponent<Button> ();
			Text buttonOKText = footer.Find ("Button_OK").Find ("Text").GetComponent<Text> ();
			Button buttonCancel = footer.Find ("Button_Cancel").GetComponent<Button> ();
			Text buttonCancelText = footer.Find ("Button_Cancel").Find ("Text").GetComponent<Text> ();

			// Remove listener
			buttonOK.onClick.RemoveAllListeners ();


			switch (type) {

			// New color scheme
			case "CS_ADD":

				if (Settings.OpenedVisualization != null)
				{
					// UI elements
					heading.text = "Neues Farbschema erstellen";
					inputField = Dialog.GetInputField ("", "Wie soll das neue Farbschema heißen?");
					inputText = Dialog.GetInputText ();

					// Events
					buttonOK.onClick.AddListener (delegate {

						long id = NewColorScheme (inputText.text);

						switch (id) {

						case (long) Database.Constants.DuplicateFound:

							// TODO
							print ("Bereits vorhanden.");

							break;

						case (long) Database.Constants.QueryFailed:

							// TODO
							print ("Fehlgeschlagen.");

							break;

						default:

							HideDialog ();

							break;

						}
					});
				}
				else
				{
					// UI elements
					heading.text = "Keine Visualisierung ausgewählt";
					inputText = Dialog.GetText ("Bitte wählen Sie eine Visualisierung aus, um ein neues Farbschema hinzuzufügen.");

					// Events
					buttonOK.onClick.AddListener (HideDialog);
				}

				break;



			// Edit color scheme
			case "CS_EDIT":

				if (colorScheme != null)
				{
					// UI elements
					heading.text = "Farbschema bearbeiten";
					inputField = Dialog.GetInputField (colorScheme.Name, colorScheme.Name);
					inputText = Dialog.GetInputText ();

					// Events
					buttonOK.onClick.AddListener (delegate {

						// Update color scheme objects
						if (Settings.ActiveColorScheme != null && Settings.ActiveColorScheme.Equals (colorScheme)) {
							Settings.ActiveColorScheme.Name = inputText.text;
						}
						colorScheme.Name = inputText.text;

						// Update database
						bool edited = Edit (colorScheme);

						if (edited) {
							Load ();
							Display ();
						} else {
							// TODO
							print ("Fehlgeschlagen.");
						}

						HideDialog ();
					});
				}
				else
				{
					return;
				}

				break;



			// Delete color scheme
			case "CS_DEL":

				if (colorScheme != null)
				{
					// UI elements
					heading.text = "Farbschema löschen";
					text = Dialog.GetText ("Farbschema \"" + colorScheme.Name + "\" endgültig löschen?");

					// Events
					buttonOK.onClick.AddListener (delegate {

						Delete (obj, colorScheme);
						Load ();
						Display ();
						HideDialog ();
					});
				}
				else
				{
					return;
				}

				break;



			default:
				return;

			}

			// Show dialog
			Dialog.dialog.SetActive (true);

			return;
		}

		return;
	}

	public void HideDialog ()
	{
		if (Dialog != null) {
			Dialog.HideDialog ();
		}
	}




    

	//-- DATABASE METHODS

	public void Load ()
    {
		if (Database.Connect() && Settings.OpenedVisualization != null &&
			Application.CanStreamedLevelBeLoaded (Settings.OpenedVisualization.BuildNumber))
        {
            // Database command
			SqliteCommand cmd = new SqliteCommand (Database.Connection);

            // Query statement
            string sql = "SELECT id,name,viz_id,colors FROM color_scheme WHERE viz_id = @Viz_ID ORDER BY name ASC";
            cmd.CommandText = sql;

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("Viz_ID", Settings.OpenedVisualization.ID));

            // Get sql results
            SqliteDataReader reader = cmd.ExecuteReader ();

			// Reset color schemes
			ColorSchemes = new List<ColorSchemeObj> ();

            // Read sql results
            while (reader.Read ())
            {
                // Create color scheme object
				ColorSchemeObj obj = new ColorSchemeObj (reader.GetString (1));

                // Set values
                obj.ID = reader.GetInt64 (0);
				obj.Visualization = Visualization.GetVisualization (reader.GetInt64 (2), false);
				obj.Colors = GetColors (reader.GetString (3));

				// Add color scheme to colorSchemes array
                ColorSchemes.Add (obj);
            }

            // Close reader
            reader.Close ();
            cmd.Dispose ();

            // Close database connection
            Database.Close ();
        }
    }

	public long Create (ColorSchemeObj colorScheme)
	{
		if (Database.Connect () && colorScheme != null && colorScheme.Name.Length > 0)
		{
			// Insert color scheme into database
			try
			{
				string sql = "INSERT INTO color_scheme (name,viz_id,colors) VALUES(@Name, @Viz_ID, @Colors); " +
					"SELECT last_insert_rowid()";
				SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Name", colorScheme.Name));
				cmd.Parameters.Add (new SqliteParameter ("Viz_ID", colorScheme.Visualization.ID));
				cmd.Parameters.Add (new SqliteParameter ("Colors", FormatColors (colorScheme.Colors)));

				// Execute insert statement and get ID
				long id = (long) cmd.ExecuteScalar ();

				// Close database connection
				cmd.Dispose ();
				Database.Close ();

				return id;
			}
			catch (SqliteException)
			{
				// Close database connection
				Database.Close ();

				return (long) Database.Constants.DuplicateFound;
			}
		}

		// Close database connection
		Database.Close ();

		return (long) Database.Constants.QueryFailed;
	}

	public bool Edit (ColorSchemeObj colorScheme)
	{
		if (Database.Connect () && colorScheme != null && colorScheme.Name.Length > 0)
		{
			try
			{
				// Query statement
				string sql = "UPDATE color_scheme SET name = @Name, viz_id = @Viz_ID, colors = @Colors WHERE id = @ID";
				SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Name", colorScheme.Name));
				cmd.Parameters.Add (new SqliteParameter ("Viz_ID", colorScheme.Visualization.ID));
				cmd.Parameters.Add (new SqliteParameter ("Colors", FormatColors (colorScheme.Colors)));
				cmd.Parameters.Add (new SqliteParameter ("ID", colorScheme.ID));

				// Result
				int result = cmd.ExecuteNonQuery ();

				// Close database connection
				cmd.Dispose ();
				Database.Close ();

				return result > 0;
			}
			catch (SqliteException) {}
		}

		// Close database connection
		Database.Close ();

		return false;
	}

	public bool Delete (ColorSchemeObj colorScheme)
	{
		if (Database.Connect () && colorScheme != null && ColorSchemes.Count > 1)
		{
			// Query statement
			string sql = "DELETE FROM color_scheme WHERE id = @ID";
			SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("ID", colorScheme.ID));

			// Result
			int result = cmd.ExecuteNonQuery ();

			// Close database connection
			cmd.Dispose ();
			Database.Close ();

			return result > 0;
		}

		// Close database connection
		Database.Close ();

		return false;
	}



	//-- HELPER METHODS

	public string FormatColors (Color [] colors)
	{
		// Output
		List<string> cols = new List<string> ();

		foreach (Color col in colors)
		{
			string [] c = new string [3];
			c [0] = col.r.ToString ();
			c [1] = col.g.ToString ();
			c [2] = col.b.ToString ();

			cols.Add (String.Join (",", c));
		}

		return cols.Count > 0 ? String.Join (";", cols.ToArray ()) : null;
	}

	public Color [] GetColors (string input)
	{
		// List with colors
		List<Color> colors = new List<Color> ();

		// Plain colors
		string [] plain = input.Split (';');

		// Add colors
		foreach (string s in plain)
		{
			// Create color object
			Color obj = new Color ();

			// Split rgb values
			string [] rgb = s.Split (',');

			if (rgb.Length == 3)
			{
				// Get values
				obj.r = float.Parse (rgb [0]);
				obj.g = float.Parse (rgb [1]);
				obj.b = float.Parse (rgb [2]);
				obj.a = 1;

				// Add to list
				colors.Add (obj);
			}
		}

		return colors.ToArray ();
	}







    void ColorChange(int number)
    {
        ColorPicker.SetActive(true);
//        MenuFunctions.num2xy = number;
    }
//
//    void Save(int changedColor)
//    {
//
//        foreach (int p in hash)
//        {
//
//            int i = Math.Abs(p);
//            while (i >= 10)
//            {
//                i /= 10;
//            }
//            if (i == changedColor)
//            {
//                colorSchemes[changedColor-1].ColorSchemes[p-(i*10)] = GameObject.Find("Image" + p).GetComponent<Image>().color;
//                
//            }
//            
//        }
//        String combine = "";
//        for(int count =0; count <colorSchemes[changedColor-1].ColorSchemes.Length; count++)
//        {
//            combine += colorSchemes[changedColor-1].ColorSchemes[count].r + "," + colorSchemes[changedColor-1].ColorSchemes[count].g + "," + colorSchemes[changedColor-1].ColorSchemes[count].b + ";";
//        }
//        string s = combine.Remove(combine.Length - 1);
//
//        if (DbConnect())
//        {
//            
//            // Query statement
//            string sql = "UPDATE color_scheme SET colors = @param1 WHERE id = @param2";
//
//
//            SqliteCommand cmd2 = new SqliteCommand(sql, db);
//            cmd2.Parameters.Add(new SqliteParameter("@param1",s));
//            cmd2.Parameters.Add(new SqliteParameter("@param2",colorSchemes[changedColor - 1].ID));
//
//            // Result
//            cmd2.ExecuteNonQuery();
//            // Close database connection
//            cmd2.Dispose();
//            DbClose();
//        }
//    }
//
//    void Delete(int w)
//    {
//        if (DbConnect())
//        {
//            // Query statement
//            string sql2 = "DELETE FROM color_scheme WHERE id = @param1";
//
//
//            SqliteCommand cmd3 = new SqliteCommand(sql2, db);
//            cmd3.Parameters.Add(new SqliteParameter("@param1",colorSchemes[w - 1].ID));
//
//            // Result
//            cmd3.ExecuteNonQuery();
//            // Close database connection
//            cmd3.Dispose();
//            DbClose();
//        }
//        Destroy(GameObject.Find("Visualisation" + w));
//    }
//
//    void CannotDelete(int p)
//    {
//        if (DbConnect())
//        {
//            // Database command
//            SqliteCommand cmd5 = new SqliteCommand(db);
//
//            // Query statement
//            string sql3 = "SELECT viz_id FROM color_scheme WHERE viz_id = @param1";
//            cmd5.CommandText = sql3;
//            cmd5.Parameters.Add(new SqliteParameter("@param1",colorSchemes[p - 1].VizId));
//
//            // Get sql results
//            SqliteDataReader reader2 = cmd5.ExecuteReader();
//
//            // Read sql results
//            int count = 0;
//            while (reader2.Read())
//            {
//
//                int s = reader2.GetInt32(0);
//                count++;
//
//            }
//            if (count <= 1)
//            {
//                if (EditorUtility.DisplayDialog("Letztes Farbschema", "Das Farbschema \"" + colorSchemes[p-1].Name + "\" kann nicht gelöscht werden, da es das letzte Schema ist!", "OK"))
//                {
//                    check = false;
//                }
//            }
//
//            // Close reader
//            reader2.Close();
//            cmd5.Dispose();
//
//            // Close database connection
//            DbClose();
//        }
//  
//
//    }
//    
//
//    // call the input field
//    void CreateNew(int w)
//    {
//        imag.SetActive(true);
//        MenuFunctions.num = w;
//
//    }
//    GameObject gObj;
//    bool ToggleArrow(GameObject obj,GameObject obj2,bool active)
//    {
//        active = !active;
//        if (active)
//        {
//            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/arrow-down");
//            obj2.SetActive(true);
//            
//        }
//        else
//        {
//            obj.GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/arrow-right");
//            obj2.SetActive(false);
//        }
//        return active;
//    }
		 
}
