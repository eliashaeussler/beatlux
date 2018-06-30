/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SourceFolder : MonoBehaviour
{
    private const float DblClickWait = 0.5f;

    private static readonly string[] SupportedFormats =
    {
        ".mp3",
        ".ogg",
        ".wav",
        ".aif",
        ".aiff",
        ".m4a"
    };

    private static List<string> _sDirs;
    private static List<string> _sFiles;
    private static bool _searching;

    private Coroutine _dblClick;
    private GameObject _dblClickGo;
    private BackgroundThread _thread;


    private void Start()
    {
        // Set main path
        Settings.Source.Main = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);

        // Set current path
        if (Settings.Source.Current == null) Settings.Source.Current = Settings.Source.Main;

        // Display files and folders for main path
        Initialize(Directory.Exists(Settings.Source.Current) ? Settings.Source.Current : Settings.Source.Main);
    }

    private static void Initialize()
    {
        Initialize(Settings.Source.Current ?? Settings.Source.Main);
    }

    private static void Initialize(string path)
    {
        // path and objects are initialised
        Settings.Source.Current = path;

        // Get files and folders
        var directories = GetDirs(Settings.Source.Current);
        var files = GetFiles(Settings.Source.Current);

        // Show files and folders
        Display(directories, files, false);
    }


    private static void Display(IEnumerable<string> directories, IEnumerable<string> files, bool fromSearch)
    {
        // Delete all previous created GameObjects
        DestroyAll();

        // Scroll to top
        if (!fromSearch) GameObject.Find("Files").GetComponent<ScrollRect>().verticalScrollbar.value = 1;

        // Combine directories and folders
        var results = new List<string>(directories);
        var lastDirectory = results.Count;
        results.AddRange(files);

        // Get current GameObject
        var gameObject = GameObject.Find("FileContent");

        // Create item for ech entry in results
        foreach (var item in results)
        {
            // Test if item is directory
            var isDir = Directory.Exists(item);

            // Create GameObject
            var obj = new GameObject(item);
            obj.transform.SetParent(gameObject.transform);

            // Add Horizontal Layout Group
            var hlg = obj.AddComponent<HorizontalLayoutGroup>();
            hlg.spacing = 20;
            hlg.childAlignment = TextAnchor.MiddleLeft;
            hlg.childForceExpandWidth = false;
            hlg.childForceExpandHeight = true;

            // Set RectTransform
            var trans = obj.GetComponent<RectTransform>();
            trans.localScale = Vector3.one;


            // Create image GameObject
            var goImage = new GameObject("Image");
            goImage.transform.SetParent(obj.transform);

            // Add text
            var textImage = goImage.AddComponent<TextUnicode>();

            textImage.color = Settings.GetColorFromRgb(180, 180, 180);
            textImage.text = isDir ? IconFont.Folder : IconFont.Music;
            textImage.alignment = TextAnchor.MiddleLeft;
            textImage.font = IconFont.Font;
            textImage.fontSize = 30;

            // Add RectTransform
            var imageTrans = goImage.GetComponent<RectTransform>();
            imageTrans.localScale = Vector3.one;

            // Add Layout Element
            var imageLayout = goImage.AddComponent<LayoutElement>();
            imageLayout.minWidth = 30;


            // Create text GameObject
            var goText = new GameObject("Text");
            goText.transform.SetParent(obj.transform);

            // Add RectTransform element
            var textTrans = goText.AddComponent<RectTransform>();
            textTrans.pivot = new Vector2(0.5f, 0.5f);
            textTrans.localScale = Vector3.one;

            // Add Layout Element
            var layoutElement = goText.AddComponent<LayoutElement>();
            layoutElement.minHeight = 30;
            layoutElement.preferredHeight = 30;

            // Add Drag Handler
            if (!isDir) goText.AddComponent<DragHandler>();

            // Add Button
            var button = goText.AddComponent<Button>();
            button.transition = Selectable.Transition.Animation;

            var nav = new Navigation {mode = Navigation.Mode.None};
            button.navigation = nav;

            // Add OnClick Handler
            var currentItem = item;
            if (isDir)
            {
                button.onClick.AddListener(delegate { Initialize(currentItem); });
            }
            else
            {
                var currentFile = item;
                button.onClick.AddListener(delegate
                {
                    // Get reference to playlist object
                    var pl = GameObject.Find("PlaylistContent").GetComponent<Playlist>();

                    // Get file object if available
                    var file = pl.GetFile(currentItem);

                    // Get source folder object
                    var sf = GameObject.Find("FileContent").GetComponent<SourceFolder>();

                    if (!sf.DoubleClicked(goText)) return;

                    // Get drop area
                    var dropObj = GameObject.FindGameObjectWithTag("PlaylistDrop");

                    // Insert file
                    DropHandler.InsertFile(currentFile, dropObj, dropObj);
                });
            }

            // Add Animator
            var animator = goText.AddComponent<Animator>();
            animator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animations/MenuButtons");

            // Add Text
            var text = goText.AddComponent<Text>();

            text.color = Color.white;
            text.font = Resources.Load<Font>("Fonts/FuturaStd-Book");
            text.text = Path.GetFileName(item);
            text.fontSize = 30;
            text.alignment = TextAnchor.MiddleLeft;
        }
    }

    public void HistoryBack()
    {
//		// Get user folder
//		string userPath = Environment.GetFolderPath (Environment.SpecialFolder.Personal);
//
//		if (!Path.Equals (userPath, Settings.Source.Current))
//		{
        // Get new path
        var path = Path.GetFullPath(Path.Combine(Settings.Source.Current, @".."));

        // Display file contents
        Initialize(path);
//		}
    }

    private static void DestroyAll()
    {
        var entries = GameObject.Find("FileContent").transform;
        for (var i = entries.childCount - 1; i >= 0; i--) DestroyImmediate(entries.GetChild(i).gameObject);
    }

    private static IEnumerable<string> GetDirs(string path)
    {
        // Get directories
        var dirs = Directory.GetDirectories(path).Where(x =>
            (new DirectoryInfo(x).Attributes & FileAttributes.Hidden) == 0
        ).ToArray();

        var elements = new List<string>();
        foreach (var subDir in dirs)
            try
            {
                // Try to get files inside sub dir
                Directory.GetFiles(subDir);

                // Add sub dir to list
                elements.Add(subDir);
            }
            catch
            {
                // ignored
            }

        return elements;
    }

    private static IEnumerable<string> GetFiles(string Path)
    {
        // Get files
        var files = Directory.GetFiles(Path).Where(x =>
            (new FileInfo(x).Attributes & FileAttributes.Hidden) == 0
            && IsSupportedFile(x)
        ).ToArray();

        return files.ToList();
    }


    //-- FILE SEARCH

    public void SearchFiles(string s)
    {
        _searching = s.Length > 0;

        if (!_searching)
        {
            // Search done
            Invoke("HideProgress", 0.01f);
            Initialize();
        }
        else
        {
            // Reset results
            _sDirs = new List<string>();
            _sFiles = new List<string>();

            // Dispose current thread
            if (_thread != null && _thread.IsBusy) _thread.Abort();

            // Initalize thread
            _thread = new BackgroundThread {WorkerSupportsCancellation = true};
            _thread.DoWork += delegate
            {
                // Destroy elements
                MainThreadDispatcher.Instance().Enqueue(DestroyAll);

                // Get search results
                GetResults(s);

                // Display search results
                MainThreadDispatcher.Instance().Enqueue(delegate { Display(_sDirs, _sFiles, true); });

                // Hide progress
                MainThreadDispatcher.Instance().Enqueue(HideProgress);
            };

            // Run thread
            _thread.RunWorkerAsync();
        }
    }

    private static void GetResults(string pattern)
    {
        // Get results
        var path = Settings.Source.Current;
        FileSearch(path, pattern);
    }

    private static void FileSearch(string folder, string pattern)
    {
        if (Path.GetFileName(folder).IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0) _sDirs.Add(folder);

        // Get files
        var files = Directory.GetFiles(folder).Where(x =>
            (new FileInfo(x).Attributes & FileAttributes.Hidden) == 0
            && Path.GetFileName(x).IndexOf(pattern, StringComparison.OrdinalIgnoreCase) >= 0
            && IsSupportedFile(x)
        ).ToArray();

        // Add file if file name contains pattern
        foreach (var file in files) _sFiles.Add(file);

        // Get directories
        var dirs = Directory.GetDirectories(folder).Where(x =>
            (new DirectoryInfo(x).Attributes & FileAttributes.Hidden) == 0
        ).ToArray();

        // Jump into sub directory
        foreach (var dir in dirs) FileSearch(dir, pattern);
    }

    private static void HideProgress()
    {
        GameObject.Find("FileSearch/Input/Progress").SetActive(false);
    }


    //-- HELPER METHODS

    private static bool IsSupportedFile(string file)
    {
        return SupportedFormats.Contains(Path.GetExtension(file).ToLower());
    }

    private bool DoubleClicked(GameObject obj)
    {
        // Stop coroutine
        if (_dblClick != null) StopCoroutine(_dblClick);

        if (_dblClickGo != null && _dblClickGo == obj)
        {
            // Second click
            HideDoubleClickImmediate();
            return true;
        }

        // First click
        StartCoroutine(HideDoubleClick(obj));
        return false;
    }

    private IEnumerator HideDoubleClick(GameObject obj)
    {
        // Set clicked game object
        _dblClickGo = obj;

        // Wait
        yield return new WaitForSeconds(DblClickWait);

        // Hide double click
        HideDoubleClickImmediate();
    }

    private void HideDoubleClickImmediate()
    {
        _dblClickGo = null;
    }
}