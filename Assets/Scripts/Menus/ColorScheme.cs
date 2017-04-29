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
	public static List<ColorSchemeObj> ColorSchemes;

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
			UpdateColor (Settings.Selected.ColorScheme, ActiveColorID, col);
		});
	}



	public void Display ()
	{
		if (Database.Connect ())
		{
			// Remove all GameObjects
			for (int i = transform.childCount - 1; i >= 0; i--) {
				GameObject.DestroyImmediate (transform.GetChild (i).gameObject);
			}

			// Set opened color scheme
			if (Settings.Selected.ColorScheme == null && Settings.Active.ColorScheme != null) {
				Settings.Selected.ColorScheme = Settings.Active.ColorScheme;
			}

			// Insert default color scheme
			InsertDefault ();

			// Load color schemes from database
			Load (false);

			if (ColorSchemes != null && ColorSchemes.Count > 0)
			{
				foreach (ColorSchemeObj cs in ColorSchemes)
				{
					// Display color scheme
					GameObject gameObject = DisplayColorScheme (cs);

					if (gameObject != null)
					{
						// Display colors
						for (int i=0; i < cs.Colors.Length; i++) {
							DisplayColor (cs, cs.Colors [i], i);
						}

						// Get main and contents
						Transform main = gameObject.transform.Find ("Main");
						Transform contents = gameObject.transform.Find ("Contents");

						// Set scaling
						gameObject.GetComponent<RectTransform> ().localScale = Vector3.one;
						main.GetComponent<RectTransform> ().localScale = Vector3.one;

						// Hide colors
						if (contents != null && !cs.Equals (Settings.Selected.ColorScheme)) {
							contents.gameObject.SetActive (false);
						}
					}
				}
			}
		}

		// Close database connection
		Database.Close ();
	}

	public GameObject DisplayColorScheme (ColorSchemeObj colorScheme)
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
			mainTextArrow.text = colorScheme.Equals (Settings.Selected.ColorScheme)
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

			if (colorScheme.Equals (Settings.Active.ColorScheme))
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


			if (colorScheme.Name != Settings.Selected.Visualization.Name)
			{
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
			}


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
			glg.cellSize = new Vector2 (53.5f, 53.5f);
			glg.spacing = new Vector2 (25, 25);
			glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
			glg.constraintCount = 10;

			return gameObject;
		}

		return null;
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

			if (colorScheme.Name != Settings.Selected.Visualization.Name)
			{
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
	}

	public void ToggleColors (ColorSchemeObj colorScheme)
	{
		ToggleColors (colorScheme, false);
	}

	public void ToggleColors (ColorSchemeObj colorScheme, bool forceOpen)
	{
		// Set color scheme as active color scheme
		if (!forceOpen) Settings.Selected.ColorScheme = colorScheme;

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
		Settings.Selected.ColorScheme = opened ? colorScheme : null;

		// Scroll to top if scrollbar is hidden
		ScrollToTop ();
	}

	public void ScrollToTop ()
	{
		// Force canvas to update elements
		Canvas.ForceUpdateCanvases ();

		// Scroll to top if scrollbar is not visible
		if (transform.GetComponent<RectTransform> ().sizeDelta.y < 0)
			gameObject.transform.parent.GetComponent<ScrollRect> ().verticalScrollbar.value = 1;
	}

	public long NewColorScheme (string name)
	{
		if (name.Length > 0)
		{
			// Create color scheme object
			ColorSchemeObj colorScheme = new ColorSchemeObj (name, Settings.Selected.Visualization);

			// Create object in database
			long id = Create (colorScheme);

			// Reload playlists
			if (id > 0)
			{
				Load (false);
				Display ();
			}

			return id;
		}

		return (long) Database.Constants.EmptyInputValue;
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
			if (colorScheme.Equals (Settings.Active.ColorScheme)) Settings.Active.ColorScheme = null;
			if (colorScheme.Equals (Settings.Selected.ColorScheme)) Settings.Selected.ColorScheme = null;

			return true;
		}

		return false;
	}

	public static ColorSchemeObj GetDefault ()
	{
		if (Settings.Active.Visualization != null) {
			Load (true);
			return ColorSchemes.Find (x => x.Name == Settings.Active.Visualization.Name);
		}

		return null;
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

			// Button
			Button button = Dialog.ButtonOK;
			Text buttonText = Dialog.GetButtonText ();

			// Remove listener
			button.onClick.RemoveAllListeners ();


			switch (type) {

			// New color scheme
			case "CS_ADD":

				if (Settings.Selected.Visualization != null)
				{
					// UI elements
					Dialog.SetHeading ("Neues Farbschema erstellen");
					Dialog.SetInputField ("", "Wie soll das neue Farbschema heißen?");

					// Events
					button.onClick.AddListener (delegate {

						long id = NewColorScheme (Dialog.GetInputText ());

						switch (id) {

						// Color scheme name already taken
						case (long) Database.Constants.DuplicateFound:

							Dialog.SetInfo ("Ein Farbschema mit diesem Namen ist für die ausgewählte Visualisierung bereits vorhanden.");
							break;

						// Database query failed
						case (long) Database.Constants.QueryFailed:

							Dialog.SetInfo ("Das Farbschema konnte nicht erstellt werden.");
							break;

						// No user input
						case (long) Database.Constants.EmptyInputValue:

							Dialog.SetInfo ("Bitte geben Sie einen Namen für das Farbschema ein.");
							break;

						default:

							Dialog.HideDialog ();
							break;

						}
					});
				}
				else
				{
					Dialog.ShowDialog (
						"Keine Visualisierung ausgewählt",
						"Bitte wählen Sie eine Visualisierung aus, um ein neues Farbschema hinzuzufügen."
					);
				}

				break;



			// Edit color scheme
			case "CS_EDIT":

				if (colorScheme != null)
				{
					// UI elements
					Dialog.SetHeading ("Farbschema bearbeiten");
					Dialog.SetInputField (colorScheme.Name, colorScheme.Name);

					// Events
					button.onClick.AddListener (delegate {

						// Update color scheme objects
						if (Settings.Active.ColorScheme != null && Settings.Active.ColorScheme.Equals (colorScheme)) {
							Settings.Active.ColorScheme.Name = Dialog.GetInputText ();
						}
						colorScheme.Name = Dialog.GetInputText ();

						// Update database
						long result = Edit (colorScheme);

						// Handle database result
						switch (result) {

						// Color scheme name already taken
						case (long) Database.Constants.DuplicateFound:

							Dialog.SetInfo ("Ein Farbschema mit diesem Namen ist für die ausgewählte Visualisierung bereits vorhanden.");
							break;

						// Database query failed
						case (long) Database.Constants.QueryFailed:

							Dialog.SetInfo ("Das Farbschema konnte nicht aktualisiert werden.");
							break;

						// No user input
						case (long) Database.Constants.EmptyInputValue:

							Dialog.SetInfo ("Bitte geben Sie einen Namen für das Farbschema ein.");
							break;

						default:

							Load (false);
							Display ();
							Dialog.HideDialog ();
							break;

						}
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
					Dialog.SetHeading ("Farbschema löschen");
					Dialog.SetText ("Farbschema \"" + colorScheme.Name + "\" endgültig löschen?");

					// Events
					button.onClick.AddListener (delegate {

						Delete (obj, colorScheme);

						Load (false);
						Display ();

						Dialog.HideDialog ();

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

    

	//-- DATABASE METHODS

	public void InsertDefault ()
	{
		if (Database.Connect () && Settings.Selected.Visualization != null && Settings.Selected.Visualization.ID > 0 &&
			!Exists (new ColorSchemeObj (Settings.Selected.Visualization.Name, Settings.Selected.Visualization)))
		{
			// Insert default color scheme
			string sql = "INSERT INTO color_scheme (name, viz_id, colors) VALUES (@Name, @Viz_ID, @Colors)";
			SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("Name", Settings.Selected.Visualization.Name));
			cmd.Parameters.Add (new SqliteParameter ("Viz_ID", Settings.Selected.Visualization.ID));

			if (Settings.Defaults.Colors.ContainsKey (Settings.Selected.Visualization.Name))
			{
				// Set colors
				Color[] colors = Settings.Defaults.Colors [Settings.Selected.Visualization.Name];
				cmd.Parameters.Add (new SqliteParameter ("Colors", FormatColors (colors)));

				// Execute insert statement
				cmd.ExecuteNonQuery ();

				// Dispose command
				cmd.Dispose ();
			}
		}

		// Close database connection
		Database.Close ();
	}

	public static void Load (bool defaultOnly)
    {
		if (Database.Connect() && Settings.Selected.Visualization != null &&
			Application.CanStreamedLevelBeLoaded (Settings.Selected.Visualization.BuildNumber))
        {
            // Database command
			SqliteCommand cmd = new SqliteCommand (Database.Connection);

            // Query statement
            string sql = "SELECT id,name,viz_id,colors FROM color_scheme WHERE viz_id = @Viz_ID " +
				(defaultOnly ? "AND name = @Name " : "") +
				"ORDER BY name ASC";
            cmd.CommandText = sql;

			// Add Parameters to statement
			cmd.Parameters.Add (new SqliteParameter ("Viz_ID", Settings.Selected.Visualization.ID));
			if (defaultOnly) cmd.Parameters.Add (new SqliteParameter ("Name", Settings.Selected.Visualization.Name));

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
			if (!Exists (colorScheme))
			{
				// Insert color scheme into database
				string sql = "INSERT INTO color_scheme (name,viz_id,colors) VALUES (@Name, @Viz_ID, @Colors); " +
				"SELECT last_insert_rowid()";
				SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

				// Add Parameters to statement
				cmd.Parameters.Add (new SqliteParameter ("Name", colorScheme.Name));
				cmd.Parameters.Add (new SqliteParameter ("Viz_ID", colorScheme.Visualization.ID));
				cmd.Parameters.Add (new SqliteParameter ("Colors", FormatColors (colorScheme.Colors)));

				// Execute insert statement and get ID
				long id = (long)cmd.ExecuteScalar ();

				// Close database connection
				cmd.Dispose ();
				Database.Close ();

				return id;
			}

			// Close database connection
			Database.Close ();

			return (long) Database.Constants.DuplicateFound;
		}

		// Close database connection
		Database.Close ();

		return (long) Database.Constants.QueryFailed;
	}

	public long Edit (ColorSchemeObj colorScheme)
	{
		if (Database.Connect () && colorScheme != null && colorScheme.Name.Length > 0)
		{
			if (!Exists (colorScheme))
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

				return (long) (result > 0 ? Database.Constants.Successful : Database.Constants.QueryFailed);
			}

			// Close database connection
			Database.Close ();

			return (long) Database.Constants.DuplicateFound;
		}

		// Close database connection
		Database.Close ();

		return (long) (colorScheme.Name.Length > 0 ? Database.Constants.QueryFailed : Database.Constants.EmptyInputValue);
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

	public bool Exists (ColorSchemeObj colorScheme)
	{
		string sql = "SELECT id FROM color_scheme WHERE name = @Name AND viz_id = @Viz_ID AND id != @ID";
		SqliteCommand cmd = new SqliteCommand (sql, Database.Connection);

		// Add Parameters to statement
		cmd.Parameters.Add (new SqliteParameter ("Name", colorScheme.Name));
		cmd.Parameters.Add (new SqliteParameter ("Viz_ID", colorScheme.Visualization.ID));
		cmd.Parameters.Add (new SqliteParameter ("ID", colorScheme.ID));

		// Exit if color scheme name already exists
		SqliteDataReader reader = cmd.ExecuteReader ();
		while (reader.Read ())
		{
			cmd.Dispose ();
			reader.Close ();

			return true;
		}

		// Dispose command
		cmd.Dispose ();

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

	public static Color [] GetColors (string input)
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
		 
}
