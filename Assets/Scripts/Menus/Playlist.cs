/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mono.Data.Sqlite;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Playlist : MonoBehaviour
{
    // Dialog reference
    public Dialog Dialog;

    // File to add to new playlist
    public FileObj FileToAdd;

    // Last created playlist
    public long LastPlaylist;

    // Playlists
    public List<PlaylistObj> Playlists;
    public bool ShowingDialog;

    // Playlist to toggle
    public PlaylistObj TogglePlaylist;


    private void Start()
    {
        // Select playlists from database
        Load();

        // Set selected elements
        MenuFunctions.SetSelected();

        // Display playlists
        Display();

        // Close database connection
        Database.Close();

        // Show or hide start button
        MenuFunctions.ToggleStart();

        // Set start button
        if (Settings.Active.File != null)
            GameObject.Find("Start/Button/Text").GetComponent<Text>().text =
                Settings.MenuManager.LangManager.getString("continue");
    }

    private void Update()
    {
        // Toggle playlist
        if (TogglePlaylist != null)
        {
            ToggleFiles(TogglePlaylist, true);
            TogglePlaylist = null;
        }

        // Create playlist and add file
        if (FileToAdd != null && LastPlaylist > 0)
        {
            // Get last playlist
            var last = Playlists.Find(x => x.Id == LastPlaylist);

            if (last != null)
            {
                AddFile(FileToAdd, last);
                TogglePlaylist = last;
            }

            // Unset file to add
            FileToAdd = null;
            ShowingDialog = false;
        }

        if (FileToAdd != null && !ShowingDialog)
        {
            ShowingDialog = true;
            ShowDialog(Dialog.Type.PlaylistAdd);
        }

        // Reset last created playlist
        LastPlaylist = 0;
    }


    private void Display()
    {
        // Remove all GameObjects
        for (var i = transform.childCount - 1; i >= 0; i--) DestroyImmediate(transform.GetChild(i).gameObject);

        foreach (var p in Playlists)
        {
            // Display playlist
            DisplayPlaylist(p);

            // Display files
            foreach (var f in p.Files) DisplayFile(p, f);

            // Hide playlist files
            var contents = transform.Find("#" + p.Id + "/Contents");
            if (contents != null && !p.Equals(Settings.Selected.Playlist))
                contents.gameObject.SetActive(false);
        }
    }

    private void DisplayPlaylist(PlaylistObj playlist)
    {
        // Create GameObject
        var element = DisplayPlaylistOrFile(playlist, null);
        var text = element.transform.Find("Main/Text").gameObject.GetComponent<Text>();

        // Text settings
        text.text = playlist.Name;

        // Set scaling
        element.GetComponent<RectTransform>().localScale = Vector3.one;
        element.transform.Find("Main").GetComponent<RectTransform>().localScale = Vector3.one;

        // Get Event Trigger
        var events = element.GetComponent<EventTrigger>();

        // Add Click Event
        var evtClick = new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
        events.triggers.Add(evtClick);

        evtClick.callback.AddListener(eventData => { ToggleFiles(playlist); });

        // Add Event to Button
        element.transform.Find("Main/Text").gameObject.GetComponent<Button>().onClick
            .AddListener(delegate { ToggleFiles(playlist); });
    }

    private void DisplayFile(PlaylistObj playlist, FileObj file)
    {
        // Create GameObject
        var element = DisplayPlaylistOrFile(playlist, file);
        var text = element.transform.Find("Text").gameObject.GetComponent<Text>();

        // Text settings
        text.text = Path.GetFileName(file.Path);

        // Set scaling
        element.GetComponent<RectTransform>().localScale = Vector3.one;
        element.transform.parent.GetComponent<RectTransform>().localScale = Vector3.one;

        // Get Event Trigger
        var events = element.GetComponent<EventTrigger>();

        // Add Click Event
        var evtClick = new EventTrigger.Entry {eventID = EventTriggerType.PointerClick};
        events.triggers.Add(evtClick);

        evtClick.callback.AddListener(eventData => { UpdateSelectedFile(file, true); });

        // Add Event to Button
        element.transform.Find("Text").gameObject.GetComponent<Button>().onClick
            .AddListener(delegate { UpdateSelectedFile(file, true); });
    }

    private GameObject DisplayPlaylistOrFile(PlaylistObj playlist, FileObj file)
    {
        if (playlist == null) return null;

        // Navigation for buttons
        var nav = new Navigation {mode = Navigation.Mode.None};

        // Set name
        var title = "#" + playlist.Id + (file != null ? "." + file.Id : "");

        // Create main GameObject
        var main = new GameObject("Main");
        if (file != null) main.name = "Contents";

        // Check if Contents GameObject already exists
        var contents = transform.Find("#" + playlist.Id + "/Contents");
        var contentsExists = !ReferenceEquals(contents, null);

        if (contentsExists && file != null)
        {
            DestroyImmediate(main);
            main = contents.gameObject;
        }

        // Set parent of GameObject
        var parent = transform;
        if (file != null) parent = main.transform;

        // Create GameOject
        var element = new GameObject(title);
        element.transform.SetParent(parent);

        // Add Vertical Layout Group
        if (!contentsExists || file == null)
        {
            var vlg = (file == null ? element : main).AddComponent<VerticalLayoutGroup>();
            vlg.spacing = 20;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;
        }


        // Set GameObject for return
        var goReturn = element;

        // Set parent of main GameObject
        parent = element.transform;
        if (file != null) parent = transform.Find("#" + playlist.Id);
        main.transform.SetParent(parent);


        // Change GameObjects if file is displayed to inherit from different GameObject
        if (file != null) main = element;


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
        mainHlg.padding = new RectOffset(0, file == null ? 65 : 30, 0, 0);

        // Add Drop Handler script
        if (file == null) element.AddComponent<Image>().color = Color.clear;


        // Create arrow text GameObject
        var mainArrow = new GameObject("Arrow");
        mainArrow.transform.SetParent(main.transform);

        // Add text
        var mainTextArrow = mainArrow.AddComponent<TextUnicode>();
        mainTextArrow.color = Color.white;

        if (file == null)
            mainTextArrow.text = playlist.Equals(Settings.Selected.Playlist)
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


        // Create listening text GameObject
        var mainListening = new GameObject("Listening");
        if (file != null) mainListening.transform.SetParent(main.transform);

        // Add text
        var mainTextListening = mainListening.AddComponent<TextUnicode>();


        if (playlist.Equals(Settings.Active.Playlist) && (file == null || file.Equals(Settings.Active.File)))
        {
            mainTextListening.text = IconFont.Listening;
            mainTextListening.fontSize = 30;
            mainTextListening.color = file == null ? Color.white : Settings.GetColorFromRgb(180, 180, 180);
        }
        else if (file != null && playlist.Equals(Settings.Selected.Playlist) && file.Equals(Settings.Selected.File))
        {
            mainTextListening.text = IconFont.DropdownClosed;
            mainTextListening.fontSize = 20;
            mainTextListening.color = Color.gray;
        }

        // Set text alignment
        mainTextListening.alignment = file == null ? TextAnchor.MiddleRight : TextAnchor.MiddleLeft;

        // Font settings
        mainTextListening.font = IconFont.Font;

        // Add Layout Element
        var mainLayoutElementListening = mainListening.AddComponent<LayoutElement>();
        mainLayoutElementListening.minWidth = file == null ? 40 : 32;


        // Create text GameObject
        var mainText = new GameObject("Text");
        mainText.transform.SetParent(main.transform);

        // Add text
        var text = mainText.AddComponent<Text>();

        // Set text alignment
        text.alignment = TextAnchor.MiddleLeft;

        // Set text color
        if (file == null)
            text.color = Color.white;
        else if (playlist.Equals(Settings.Active.Playlist) && file.Equals(Settings.Active.File))
            text.color = Settings.GetColorFromRgb(180, 180, 180);
        else
            text.color = Color.gray;

        // Font settings
        text.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
        text.fontSize = 30;

        // Set transformations
        text.rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Add button
        var buttonText = mainText.AddComponent<Button>();
        buttonText.transition = Selectable.Transition.Animation;
        buttonText.navigation = nav;

        // Add animator
        var animatorText = mainText.AddComponent<Animator>();
        animatorText.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/MenuButtons");


        // Add listening element
        if (file == null) mainListening.transform.SetParent(main.transform);


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


        if (file == null)
        {
            // Create edit text GameObject
            var edit = new GameObject("Edit");
            edit.transform.SetParent(editIcons.transform);

            // Add text
            var editText = edit.AddComponent<TextUnicode>();
            editText.text = IconFont.Edit;
            editText.color = Color.white;

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
            buttonEditEvt.navigation = nav;

            // Add button onclick event
            buttonEditEvt.onClick.AddListener(delegate { ShowDialog(Dialog.Type.PlaylistEdit, element); });

            // Add animator
            var animatorEditEvt = edit.AddComponent<Animator>();
            animatorEditEvt.runtimeAnimatorController =
                Resources.Load<RuntimeAnimatorController>("Animations/MenuButtons");
        }


        // Create delete text GameObject
        var delete = new GameObject("Delete");
        delete.transform.SetParent(editIcons.transform);

        // Add text
        var deleteText = delete.AddComponent<Text>();
        deleteText.text = IconFont.Trash;
        deleteText.color = Color.white;

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
        buttonDeleteEvt.navigation = nav;

        // Add button onclick event
        buttonDeleteEvt.onClick.AddListener(delegate
        {
            if (FindFile(element) == null)
                ShowDialog(Dialog.Type.PlaylistDelete, element);
            else
                Delete(element);
        });

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


        return goReturn;
    }

    private void ToggleFiles(PlaylistObj playlist, bool forceOpen = false)
    {
        // Show or hide playlist files
        var opened = false;
        foreach (var p in Playlists)
        {
            // Get GameObject for current file
            var contents = transform.Find("#" + p.Id + "/Contents");
            var main = contents != null ? contents.gameObject : null;

            // Get arrow
            var arr = transform.Find("#" + p.Id + "/Main/Arrow").GetComponent<Text>();

            // Toggle files for GameObject
            if (main != null)
            {
                if (Equals(p, playlist) && p.Files.Count > 0)
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
            else if (Equals(p, playlist))
            {
                opened = true;
            }

            // Change arrows
            if (!forceOpen && !Equals(p, playlist)) arr.text = IconFont.DropdownClosed;
        }

        // Change arrow image
        var arrow = transform.Find("#" + playlist.Id + "/Main/Arrow").GetComponent<TextUnicode>();
        if (arrow != null) arrow.text = opened ? IconFont.DropdownOpened : IconFont.DropdownClosed;

        if (!opened && Settings.Selected.File != null)
        {
            // Remove icon from selected file
            var file = transform.Find("#" + Settings.Selected.Playlist.Id + "/Contents/#" +
                                      Settings.Selected.Playlist.Id + "." + Settings.Selected.File.Id + "/Listening");
            if (file != null) file.GetComponent<Text>().text = "";

            // Unset selected file
            Settings.Selected.File = null;
        }

        if (opened && !Equals(Settings.Selected.Playlist, playlist) || forceOpen)
        {
            // Set selected playlist
            Settings.Selected.Playlist = playlist;

            // Set selected file
            if (Settings.Selected.Playlist.Files.Count > 0)
                UpdateSelectedFile(Settings.Selected.Playlist.Files.First(),
                    Settings.Active.File == null || Settings.Active.Playlist == null
                                                 || Settings.Active.File != null &&
                                                 !Settings.Active.Playlist.Equals(Settings.Selected.Playlist));
            else
                Settings.Selected.File = null;
        }

        // Unset selected playlist
        if (!opened && !forceOpen) Settings.Selected.Playlist = null;

        // Scroll to top if scrollbar is hidden
        ScrollToTop();

        // Toggle start button
        MenuFunctions.ToggleStart();
    }

    private void UpdateSelectedFile(FileObj file, bool updateFile)
    {
        // Update selected file
        if (updateFile) Settings.Selected.File = file;

        // Re-display files and playlists
        Display();
    }

    private void ScrollToTop()
    {
        // Force canvas to update elements
        Canvas.ForceUpdateCanvases();

        // Scroll to top if scrollbar is not visible
        if (transform.GetComponent<RectTransform>().sizeDelta.y < 0)
            gameObject.transform.parent.GetComponent<ScrollRect>().verticalScrollbar.value = 1;
    }

    private long NewPlaylist(string title)
    {
        if (title.Length <= 0) return (long) Database.Constants.EmptyInputValue;

        // Create playlist object
        var playlist = new PlaylistObj(title);

        // Create object in database
        var id = Create(playlist);

        if (id <= 0) return id;

        // Reload playlists
        Load();
        Display();

        return id;
    }

    private long EditPlaylist(PlaylistObj playlist, string name)
    {
        if (name.Length <= 0) return (long) Database.Constants.EmptyInputValue;

        // Clone playlist
        var pl = (PlaylistObj) playlist.Clone();
        pl.Name = name;

        // Edit playlist
        var id = Edit(pl);

        if (id != (long) Database.Constants.Successful) return id;

        // Update playlist objects
        if (Settings.Active.Playlist != null && Settings.Active.Playlist.Equals(playlist))
            Settings.Active.Playlist.Name = name;
        if (Settings.Selected.Playlist != null && Settings.Selected.Playlist.Equals(playlist))
            Settings.Selected.Playlist.Name = name;

        // Reload playlists
        Load();
        Display();

        return id;
    }

    private bool Delete(GameObject element)
    {
        // Get file
        var playlist = FindPlaylist(element);
        var file = FindFile(element);

        var deleted = playlist != null && (file != null ? DeleteFile(playlist, file) : DeletePlaylist(playlist));

        if (!deleted) return false;

        // Unset selected file
        if (playlist.Files.Contains(Settings.Selected.File) && (file == null || file.Equals(Settings.Selected.File)))
            Settings.Selected.File = null;

        // Delete extant files
        var files = file == null ? playlist.Files.ToArray() : new[] {file};
        foreach (var f in files) DeleteExtantFile(f, playlist);

        if (file == null)
        {
            // Remove from list
            Playlists.Remove(playlist);

            // Unset active and opened playlist
            if (playlist.Equals(Settings.Active.Playlist)) Settings.Active.Playlist = null;
            if (playlist.Equals(Settings.Selected.Playlist)) Settings.Selected.Playlist = null;
        }
        else
        {
            // Remove files from playlists
            playlist.Files.Remove(file);
            if (playlist.Equals(Settings.Active.Playlist)) Settings.Active.Playlist.Files.Remove(file);
            if (playlist.Equals(Settings.Selected.Playlist)) Settings.Selected.Playlist.Files.Remove(file);

            // Toggle files
            if (playlist.Files.Count == 0) ToggleFiles(playlist);
        }

        // Display playlists
        Display();

        return true;
    }

    public PlaylistObj FindPlaylist(GameObject element)
    {
        if (element == null) return null;

        // Get playlist id
        var title = element.name.Split('.');
        var playlistId = long.Parse(title[0].Split('#')[1]);

        // Get playlist
        return Playlists.Find(x => x.Id == playlistId);
    }

    private FileObj FindFile(GameObject element)
    {
        if (element == null) return null;

        // Get playlist and file id
        var title = element.name.Split('.');
        var fileId = title.Length > 1 ? long.Parse(title[1]) : 0;

        // Get playlist
        var playlist = FindPlaylist(element);

        // Get file
        return playlist != null ? playlist.Files.Find(x => x.Id == fileId) : null;
    }

    public void ShowDialog(string type)
    {
        var typeInt = 0;
        if (int.TryParse(type, out typeInt) && Enum.IsDefined(typeof(Dialog.Type), typeInt))
        {
            ShowDialog((Dialog.Type) Enum.ToObject(typeof(Dialog.Type), typeInt));
        }
    }

    private void ShowDialog(Dialog.Type type, GameObject obj = null)
    {
        if (Dialog == null) return;

        // Playlist object
        var playlist = FindPlaylist(obj);

        // Button
        var button = Dialog.ButtonOk;
        var buttonText = Dialog.GetButtonText();

        // Remove listener
        button.onClick.RemoveAllListeners();


        switch (type)
        {
            // New playlist
            case Dialog.Type.PlaylistAdd:

                // UI elements
                Dialog.SetHeading(Settings.MenuManager.LangManager.getString("newPlaylist"));
                Dialog.SetInputField("", Settings.MenuManager.LangManager.getString("namePlaylist"));

                // Events
                button.onClick.AddListener(delegate
                {
                    // Create playlist
                    var result = NewPlaylist(Dialog.GetInputText().Trim());

                    // Handle database result
                    switch (result)
                    {
                        // Playlist name already taken
                        case (long) Database.Constants.DuplicateFound:

                            Dialog.SetInfo(Settings.MenuManager.LangManager.getString("existsPlaylist"));
                            break;

                        // Database query failed
                        case (long) Database.Constants.QueryFailed:

                            Dialog.SetInfo(Settings.MenuManager.LangManager.getString("noPlaylist"));
                            break;

                        // No user input
                        case (long) Database.Constants.EmptyInputValue:

                            Dialog.SetInfo(Settings.MenuManager.LangManager.getString("namePlaylist"));
                            break;

                        default:

                            Dialog.HideDialog();
                            break;
                    }
                });

                break;


            // Edit playlist
            case Dialog.Type.PlaylistEdit:

                if (playlist != null)
                {
                    // UI elements
                    Dialog.SetHeading(Settings.MenuManager.LangManager.getString("editPlaylist"));
                    Dialog.SetInputField(playlist.Name, playlist.Name);

                    // Events
                    button.onClick.AddListener(delegate
                    {
                        // Edit playlist
                        var result = EditPlaylist(playlist, Dialog.GetInputText().Trim());

                        // Handle database result
                        switch (result)
                        {
                            // Playlist name already taken
                            case (long) Database.Constants.DuplicateFound:

                                Dialog.SetInfo(Settings.MenuManager.LangManager.getString("existsPlaylist"));
                                break;

                            // Database query failed
                            case (long) Database.Constants.QueryFailed:

                                Dialog.SetInfo(Settings.MenuManager.LangManager.getString("noEditPlaylist"));
                                break;

                            // No user input
                            case (long) Database.Constants.EmptyInputValue:

                                Dialog.SetInfo(Settings.MenuManager.LangManager.getString("namePlaylist"));
                                break;

                            default:

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


            // Delete playlist
            case Dialog.Type.PlaylistDelete:

                if (playlist != null)
                {
                    // UI elements
                    Dialog.SetHeading(Settings.MenuManager.LangManager.getString("deletePlaylist"));
                    Dialog.SetText(Settings.MenuManager.LangManager.getString("sureDelete"));

                    // Events
                    button.onClick.AddListener(delegate
                    {
                        Delete(obj);

                        Load();
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

    private void Load()
    {
        if (Database.Connect())
        {
            // Reset playlists list
            Playlists = new List<PlaylistObj>();

            // Database command
            var cmd = new SqliteCommand(Database.Connection);

            // Query statement
            var sql = "SELECT id,name,files FROM playlist ORDER BY name ASC";
            cmd.CommandText = sql;

            // Get sql results
            var reader = cmd.ExecuteReader();

            // Read sql results
            while (reader.Read())
            {
                // Create playlist object
                var obj = new PlaylistObj(reader.GetString(1)) {Id = reader.GetInt64(0)};

                // Get file IDs
                var fileIDs = !reader.IsDBNull(2) ? reader.GetString(2).Split(',', ' ') : new string[0];

                // Select files
                var files = fileIDs.Select(id => GetFile(long.Parse(id), false))
                    .Where(file => file != null && File.Exists(file.Path)).ToList();

                // Sort files
                files.Sort((lhs, rhs) => string.Compare(Path.GetFileName(lhs.Path), Path.GetFileName(rhs.Path),
                    StringComparison.Ordinal));

                // Set files
                obj.Files = files;

                // Add contents to playlists array
                Playlists.Add(obj);
            }

            // Close reader
            reader.Close();
            cmd.Dispose();
        }

        // Close database connection
        Database.Close();
    }

    private long Create(PlaylistObj playlist)
    {
        if (Database.Connect() && playlist != null && playlist.Name.Length > 0)
        {
            // SQL settings
            SqliteCommand cmd;
            string sql;

            // Check if playlist files are already in database
            foreach (var file in playlist.Files)
            {
                // Query statement
                sql = "SELECT id FROM file WHERE path = @Path";
                cmd = new SqliteCommand(sql, Database.Connection);

                // Add Parameters to statement
                cmd.Parameters.Add(new SqliteParameter("Path", file.Path));

                // Get sql results
                var reader = cmd.ExecuteReader();

                // Read and add file IDs
                var count = 0;
                while (reader.Read())
                {
                    file.Id = reader.GetInt64(0);
                    count++;
                }

                // Close reader
                reader.Close();
                cmd.Dispose();

                // Add file to database if not already exists
                if (count != 0) continue;

                // Query statement
                sql = "INSERT INTO file (path) VALUES(@Path); " +
                      "SELECT last_insert_rowid()";
                cmd = new SqliteCommand(sql, Database.Connection);

                // Add Parameters to statement
                cmd.Parameters.Add(new SqliteParameter("Path", file.Path));

                // Execute statement
                file.Id = (long) cmd.ExecuteScalar();
                cmd.Dispose();
            }

            // Format file IDs
            var files = FormatFileIDs(playlist.Files);

            // Insert playlist into database
            try
            {
                sql = "INSERT INTO playlist (name,files) VALUES(@Name, @Files); " +
                      "SELECT last_insert_rowid()";
                cmd = new SqliteCommand(sql, Database.Connection);

                // Add Parameters to statement
                cmd.Parameters.Add(new SqliteParameter("Name", playlist.Name));
                cmd.Parameters.Add(new SqliteParameter("Files", files));

                // Execute insert statement and get ID
                var id = (long) cmd.ExecuteScalar();

                // Close database connection
                cmd.Dispose();
                Database.Close();

                // Set last created playlist
                LastPlaylist = id;

                return id;
            }
            catch (SqliteException)
            {
                // Close database connection
                Database.Close();

                return (long) Database.Constants.DuplicateFound;
            }
        }

        // Close database connection
        Database.Close();

        return (long) Database.Constants.QueryFailed;
    }

    private long Edit(PlaylistObj playlist)
    {
        if (Database.Connect() && playlist != null && playlist.Name.Length > 0)
            try
            {
                // Query statement
                var sql = "UPDATE playlist SET name = @Name, files = @Files WHERE id = @ID";
                var cmd = new SqliteCommand(sql, Database.Connection);

                // Add Parameters to statement
                cmd.Parameters.Add(new SqliteParameter("Name", playlist.Name));
                cmd.Parameters.Add(new SqliteParameter("Files", FormatFileIDs(playlist.Files)));
                cmd.Parameters.Add(new SqliteParameter("ID", playlist.Id));

                // Result
                var result = cmd.ExecuteNonQuery();

                // Close database connection
                cmd.Dispose();
                Database.Close();

                return (long) (result > 0 ? Database.Constants.Successful : Database.Constants.QueryFailed);
            }
            catch (SqliteException)
            {
                return (long) Database.Constants.DuplicateFound;
            }

        // Close database connection
        Database.Close();

        return (long) (playlist != null && playlist.Name.Length > 0
            ? Database.Constants.QueryFailed
            : Database.Constants.EmptyInputValue);
    }

    public long AddFile(FileObj file, PlaylistObj playlist)
    {
        if (Database.Connect() && file != null && playlist != null)
        {
            // Reset file ID
            file.Id = 0;

            // Update file ID: Query statement
            var sql = "SELECT id FROM file WHERE path = @Path";
            var cmd = new SqliteCommand(sql, Database.Connection);

            // Add Parameters to statement
            cmd.Parameters.Add(new SqliteParameter("Path", file.Path));

            // Get sql results
            var reader = cmd.ExecuteReader();

            // Read id
            while (reader.Read()) file.Id = reader.GetInt64(0);

            // Close reader
            reader.Close();
            cmd.Dispose();

            // Add file if ID is still not valid
            if (!(file.Id > 0))
            {
                // Query statement
                sql = "INSERT INTO file (path) VALUES (@Path); " +
                      "SELECT last_insert_rowid()";
                cmd = new SqliteCommand(sql, Database.Connection);

                // Add Parameters to statement
                cmd.Parameters.Add(new SqliteParameter("Path", file.Path));

                // Send query
                file.Id = (long) cmd.ExecuteScalar();
                cmd.Dispose();
            }

            if (playlist.Files.Contains(file)) return (long) Database.Constants.DuplicateFound;

            // Add file to playlist
            playlist.Files.Add(file);

            // Sort files
            playlist.Files.Sort((lhs, rhs) =>
                string.Compare(Path.GetFileName(lhs.Path), Path.GetFileName(rhs.Path), StringComparison.Ordinal));

            // Set file IDs
            var files = FormatFileIDs(playlist.Files);

            // Query statement
            sql = "UPDATE playlist SET files = @Files WHERE id = @ID";
            cmd = new SqliteCommand(sql, Database.Connection);

            // Add Parameters to statement
            cmd.Parameters.Add(new SqliteParameter("Files", files));
            cmd.Parameters.Add(new SqliteParameter("ID", playlist.Id));

            // Result
            var result = cmd.ExecuteNonQuery();

            // Close database connection
            cmd.Dispose();
            Database.Close();

            return (long) (result > 0 ? Database.Constants.Successful : Database.Constants.QueryFailed);
        }

        // Close database connection
        Database.Close();

        return (long) Database.Constants.QueryFailed;
    }

    private static FileObj GetFile(long id, bool closeConnection)
    {
        if (Database.Connect())
        {
            // Send database query
            var cmd = new SqliteCommand(Database.Connection) {CommandText = "SELECT id,path FROM file WHERE id = @ID"};

            // Add Parameters to statement
            cmd.Parameters.Add(new SqliteParameter("ID", id));

            var reader = cmd.ExecuteReader();
            FileObj file = null;

            // Read and add file
            while (reader.Read())
                file = new FileObj
                {
                    Id = reader.GetInt64(0),
                    Path = reader.GetString(1)
                };

            // Close reader
            reader.Close();
            cmd.Dispose();
            if (closeConnection) Database.Close();

            return file;
        }

        // Close database connection
        if (closeConnection) Database.Close();

        return null;
    }

    public FileObj GetFile(string path)
    {
        if (Database.Connect())
        {
            // Send database query
            var cmd = new SqliteCommand(Database.Connection)
            {
                CommandText = "SELECT id,path FROM file WHERE path = @File"
            };

            // Add Parameters to statement
            cmd.Parameters.Add(new SqliteParameter("File", path));

            var reader = cmd.ExecuteReader();
            FileObj file = null;

            // Read and add file
            while (reader.Read())
                file = new FileObj
                {
                    Id = reader.GetInt64(0),
                    Path = reader.GetString(1)
                };

            // Close reader
            reader.Close();
            cmd.Dispose();
            Database.Close();

            return file;
        }

        // Close database connection
        Database.Close();

        return null;
    }

    private static bool DeletePlaylist(PlaylistObj playlist)
    {
        if (Database.Connect() && playlist != null)
        {
            // Query statement
            var sql = "DELETE FROM playlist WHERE id = @ID";
            var cmd = new SqliteCommand(sql, Database.Connection);

            // Add Parameters to statement
            cmd.Parameters.Add(new SqliteParameter("ID", playlist.Id));

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

    private bool DeleteFile(PlaylistObj playlist, FileObj file)
    {
        if (Database.Connect() && playlist != null && file != null && playlist.Files.Contains(file))
        {
            // Select files of playlist
            var sql = "SELECT files FROM playlist WHERE id = @ID";
            var cmd = new SqliteCommand(sql, Database.Connection);

            // Add Parameters to statement
            cmd.Parameters.Add(new SqliteParameter("ID", playlist.Id));

            // Get sql results
            var reader = cmd.ExecuteReader();

            // Read file IDs
            List<string> fileIds = null;
            while (reader.Read())
                if (!reader.IsDBNull(0))
                    fileIds = new List<string>(reader.GetString(0).Split(',', ' '));

            // Close reader
            reader.Close();
            cmd.Dispose();

            if (fileIds != null && fileIds.Contains(file.Id.ToString()))
            {
                // Remove file
                fileIds.Remove(file.Id.ToString());

                // Query statement
                sql = "UPDATE playlist SET files = @Files WHERE id = @ID";
                cmd = new SqliteCommand(sql, Database.Connection);

                // Add Parameters to statement
                cmd.Parameters.Add(new SqliteParameter("Files", FormatFileIDs(fileIds)));
                cmd.Parameters.Add(new SqliteParameter("ID", playlist.Id));

                // Result
                var result = cmd.ExecuteNonQuery();

                // Close database connection
                cmd.Dispose();
                Database.Close();

                return result > 0;
            }
        }

        // Close database connection
        Database.Close();

        return false;
    }

    private void DeleteExtantFile(FileObj file, PlaylistObj exclude)
    {
        if (Database.Connect() && !IsFileUsed(file, exclude))
        {
            // Query statement
            var sql = "DELETE FROM file WHERE id = @ID";
            var cmd = new SqliteCommand(sql, Database.Connection);

            // Add Parameters to statement
            cmd.Parameters.Add(new SqliteParameter("ID", file.Id));

            // Send query
            cmd.ExecuteNonQuery();

            // Dispose command
            cmd.Dispose();
        }

        // Close database connection
        Database.Close();
    }

    private static bool IsFileUsed(FileObj file, PlaylistObj exclude)
    {
        if (file == null) return false;

        // Query statement
        var sql = "SELECT files FROM playlist WHERE files LIKE @Files" + (exclude != null ? " AND id != @ID" : "");
        var cmd = new SqliteCommand(sql, Database.Connection);

        // Add Parameters to statement
        cmd.Parameters.Add(new SqliteParameter("Files", file.Id));
        if (exclude != null) cmd.Parameters.Add(new SqliteParameter("ID", exclude.Id));

        // Get sql results
        var reader = cmd.ExecuteReader();

        // Read file IDs
        while (reader.Read())
        {
            if (reader.IsDBNull(0)) continue;

            // Get file IDs
            var fileIds = new List<string>(reader.GetString(0).Split(',', ' '));

            // Check if list contains file
            if (fileIds.Contains(file.Id.ToString())) return true;
        }

        return false;
    }


    //-- HELPER METHODS

    private string FormatFileIDs(List<string> fileIDs)
    {
        // Create FileObj list
        var files = new List<FileObj>(fileIDs.Count);
        foreach (var id in fileIDs)
        {
            var file = new FileObj {Id = long.Parse(id)};
            files.Add(file);
        }

        return FormatFileIDs(files);
    }

    private static string FormatFileIDs(IEnumerable<FileObj> files)
    {
        // Add IDs
        var ids = (from file in files where file.Id != 0 select file.Id.ToString()).ToList();
        return ids.Count > 0 ? string.Join(",", ids.ToArray()) : null;
    }
}