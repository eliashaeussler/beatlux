using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	// Color constants
	public static Color COLOR_DISABLED = new Color (0.4f, 0.4f, 0.4f);
	public static Color COLOR_ENABLED = Color.white;

	// Volume rotation constants
	public static int VOLUME_ROTATION_MAX = 137;

	// Color rotation constants
	public static int COLOR_ROTATION_MAX = 138;



	// Audio source and clip
	public AudioSource audio;
	private AudioClip clip;

	// UI elements
	public GameObject play;
	public GameObject pause;
	public GameObject previous;
	public GameObject next;
	public GameObject shuffle;
	public GameObject repeat;
	public Text currentTime;
	public Text fullTime;
	public Text artist;
	public Text visualization;
	public Slider timeline;
	public Transform volumeCircle;
	public Slider volumeSlider;
	public Transform colorCircle;
	public Transform colorElement;

	// Defines player states (set by user or default)
	private bool userChangedSlider = false;
	private bool continuePlay = true;

	// Files
	private List<FileObj> files;
	private int oldPos;
	private int position;

	// Shuffle state
	private bool isShuffled = false;
	


	void Start ()
	{
		SetVolume (Settings.Player.Volume);
	}

	void Update ()
	{
		// Set visualization and colors from color scheme
		if (Settings.Active.Visualization != null)
		{
			visualization.text = Settings.Active.Visualization.Name;
			SetColors ();
		}

		// Update UI elements
		ToggleRepeat (Settings.Player.RepeatMode);
		UpdatePlayButton ();
		UpdateSlider ();

		// Reset old position and shuffle state if playlist has changed
		if (Settings.Selected.Playlist != null && !Settings.Selected.Playlist.Equals (Settings.Active.Playlist)) {

			oldPos = -1;
			isShuffled = false;
		}

		// Check for updated playlist or file
		if (Settings.Selected.Playlist != null || (Settings.Selected.File != null && !Settings.Selected.File.Equals (Settings.Active.File)) || files == null) {

			ToggleShuffle (Settings.Player.Shuffle);
		}

		// Play next file
		if (Settings.Selected.File == null && files.Count > 0 && audio.clip != null && !audio.isPlaying && continuePlay) {
			
			Next ();
		}

		// Stop playing continously
		continuePlay = audio.isPlaying;

		// Set current position
		if (Settings.Selected.File != null) {
			position = files.IndexOf (Settings.Selected.File);
		} else if (Settings.Active.File != null) {
			position = files.IndexOf (Settings.Active.File);
		}

		// Play file if current has changed
		if ((Settings.Active.File != null && !Settings.Active.File.Equals (files [position])) ||
			(oldPos != position && position >= 0 && Settings.Selected.File != null
			&& !Settings.Selected.File.Equals (Settings.Active.File))) {

			Play ();
		}
	}



	private void SetFiles ()
	{
		// Instantiate list
		files = new List<FileObj> ();

		// Set active playlist
		if (Settings.Selected.Playlist != null) {
			Settings.Active.Playlist = Settings.Selected.Playlist;
		}

		// Get files from active playlist
		if (Settings.Active.Playlist != null)
		{
			foreach (FileObj file in Settings.Active.Playlist.Files) {
				files.Add (file);
			}
		}

		// Reset selected playlist
		Settings.Selected.Playlist = null;
	}

	private void Play ()
	{
		if (File.Exists (Settings.Selected.File.Path))
		{
			// Set active file
			Settings.Active.File = Settings.Selected.File;

			// Reset selected file
			Settings.Selected.File = null;

			// Get active file
			FileObj file = Settings.Active.File;

			if (file != null)
			{
				// Play file
				if (Path.GetExtension (file.Path) == ".mp3")
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
				}
			}
		}
		else
		{
			// Try to play next file
			Next ();
		}
	}

	private void StartPlay ()
	{
		// Set old position
		oldPos = position;

		// Update current audio source
		audio.clip = clip;

		// Reset time
		audio.time = 0;

		// Play audio
		audio.Play ();

		// Set full time
		fullTime.text = FormatTime (audio.clip.length);

		// Set artist and title
		string output = "";
		if (Settings.Active.File != null)
		{
			// Get artists and title
			TagLib.File tags = TagLib.File.Create (Settings.Active.File.Path);
			string artist = tags.Tag.FirstPerformer;
			string title = tags.Tag.Title;

			// Set artists and title
			if (artist != null && artist.Length > 0)
				output = artist + " – ";

			if (title != null && title.Length > 0) {
				output += title;
			} else {
				output += Path.GetFileNameWithoutExtension (Settings.Active.File.Path);
			}
		}

		this.artist.text = output;
	}

	public void ToggleShuffle () {
		ToggleShuffle (!Settings.Player.Shuffle);
	}

	public void ToggleShuffle (bool state)
	{
		// Change shuffle
		Settings.Player.Shuffle = state;

		// Set files
		if (files == null || !Settings.Player.Shuffle
			|| (Settings.Selected.Playlist != null && !Settings.Selected.Playlist.Equals (Settings.Active.Playlist))) {

			SetFiles ();
		}

		// Update playlist
		if (Settings.Player.Shuffle && !isShuffled)
		{
			// Re-order files
			System.Random rand = new System.Random ();
			int n = files.Count;
			while (n > 1) {
				n--;
				int k = rand.Next (n + 1);
				FileObj val = files [k];
				files [k] = files [n];
				files [n] = val;
			}
		}

		// Set shuffle
		isShuffled = Settings.Player.Shuffle;

		// Update UI
		shuffle.GetComponent<Text> ().color = Settings.Player.Shuffle ? COLOR_ENABLED : COLOR_DISABLED;
	}

	private void UpdatePlayButton ()
	{
		if (audio != null) {
			play.SetActive (!audio.isPlaying);
			pause.SetActive (audio.isPlaying);
		}
	}

	private void UpdateSlider ()
	{
		if (audio != null && audio.isPlaying
			&& audio.clip != null
			&& audio.clip.length != 0 && !userChangedSlider) {

			// Update slider value
			timeline.value = audio.time
				/ audio.clip.length;
		}
	}

	public void SetUserChanged (bool value)
	{
		userChangedSlider = value;
	}

	public void UpdateAudio ()
	{
		if (timeline != null) {
			SetAudioTime (timeline.value);
		}
	}

	private void SetAudioTime (float value)
	{
		if (audio != null && audio.clip != null
			&& audio.clip.length != 0 && userChangedSlider) {

			audio.time = value < 1
				? value * audio.clip.length
				: audio.clip.length - 0.1f;
		}
	}

	public void UpdateTime (float value)
	{
		if (audio != null && audio.clip != null && audio.clip.length > 0) {
			currentTime.text = FormatTime (value * audio.clip.length);
		} else {
			timeline.value = 0;
		}
	}

	public void SetVolume (float value)
	{
		if (audio.volume != value)
		{
			// Set volume
			audio.volume = value;

			// Update slider
			if (volumeSlider.value != value) volumeSlider.value = value;

			// Update UI
			volumeCircle.localRotation = Quaternion.AngleAxis (value * VOLUME_ROTATION_MAX, Vector3.forward);
		}
	}

	public void SetColors ()
	{
		if ( (Settings.Active.Visualization != null && Settings.Active.ColorScheme != null)
			|| (Settings.Active.Visualization.Equals (Settings.Defaults.Visualization)) )
		{
			// Get colors from color scheme
			Color[] colors = Settings.Active.Visualization.Equals (Settings.Defaults.Visualization)
				? new Color[] { Color.white }
				: Settings.Active.ColorScheme.Colors;

			// Add or remove color elements
			if (colorCircle.childCount < colors.Length)
			{
				for (int i=0; i < colors.Length - colorCircle.childCount; i++)
				{
					Instantiate (colorElement, colorCircle);
				}
			}
			else if (colorCircle.childCount > colors.Length)
			{
				for (int i=colorCircle.childCount-1; i >= colors.Length; i--)
				{
					GameObject.DestroyImmediate (colorCircle.GetChild (i).gameObject);
				}
			}

			// Set colors and rotation
			float angle = (float) COLOR_ROTATION_MAX / colors.Length;
			for (int i=0; i < colors.Length && i < colorCircle.childCount; i++)
			{
				Transform child = colorCircle.GetChild (i);
				child.GetComponent<Image> ().color = colors [i];
				float rotation = (colors.Length - i) * -angle;
				child.GetComponent<RectTransform> ().localRotation = Quaternion.AngleAxis (rotation, Vector3.forward);
			}
		}
	}

	public void TogglePlay ()
	{
		if (audio.isPlaying)
		{
			// Pause
			audio.Pause ();
			continuePlay = false;
		}
		else
		{
			// Play
			audio.UnPause ();
			continuePlay = true;
		}
	
	}

	public void ToggleRepeat ()
	{
		if (Settings.Player.RepeatMode < 1) {
			Settings.Player.RepeatMode++;
		} else {
			Settings.Player.RepeatMode = -1;
		}
		ToggleRepeat (Settings.Player.RepeatMode);
	}

	public void ToggleRepeat (int mode)
	{
		// Change loop
		Settings.Player.RepeatMode = mode;
		audio.loop = mode == 1;

		// Update UI
		Text text = repeat.GetComponent<Text> ();
		text.text = mode == 1
			? IconFont.REPEAT_SINGLE
			: IconFont.REPEAT;
		text.color = mode == -1
			? COLOR_DISABLED
			: COLOR_ENABLED;
	}



	//-- PLAYER CONTROLS

	// Play next clip
	public void Next ()
	{
		if (position < files.Count - 1 || (position == files.Count - 1 && (Settings.Player.RepeatMode == 0 || !continuePlay)))
		{
			continuePlay = true;
			Settings.Selected.File = GetFile (1);
		}
	}

	// Play previous clip
	public void Previous ()
	{
		continuePlay = true;
		Settings.Selected.File = GetFile (-1);
	}
	
	
	
	//-- HELPER METHODS
	
	public string FormatTime (float value)
	{
		TimeSpan ts = TimeSpan.FromSeconds (value);
		return ts.Minutes.ToString () + ":" + ts.Seconds.ToString ().PadLeft (2, '0');
	}

	public FileObj GetFile (int step)
	{
		int pos = position;

		if (files.Count > 0)
		{
			// Next file
			if (step == 1) pos = position < files.Count-1 ? position+1 : 0;

			// Previous file
			else if (step == -1) pos = position > 0 ? position-1 : files.Count-1;
		}

		return files [pos];
	}

}