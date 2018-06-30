/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.Collections.Generic;
using System.Globalization;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <inheritdoc />
/// <summary>
///     Provides methods to display color schemes of a given visualization inside the UI.
/// </summary>
public class ColorScheme : MonoBehaviour
{
	/// <summary>
	///     List of available color schemes of the selected visualization.
	/// </summary>
	private static List<ColorSchemeObj> _colorSchemes;

	/// <summary>
	///     The current by the user selected color.
	/// </summary>
	private static GameObject _activeColor;

	/// <summary>
	///     The id of the current by the user selected color.
	/// </summary>
	private static int _activeColorId;

	/// <summary>
	///     The <see cref="UnityEngine.GameObject" /> holding the color picker.
	/// </summary>
	public GameObject ColorPicker;

	/// <summary>
	///     Dialog to display info messages and to provide user input ability.
	/// </summary>
	public Dialog Dialog;

	/// <summary>
	///     The color picker object which provides methods for the user to select any color for a color scheme.
	/// </summary>
	public ColorPicker PickerObj;


    private void Start()
    {
        // Add listener for Color Picker
        PickerObj.onValueChanged.AddListener(
            col => { UpdateColor(Settings.Selected.ColorScheme, _activeColorId, col); });

        // Show or hide start button
        MenuFunctions.ToggleStart();
    }

    public void Display()
    {
        if (Database.Connect())
        {
            // Remove all GameObjects
            for (var i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);

            // Set opened color scheme
            if (Settings.Selected.ColorScheme == null && Settings.Active.ColorScheme != null)
                Settings.Selected.ColorScheme = Settings.Active.ColorScheme;

            // Load color schemes from database
            Load(false);

            if (_colorSchemes != null && _colorSchemes.Count > 0)
                foreach (var cs in _colorSchemes)
                {
                    // Display color scheme
                    var element = DisplayColorScheme(cs);

                    if (element == null) continue;

                    // Display colors
                    for (var i = 0; i < cs.Colors.Length; i++) DisplayColor(cs, cs.Colors[i], i);

                    // Get main and contents
                    var main = element.transform.Find("Main");
                    var contents = element.transform.Find("Contents");

                    // Set scaling
                    element.GetComponent<RectTransform>().localScale = Vector3.one;
                    main.GetComponent<RectTransform>().localScale = Vector3.one;

                    // Hide colors
                    if (contents != null && !cs.Equals(Settings.Selected.ColorScheme))
                        contents.gameObject.SetActive(false);
                }
        }

        // Close database connection
        Database.Close();
    }

    private GameObject DisplayColorScheme(ColorSchemeObj colorScheme)
    {
        if (colorScheme == null) return null;

        var title = "#" + colorScheme.Id;

        // Create GameOject
        var element = new GameObject(title);
        element.transform.SetParent(transform);


        // Create main GameObject
        var main = new GameObject("Main");
        main.transform.SetParent(element.transform);

        // Add Vertical Layout Group
        var vlg = element.AddComponent<VerticalLayoutGroup>();
        vlg.spacing = 20;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;


        // Add Layout Element to GameObject
        var mainLayout = main.AddComponent<LayoutElement>();
        mainLayout.minHeight = 30;
        mainLayout.preferredHeight = mainLayout.minHeight;

        // Add image to GameObject
        var mainImg = main.AddComponent<Image>();
        mainImg.color = Color.clear;

        // Set transformations
        mainImg.rectTransform.pivot = new Vector2(0, 0.5f);

        // Add Horizontal Layout Group
        var mainHlg = main.AddComponent<HorizontalLayoutGroup>();
        mainHlg.spacing = 10;
        mainHlg.childForceExpandWidth = false;
        mainHlg.childForceExpandHeight = false;
        mainHlg.childAlignment = TextAnchor.MiddleLeft;

        // Set padding right of Horizontal Layout Group
        mainHlg.padding = new RectOffset(0, 65, 0, 0);


        // Create arrow text GameObject
        var mainArrow = new GameObject("Arrow");
        mainArrow.transform.SetParent(main.transform);

        // Add text
        var mainTextArrow = mainArrow.AddComponent<TextUnicode>();
        mainTextArrow.text = colorScheme.Equals(Settings.Selected.ColorScheme)
            ? IconFont.DropdownOpened
            : IconFont.DropdownClosed;

        // Set text alignment
        mainTextArrow.alignment = TextAnchor.MiddleLeft;

        // Font settings
        mainTextArrow.font = IconFont.Font;
        mainTextArrow.fontSize = 20;

        // Add Layout Element
        var mainLayoutElementArrow = mainArrow.AddComponent<LayoutElement>();
        mainLayoutElementArrow.minWidth = 22;


        // Create text GameObject
        var mainText = new GameObject("Text");
        mainText.transform.SetParent(main.transform);

        // Add text
        var text = mainText.AddComponent<Text>();
        text.text = colorScheme.Name;

        if (colorScheme.Name == Settings.Selected.Visualization.Name) text.text += " (Standard)";

        // Set text alignment
        text.alignment = TextAnchor.MiddleLeft;

        // Set text color
        text.color = Color.white;

        // Font settings
        text.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
        text.fontSize = 30;

        // Set transformations
        text.rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Add button
        var buttonText = mainText.AddComponent<Button>();
        buttonText.transition = Selectable.Transition.Animation;

        // Add Event to Button
        buttonText.onClick.AddListener(delegate { ToggleColors(colorScheme); });

        // Add animator
        var animatorText = mainText.AddComponent<Animator>();
        animatorText.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/MenuButtons");


        // Create active text GameObject
        var mainActive = new GameObject("Active");
        mainActive.transform.SetParent(main.transform);

        // Add text
        var mainTextActive = mainActive.AddComponent<TextUnicode>();

        if (colorScheme.Equals(Settings.Active.ColorScheme))
        {
            mainTextActive.text = IconFont.Visualization;
            mainTextActive.fontSize = 30;
            mainTextActive.color = new Color(0.7f, 0.7f, 0.7f);
        }

        // Set text alignment
        mainTextActive.alignment = TextAnchor.MiddleRight;

        // Font settings
        mainTextActive.font = IconFont.Font;

        // Add Layout Element
        var mainLayoutElementListening = mainActive.AddComponent<LayoutElement>();
        mainLayoutElementListening.preferredWidth = 40;


        if (colorScheme.Name != Settings.Selected.Visualization.Name)
        {
            // Create edit icons GameObject
            var editIcons = new GameObject("Images");
            editIcons.transform.SetParent(main.transform);

            // Set transformations
            var editIconsTrans = editIcons.AddComponent<RectTransform>();
            editIconsTrans.anchoredPosition = Vector2.zero;
            editIconsTrans.anchorMin = new Vector2(1, 0.5f);
            editIconsTrans.anchorMax = new Vector2(1, 0.5f);
            editIconsTrans.pivot = new Vector2(1, 0.5f);

            // Add Layout Element
            var editIconslayoutElement = editIcons.AddComponent<LayoutElement>();
            editIconslayoutElement.ignoreLayout = true;

            // Add Content Size Fitter
            var editIconsCsf = editIcons.AddComponent<ContentSizeFitter>();
            editIconsCsf.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
            editIconsCsf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            // Add Layout Group
            var editIconsHlgImg = editIcons.AddComponent<HorizontalLayoutGroup>();
            editIconsHlgImg.childAlignment = TextAnchor.MiddleRight;
            editIconsHlgImg.spacing = 5;
            editIconsHlgImg.childForceExpandWidth = false;
            editIconsHlgImg.childForceExpandHeight = false;

            // Disable edit icons GameObject
            editIcons.SetActive(false);


            // Create edit text GameObject
            var edit = new GameObject("Edit");
            edit.transform.SetParent(editIcons.transform);

            // Add text
            var editText = edit.AddComponent<TextUnicode>();
            editText.text = IconFont.Edit;

            // Set text alignment
            editText.alignment = TextAnchor.MiddleRight;

            // Set transformations
            editText.rectTransform.sizeDelta = new Vector2(20, 30);

            // Font settings
            editText.font = IconFont.Font;
            editText.fontSize = 30;

            // Add button
            var buttonEditEvt = edit.AddComponent<Button>();
            buttonEditEvt.transition = Selectable.Transition.Animation;

            // Add button onclick event
            buttonEditEvt.onClick.AddListener(delegate { ShowDialog(Dialog.Type.ColorSchemeEdit, element); });

            // Add animator
            var animatorEditEvt = edit.AddComponent<Animator>();
            animatorEditEvt.runtimeAnimatorController =
                Resources.Load<RuntimeAnimatorController>("Animations/MenuButtons");


            // Create delete text GameObject
            var delete = new GameObject("Delete");
            delete.transform.SetParent(editIcons.transform);

            // Add text
            var deleteText = delete.AddComponent<Text>();
            deleteText.text = IconFont.Trash;

            // Set text alignment
            deleteText.alignment = TextAnchor.MiddleRight;

            // Set transformations
            deleteText.rectTransform.sizeDelta = new Vector2(20, 30);

            // Font settings
            deleteText.font = IconFont.Font;
            deleteText.fontSize = 30;

            // Add button
            var buttonDeleteEvt = delete.AddComponent<Button>();
            buttonDeleteEvt.transition = Selectable.Transition.Animation;

            // Add button onclick event
            buttonDeleteEvt.onClick.AddListener(delegate { ShowDialog(Dialog.Type.ColorSchemeDelete, element); });

            // Add animator
            var animatorDeleteEvt = delete.AddComponent<Animator>();
            animatorDeleteEvt.runtimeAnimatorController =
                Resources.Load<RuntimeAnimatorController>("Animations/MenuButtons");


            // Create GameObject Event Triggers
            var evtWrapper = element.AddComponent<EventTrigger>();

            // Add Hover Enter Event
            var evtHover = new EventTrigger.Entry {eventID = EventTriggerType.PointerEnter};
            evtWrapper.triggers.Add(evtHover);

            evtHover.callback.AddListener(eventData => { editIcons.SetActive(true); });

            // Add Hover Exit Event
            var evtExit = new EventTrigger.Entry {eventID = EventTriggerType.PointerExit};
            evtWrapper.triggers.Add(evtExit);

            evtExit.callback.AddListener(eventData => { editIcons.SetActive(false); });
        }


        // Add Event Trigger
        var evtMain = main.AddComponent<EventTrigger>();

        // Add Click Event
        var evtClick = new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
        evtMain.triggers.Add(evtClick);

        evtClick.callback.AddListener(eventData => { ToggleColors(colorScheme); });


        // Add Contents GameObject
        var contents = new GameObject("Contents");
        contents.transform.SetParent(element.transform);

        // Add Grid Layout Group
        var glg = contents.AddComponent<GridLayoutGroup>();

        // Set Grid Layout
        glg.cellSize = new Vector2(53.5f, 53.5f);
        glg.spacing = new Vector2(25, 25);
        glg.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        glg.constraintCount = 10;

        return element;
    }

    private void DisplayColor(ColorSchemeObj colorScheme, Color color, int id)
    {
        if (colorScheme == null) return;

        // Get Contents GameObject
        var contents = transform.Find("#" + colorScheme.Id + "/Contents").gameObject;

        // Create Image GameObject
        var image = new GameObject("#" + colorScheme.Id + "." + id);
        image.transform.SetParent(contents.transform);

        // Add image
        var img = image.AddComponent<Image>();
        img.color = color;

        if (colorScheme.Name != Settings.Selected.Visualization.Name)
        {
            // Add Event Trigger
            var trigger = image.AddComponent<EventTrigger>();

            // Add Click Event
            var evtClick = new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
            trigger.triggers.Add(evtClick);

            evtClick.callback.AddListener(eventData =>
            {
                // Set active color
                _activeColor = image;
                _activeColorId = id;

                // Set current color
                PickerObj.CurrentColor = colorScheme.Colors[id];

                // Show color picker
                ColorPicker.SetActive(true);
            });
        }
        else
        {
            // Create Overlay GameObject
            var overlay = new GameObject("Overlay");
            overlay.transform.SetParent(image.transform);

            // Add RectTransform
            var trans = overlay.AddComponent<RectTransform>();
            trans.anchorMin = Vector2.zero;
            trans.anchorMax = Vector2.one;
            trans.offsetMin = Vector2.zero;
            trans.offsetMax = Vector2.zero;

            // Add Image
            var overlayImg = overlay.AddComponent<Image>();
            overlayImg.color = new Color(0, 0, 0, 0.5f);


            // Create Text GameObject
            var text = new GameObject("Text");
            text.transform.SetParent(overlay.transform);

            // Add RectTransform
            var textTrans = text.AddComponent<RectTransform>();
            textTrans.anchorMin = Vector2.zero;
            textTrans.anchorMax = Vector2.one;
            textTrans.offsetMin = Vector2.zero;
            textTrans.offsetMax = Vector2.zero;

            // Add Text
            var textText = text.AddComponent<TextUnicode>();
            textText.text = IconFont.Lock;
            textText.color = Color.white;
            textText.font = IconFont.Font;
            textText.fontSize = 24;
            textText.alignment = TextAnchor.MiddleCenter;
        }
    }

    private void ToggleColors(ColorSchemeObj colorScheme, bool forceOpen = false)
    {
        // Set color scheme as active color scheme
        if (!forceOpen) Settings.Selected.ColorScheme = colorScheme;

        // Show or hide colors
        var opened = false;
        foreach (var cs in _colorSchemes)
        {
            // Get GameObject for current color
            var contents = transform.Find("#" + cs.Id + "/Contents");
            var main = contents != null ? contents.gameObject : null;

            // Get arrow
            var arr = transform.Find("#" + cs.Id + "/Main/Arrow").GetComponent<Text>();

            // Toggle colors for GameObject
            if (main != null)
            {
                if (Equals(cs, colorScheme))
                {
                    main.SetActive(forceOpen || !main.activeSelf);
                    opened = main.activeSelf;
                }
                else
                {
                    main.SetActive(false);

                    if (arr.text == IconFont.DropdownOpened) arr.text = IconFont.DropdownClosed;
                }
            }

            // Change arrows
            if (!forceOpen && !Equals(cs, colorScheme)) arr.text = IconFont.DropdownClosed;
        }

        // Change arrow image
        var arrow = transform.Find("#" + colorScheme.Id + "/Main/Arrow").GetComponent<Text>();
        if (!forceOpen && arrow != null) arrow.text = opened ? IconFont.DropdownOpened : IconFont.DropdownClosed;

        // Set opened and selected color scheme
        Settings.Selected.ColorScheme = opened ? colorScheme : null;

        // Scroll to top if scrollbar is hidden
        ScrollToTop();
    }

    private void ScrollToTop()
    {
        // Force canvas to update elements
        Canvas.ForceUpdateCanvases();

        // Scroll to top if scrollbar is not visible
        if (transform.GetComponent<RectTransform>().sizeDelta.y < 0)
            gameObject.transform.parent.GetComponent<ScrollRect>().verticalScrollbar.value = 1;
    }

    private long NewColorScheme(string title)
    {
        if (title.Length <= 0) return (long) Database.Constants.EmptyInputValue;

        // Create color scheme object
        var colorScheme = new ColorSchemeObj(title, Settings.Selected.Visualization);

        // Create object in database
        var id = Create(colorScheme);

        if (id <= 0) return id;

        // Reload playlists
        Load(false);
        Display();

        return id;
    }

    private static void Delete(Object element, ColorSchemeObj colorScheme)
    {
        if (!Delete(colorScheme)) return;

        // Remove from interface
        DestroyImmediate(element);

        // Remove from list
        _colorSchemes.Remove(colorScheme);

        // Unset active and opened color scheme
        if (colorScheme.Equals(Settings.Active.ColorScheme)) Settings.Active.ColorScheme = null;
        if (colorScheme.Equals(Settings.Selected.ColorScheme)) Settings.Selected.ColorScheme = null;
    }

    public static ColorSchemeObj GetDefault(VisualizationObj viz)
    {
        if (viz == null) return null;

        Load(true);

        return _colorSchemes.Find(x => x.Name == viz.Name);
    }

    private ColorSchemeObj FindColorScheme(Object element)
    {
        if (element == null) return null;

        // Get color scheme id
        var title = element.name.Split('.');
        var csId = long.Parse(title[0].Split('#')[1]);

        // Get color scheme
        return _colorSchemes.Find(x => x.Id == csId);
    }

    private static void UpdateColor(ColorSchemeObj colorScheme, int id, Color color)
    {
        if (colorScheme == null || colorScheme.Colors.Length < id + 1) return;

        // Set color
        colorScheme.Colors[id] = color;

        // Change image
        GameObject.Find("#" + colorScheme.Id + "." + id).GetComponent<Image>().color = color;

        // Edit color scheme in database
        Edit(colorScheme);
    }

    private void ShowDialog(Dialog.Type type, GameObject obj = null)
    {
        if (Dialog == null) return;

        // Color scheme object
        var colorScheme = FindColorScheme(obj);

        // Button
        var button = Dialog.ButtonOk;

        // Remove listener
        button.onClick.RemoveAllListeners();


        switch (type)
        {
            // New color scheme
            case Dialog.Type.ColorSchemeAdd:

                if (Settings.Selected.Visualization != null)
                {
                    // UI elements
                    Dialog.SetHeading(Settings.MenuManager.LangManager.getString("newColor"));
                    Dialog.SetInputField("", Settings.MenuManager.LangManager.getString("nameColor"));

                    // Events
                    button.onClick.AddListener(delegate
                    {
                        var id = NewColorScheme(Dialog.GetInputText());

                        switch (id)
                        {
                            // Color scheme name already taken
                            case (long) Database.Constants.DuplicateFound:

                                Dialog.SetInfo(Settings.MenuManager.LangManager.getString("existsColor"));
                                break;

                            // Database query failed
                            case (long) Database.Constants.QueryFailed:

                                Dialog.SetInfo(Settings.MenuManager.LangManager.getString("noColor"));
                                break;

                            // No user input
                            case (long) Database.Constants.EmptyInputValue:

                                Dialog.SetInfo(Settings.MenuManager.LangManager.getString("nameColor"));
                                break;

                            default:

                                Dialog.HideDialog();
                                break;
                        }
                    });
                }
                else
                {
                    print(Settings.MenuManager.LangManager);
                    Dialog.ShowDialog(
                        Settings.MenuManager.LangManager.getString("noChosenColor"),
                        Settings.MenuManager.LangManager.getString("chooseColor")
                    );
                }

                break;


            // Edit color scheme
            case Dialog.Type.ColorSchemeEdit:

                if (colorScheme != null)
                {
                    // UI elements
                    Dialog.SetHeading(Settings.MenuManager.LangManager.getString("editColor"));
                    Dialog.SetInputField(colorScheme.Name, colorScheme.Name);

                    // Events
                    button.onClick.AddListener(delegate
                    {
                        // Update color scheme objects
                        if (Settings.Active.ColorScheme != null && Settings.Active.ColorScheme.Equals(colorScheme))
                            Settings.Active.ColorScheme.Name = Dialog.GetInputText();

                        colorScheme.Name = Dialog.GetInputText();

                        // Update database
                        var result = Edit(colorScheme);

                        // Handle database result
                        switch (result)
                        {
                            // Color scheme name already taken
                            case (long) Database.Constants.DuplicateFound:

                                Dialog.SetInfo(Settings.MenuManager.LangManager.getString("existsColor"));
                                break;

                            // Database query failed
                            case (long) Database.Constants.QueryFailed:

                                Dialog.SetInfo(Settings.MenuManager.LangManager.getString("noEditColor"));
                                break;

                            // No user input
                            case (long) Database.Constants.EmptyInputValue:

                                Dialog.SetInfo(Settings.MenuManager.LangManager.getString("nameColor"));
                                break;

                            default:

                                Load(false);
                                Display();
                                Dialog.HideDialog();
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
            case Dialog.Type.ColorSchemeDelete:

                if (colorScheme != null)
                {
                    // UI elements
                    Dialog.SetHeading(Settings.MenuManager.LangManager.getString("deleteColor"));
                    Dialog.SetText(Settings.MenuManager.LangManager.getString("sureDelete"));

                    // Events
                    button.onClick.AddListener(delegate
                    {
                        Delete(obj, colorScheme);

                        Load(false);
                        Display();

                        Dialog.HideDialog();
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
        Dialog.Element.SetActive(true);
    }


    //-- DATABASE METHODS

    private static void Load(bool defaultOnly)
    {
        // Reset color schemes
        _colorSchemes = new List<ColorSchemeObj>();

        // Get color schemes
        if (!Database.Connect() || Settings.Selected.Visualization == null || Settings.Selected.Visualization.Id <= 0 ||
            !Application.CanStreamedLevelBeLoaded(Settings.Selected.Visualization.BuildNumber)) return;

        // Database command
        var cmd = new SqliteCommand(Database.Connection);

        // Query statement
        var sql = "SELECT id,name,viz_id,colors FROM color_scheme WHERE viz_id = @Viz_ID " +
                  (defaultOnly ? "AND name = @Name " : "") + "ORDER BY name ASC";
        cmd.CommandText = sql;

        // Add Parameters to statement
        cmd.Parameters.Add(new SqliteParameter("Viz_ID", Settings.Selected.Visualization.Id));
        if (defaultOnly) cmd.Parameters.Add(new SqliteParameter("Name", Settings.Selected.Visualization.Name));

        // Get sql results
        var reader = cmd.ExecuteReader();

        // Read sql results
        while (reader.Read())
        {
            // Create color scheme object and set values
            var obj = new ColorSchemeObj(reader.GetString(1))
            {
                Id = reader.GetInt64(0),
                Visualization = Visualization.GetVisualization(reader.GetInt64(2), false),
                Colors = GetColors(reader.GetString(3))
            };

            // Add color scheme to colorSchemes array
            _colorSchemes.Add(obj);
        }

        // Close reader
        reader.Close();
        cmd.Dispose();

        // Close database connection
        Database.Close();
    }

    private static long Create(ColorSchemeObj colorScheme)
    {
        if (Database.Connect() && colorScheme != null && colorScheme.Name.Length > 0)
        {
            if (!Exists(colorScheme))
            {
                // Insert color scheme into database
                var sql = "INSERT INTO color_scheme (name,viz_id,colors) VALUES (@Name, @Viz_ID, @Colors); " +
                          "SELECT last_insert_rowid()";
                var cmd = new SqliteCommand(sql, Database.Connection);

                // Add Parameters to statement
                cmd.Parameters.Add(new SqliteParameter("Name", colorScheme.Name));
                cmd.Parameters.Add(new SqliteParameter("Viz_ID", colorScheme.Visualization.Id));
                cmd.Parameters.Add(new SqliteParameter("Colors", FormatColors(colorScheme.Colors)));

                // Execute insert statement and get ID
                var id = (long) cmd.ExecuteScalar();

                // Close database connection
                cmd.Dispose();
                Database.Close();

                return id;
            }

            // Close database connection
            Database.Close();

            return (long) Database.Constants.DuplicateFound;
        }

        // Close database connection
        Database.Close();

        return (long) Database.Constants.QueryFailed;
    }

    private static long Edit(ColorSchemeObj colorScheme)
    {
        if (Database.Connect() && colorScheme != null && colorScheme.Name.Length > 0)
        {
            if (!Exists(colorScheme))
            {
                // Query statement
                var sql = "UPDATE color_scheme SET name = @Name, viz_id = @Viz_ID, colors = @Colors WHERE id = @ID";
                var cmd = new SqliteCommand(sql, Database.Connection);

                // Add Parameters to statement
                cmd.Parameters.Add(new SqliteParameter("Name", colorScheme.Name));
                cmd.Parameters.Add(new SqliteParameter("Viz_ID", colorScheme.Visualization.Id));
                cmd.Parameters.Add(new SqliteParameter("Colors", FormatColors(colorScheme.Colors)));
                cmd.Parameters.Add(new SqliteParameter("ID", colorScheme.Id));

                // Result
                var result = cmd.ExecuteNonQuery();

                // Close database connection
                cmd.Dispose();
                Database.Close();

                return (long) (result > 0 ? Database.Constants.Successful : Database.Constants.QueryFailed);
            }

            // Close database connection
            Database.Close();

            return (long) Database.Constants.DuplicateFound;
        }

        // Close database connection
        Database.Close();

        return (long) (colorScheme != null && colorScheme.Name.Length > 0
            ? Database.Constants.QueryFailed
            : Database.Constants.EmptyInputValue);
    }

    private static bool Delete(ColorSchemeObj colorScheme)
    {
        if (Database.Connect() && colorScheme != null && _colorSchemes.Count > 1)
        {
            // Query statement
            var sql = "DELETE FROM color_scheme WHERE id = @ID";
            var cmd = new SqliteCommand(sql, Database.Connection);

            // Add Parameters to statement
            cmd.Parameters.Add(new SqliteParameter("ID", colorScheme.Id));

            // Result
            var result = cmd.ExecuteNonQuery();

            // Close database connection
            cmd.Dispose();
            Database.Close();

            return result > 0;
        }

        // Close database connection
        Database.Close();

        return false;
    }

    public static bool Exists(ColorSchemeObj colorScheme)
    {
        var sql = "SELECT id FROM color_scheme WHERE name = @Name AND viz_id = @Viz_ID AND id != @ID";
        var cmd = new SqliteCommand(sql, Database.Connection);

        // Add Parameters to statement
        cmd.Parameters.Add(new SqliteParameter("Name", colorScheme.Name));
        cmd.Parameters.Add(new SqliteParameter("Viz_ID", colorScheme.Visualization.Id));
        cmd.Parameters.Add(new SqliteParameter("ID", colorScheme.Id));

        // Exit if color scheme name already exists
        var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            cmd.Dispose();
            reader.Close();

            return true;
        }

        // Dispose command
        cmd.Dispose();

        return false;
    }


    //-- HELPER METHODS

    public static string FormatColors(IEnumerable<Color> colors)
    {
        // Output
        var cols = new List<string>();

        foreach (var col in colors)
        {
            var c = new string [3];
            c[0] = col.r.ToString(CultureInfo.InvariantCulture);
            c[1] = col.g.ToString(CultureInfo.InvariantCulture);
            c[2] = col.b.ToString(CultureInfo.InvariantCulture);

            cols.Add(string.Join(",", c));
        }

        return cols.Count > 0 ? string.Join(";", cols.ToArray()) : null;
    }

    private static Color[] GetColors(string input)
    {
        // List with colors
        var colors = new List<Color>();

        // Plain colors
        var plain = input.Split(';');

        // Add colors
        foreach (var s in plain)
        {
            // Create color object
            var obj = new Color();

            // Split rgb values
            var rgb = s.Split(',');

            if (rgb.Length != 3) continue;

            // Get values
            obj.r = float.Parse(rgb[0]);
            obj.g = float.Parse(rgb[1]);
            obj.b = float.Parse(rgb[2]);
            obj.a = 1;

            // Add to list
            colors.Add(obj);
        }

        return colors.ToArray();
    }
}