/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System;
using System.Collections.Generic;
using System.IO;
using NAudio.Wave;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Player : MonoBehaviour
{
    // Volume rotation constants
    private const int VolumeRotationMax = 137;

    // Color rotation constants
    private const int ColorRotationMax = 138;

    // Color constants
    public static readonly Color ColorDisabled = new Color(0.4f, 0.4f, 0.4f);
    public static readonly Color ColorEnabled = Color.white;
    private AudioClip _clip;
    private bool _continuePlay = true;

    // Files
    private List<FileObj> _files;

    // Shuffle state
    private bool _isShuffled;
    private int _oldPos;
    private int _position;

    // Defines player states (set by user or default)
    private bool _userChangedSlider;
    public Text Artist;


    // Audio source and clip
    public AudioSource Audio;
    public Transform ColorCircle;
    public Transform ColorElement;
    public Text CurrentTime;
    public Text FullTime;
    public GameObject NextButton;
    public GameObject PauseButton;

    // UI elements
    public GameObject PlayButton;
    public GameObject PreviousButton;
    public GameObject RepeatButton;
    public GameObject ShuffleButton;
    public Slider Timeline;
    public Text Visualization;
    public Transform VolumeCircle;
    public Slider VolumeSlider;


    private void Start()
    {
        SetVolume(Settings.Player.Volume);
    }

    public void Update()
    {
        // Set visualization and colors from color scheme
        if (Settings.Active.Visualization != null)
        {
            Visualization.text = Settings.Active.Visualization.Name;
            SetColors();
        }

        // Update UI elements
        ToggleRepeat(Settings.Player.RepeatMode);
        UpdatePlayButton();
        UpdateSlider();

        // Reset old position and shuffle state if playlist has changed
        if (Settings.Selected.Playlist != null && !Settings.Selected.Playlist.Equals(Settings.Active.Playlist))
        {
            _oldPos = -1;
            _isShuffled = false;
        }

        // Check for updated playlist or file
        if (Settings.Selected.Playlist != null ||
            Settings.Selected.File != null && !Settings.Selected.File.Equals(Settings.Active.File) ||
            _files == null) ToggleShuffle(Settings.Player.Shuffle);

        // Play next file
        if (Settings.Selected.File == null && _files.Count > 0 && Audio.clip != null && !Audio.isPlaying &&
            _continuePlay) Next();

        // Stop playing continously
        _continuePlay = Audio.isPlaying;

        // Set current position
        if (Settings.Selected.File != null)
            _position = _files.IndexOf(Settings.Selected.File);
        else if (Settings.Active.File != null) _position = _files.IndexOf(Settings.Active.File);

        // Play file if current has changed
        if (Settings.Active.File != null && !Settings.Active.File.Equals(_files[_position]) ||
            _oldPos != _position && _position >= 0 && Settings.Selected.File != null
            && !Settings.Selected.File.Equals(Settings.Active.File))
            Play();
    }


    private void SetFiles()
    {
        // Instantiate list
        _files = new List<FileObj>();

        // Set active playlist
        if (Settings.Selected.Playlist != null) Settings.Active.Playlist = Settings.Selected.Playlist;

        // Get files from active playlist
        if (Settings.Active.Playlist != null)
            foreach (var file in Settings.Active.Playlist.Files)
                _files.Add(file);

        // Reset selected playlist
        Settings.Selected.Playlist = null;
    }

    private void Play()
    {
        if (File.Exists(Settings.Selected.File.Path))
        {
            // Set active file
            Settings.Active.File = Settings.Selected.File;

            // Reset selected file
            Settings.Selected.File = null;

            // Get active file
            var file = Settings.Active.File;

            if (file == null) return;

            switch (Path.GetExtension(file.Path))
            {
                case ".mp3":
                    _clip = MP3Import.StartImport(file.Path);
                    StartPlay();

                    break;


                case ".m4a":
                    using (var reader = new MediaFoundationReader(file.Path))
                    {
                        using (var resampledReader = new ResamplerDmoStream(reader,
                            new WaveFormat(reader.WaveFormat.SampleRate, reader.WaveFormat.BitsPerSample,
                                reader.WaveFormat.Channels)))
                        {
                            // @TODO ...
                        }
                    }

                    break;


                default:
                    // Get audio resource
                    var resource = new WWW("file:///" + file.Path.Replace('\\', '/').TrimStart('/'));
                    _clip = resource.GetAudioClip(true, false);

                    // Wait until file is loaded
                    while (_clip.loadState != AudioDataLoadState.Loaded)
                    {
                        if (_clip.loadState != AudioDataLoadState.Failed) continue;

                        Next();
                        return;
                    }

                    if (_clip != null && _clip.length > 0)
                        StartPlay();
                    else
                        Next();

                    break;
            }


            // Play file
            /*if (Path.GetExtension (file.Path) == ".mp3")
                {
                    clip = MP3Import.StartImport (file.Path);
                    StartPlay ();
                }
                else
                {
                    // Get audio resource
                    WWW resource = new WWW ("file:///" + file.Path.Replace ('\\', '/').TrimStart (new char [] { '/' }));
                    clip = resource.GetAudioClip (true, false);

                    // Wait until file is loaded
                    while (clip.loadState != AudioDataLoadState.Loaded)
                    {
                        if (clip.loadState == AudioDataLoadState.Failed) {
                            Next ();
                            return;
                        }
                    }

                    if (clip != null && clip.length > 0) {
                        StartPlay ();
                    } else {
                        Next ();
                    }
                }*/
        }
        else
        {
            // Try to play next file
            Next();
        }
    }

    private void StartPlay()
    {
        // Set old position
        _oldPos = _position;

        // Update current audio source
        Audio.clip = _clip;

        // Reset time
        Audio.time = 0;

        // Play audio
        Audio.Play();

        // Set full time
        FullTime.text = FormatTime(Audio.clip.length);

        // Set artist and title
        var output = "";
        if (Settings.Active.File != null)
        {
            // Get artists and title
            var tags = TagLib.File.Create(Settings.Active.File.Path);
            var performer = tags.Tag.FirstPerformer;
            var title = tags.Tag.Title;

            // Set artists and title
            if (!string.IsNullOrEmpty(performer))
                output = performer + " – ";

            if (!string.IsNullOrEmpty(title))
                output += title;
            else
                output += Path.GetFileNameWithoutExtension(Settings.Active.File.Path);
        }

        Artist.text = output;
    }

    public void ToggleShuffle()
    {
        ToggleShuffle(!Settings.Player.Shuffle);
    }

    private void ToggleShuffle(bool state)
    {
        // Change shuffle
        Settings.Player.Shuffle = state;

        // Set files
        if (_files == null || !Settings.Player.Shuffle
                           || Settings.Selected.Playlist != null &&
                           !Settings.Selected.Playlist.Equals(Settings.Active.Playlist))
            SetFiles();

        // Update playlist
        if (Settings.Player.Shuffle && !_isShuffled)
        {
            // Re-order files
            var rand = new Random();
            var n = _files.Count;
            while (n > 1)
            {
                n--;
                var k = rand.Next(n + 1);
                var val = _files[k];
                _files[k] = _files[n];
                _files[n] = val;
            }
        }

        // Set shuffle
        _isShuffled = Settings.Player.Shuffle;

        // Update UI
        ShuffleButton.GetComponent<Text>().color = Settings.Player.Shuffle ? ColorEnabled : ColorDisabled;
    }

    private void UpdatePlayButton()
    {
        if (Audio == null) return;

        PlayButton.SetActive(!Audio.isPlaying);
        PauseButton.SetActive(Audio.isPlaying);
    }

    private void UpdateSlider()
    {
        if (Audio != null && Audio.isPlaying
                          && Audio.clip != null
                          && Math.Abs(Audio.clip.length) > 0 && !_userChangedSlider)
            Timeline.value = Audio.time
                             / Audio.clip.length;
    }

    public void SetUserChanged(bool value)
    {
        _userChangedSlider = value;
    }

    public void UpdateAudio()
    {
        if (Timeline != null) SetAudioTime(Timeline.value);
    }

    private void SetAudioTime(float value)
    {
        if (Audio != null && Audio.clip != null
                          && Math.Abs(Audio.clip.length) > 0 && _userChangedSlider)
            Audio.time = value < 1
                ? value * Audio.clip.length
                : Audio.clip.length - 0.1f;
    }

    public void UpdateTime(float value)
    {
        if (Audio != null && Audio.clip != null && Audio.clip.length > 0)
            CurrentTime.text = FormatTime(value * Audio.clip.length);
        else
            Timeline.value = 0;
    }

    public void SetVolume(float value)
    {
        if (Math.Abs(Audio.volume - value) <= 0) return;

        // Set volume
        Audio.volume = value;

        // Update slider
        if (Math.Abs(VolumeSlider.value - value) > 0) VolumeSlider.value = value;

        // Update UI
        VolumeCircle.localRotation = Quaternion.AngleAxis(value * VolumeRotationMax, Vector3.forward);
    }

    private void SetColors()
    {
        if (Settings.Active.Visualization != null &&
            (Settings.Active.Visualization == null || Settings.Active.ColorScheme == null) &&
            !Settings.Active.Visualization.Equals(Settings.Defaults.Visualization)) return;

        if (Settings.Active.ColorScheme == null) return;

        // Get colors from color scheme
        var colors = Settings.Active.Visualization != null && Settings.Active.Visualization.Equals(Settings.Defaults.Visualization)
            ? new[] {Color.white}
            : Settings.Active.ColorScheme.Colors;

        // Add or remove color elements
        if (ColorCircle.childCount < colors.Length)
            for (var i = 0; i < colors.Length - ColorCircle.childCount; i++)
                Instantiate(ColorElement, ColorCircle);
        else if (ColorCircle.childCount > colors.Length)
            for (var i = ColorCircle.childCount - 1; i >= colors.Length; i--)
                DestroyImmediate(ColorCircle.GetChild(i).gameObject);

        // Set colors and rotation
        var angle = (float) ColorRotationMax / colors.Length;
        for (var i = 0; i < colors.Length && i < ColorCircle.childCount; i++)
        {
            var child = ColorCircle.GetChild(i);
            child.GetComponent<Image>().color = colors[i];
            var rotation = (colors.Length - i) * -angle;
            child.GetComponent<RectTransform>().localRotation = Quaternion.AngleAxis(rotation, Vector3.forward);
        }
    }

    public void TogglePlay()
    {
        if (Audio.isPlaying)
        {
            // Pause
            Audio.Pause();
            _continuePlay = false;
        }
        else
        {
            // Play
            Audio.UnPause();
            _continuePlay = true;
        }
    }

    public void ToggleRepeat()
    {
        if (Settings.Player.RepeatMode < 1)
            Settings.Player.RepeatMode++;
        else
            Settings.Player.RepeatMode = -1;
        ToggleRepeat(Settings.Player.RepeatMode);
    }

    private void ToggleRepeat(int mode)
    {
        // Change loop
        Settings.Player.RepeatMode = mode;
        Audio.loop = mode == 1;

        // Update UI
        var text = RepeatButton.GetComponent<Text>();
        text.text = mode == 1
            ? IconFont.RepeatSingle
            : IconFont.Repeat;
        text.color = mode == -1
            ? ColorDisabled
            : ColorEnabled;
    }


    //-- PLAYER CONTROLS

    // Play next clip
    private void Next()
    {
        if (_position >= _files.Count - 1 &&
            (_position != _files.Count - 1 || Settings.Player.RepeatMode != 0 && _continuePlay)) return;

        _continuePlay = true;
        Settings.Selected.File = GetFile(1);
    }

    // Play previous clip
    public void Previous()
    {
        _continuePlay = true;
        Settings.Selected.File = GetFile(-1);
    }


    //-- HELPER METHODS

    private static string FormatTime(float value)
    {
        var ts = TimeSpan.FromSeconds(value);
        return ts.Minutes + ":" + ts.Seconds.ToString().PadLeft(2, '0');
    }

    private FileObj GetFile(int step)
    {
        var pos = _position;

        if (_files.Count <= 0) return _files[pos];

        if (step == 1)
            pos = _position < _files.Count - 1 ? _position + 1 : 0;
        else if (step == -1) pos = _position > 0 ? _position - 1 : _files.Count - 1;

        return _files[pos];
    }
}