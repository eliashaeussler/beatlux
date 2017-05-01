using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class Player : MonoBehaviour {

	// Color constants
	public static Color COLOR_DISABLED = new Color (0.7f, 0.7f, 0.7f);
	public static Color COLOR_ENABLED = Color.white;

	// Volume rotation constants
	public static int VOLUME_ROTATION_MAX = 137;



	// Audio source (component of Main Camera)
	public AudioSource audio;

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
	public Transform volume;
	public Slider volumeSlider;

	// Defines player states (set by user or default)
	private bool userChangedSlider = false;
	private bool continuePlay = true;
	private bool isShuffle = false;

	// Playlist
	private List<FileObj> files;
	private int position;
	


	void Start ()
	{
		// Set audio source
		audio = Camera.main.GetComponent<AudioSource> ();

		// Set visualization
		if (Settings.Active.Visualization != null) // TODO remove after debug
		visualization.text = Settings.Active.Visualization.Name;

		// Set UI
		ToggleShuffle (false);
		ToggleRepeat (false);

		// Set volume
		SetVolume (audio.volume);

		// Select first file
		position = GetFileIndex (position, -1);
		Next ();
	}

	void Update ()
	{
		// Update UI elements
		UpdatePlayButton ();
		UpdateSlider ();

		// Play next file
		if (!audio.isPlaying && continuePlay) {
			Next (); // TODO funktioniert beim Editor nicht -> evtl. on end event für audio source ?!
		}
	}



	private void SetPlaylist ()
	{
		// Instantiate list
		files = new List<FileObj> ();

		// Get files from active playlist
		if (Settings.Active.Playlist != null)
		{
			// Take all files from playlist
			foreach (FileObj file in Settings.Active.Playlist.Files) {
				files.Add (file);
			}

			// Set position in list
			if (Settings.Active.File != null && Settings.Active.Playlist.Files.Contains (Settings.Active.File)) {
				position = Settings.Active.Playlist.Files.IndexOf (Settings.Active.File);
			} else {
				position = 0;
			}
		}
	}

	private bool Play (FileObj file)
	{
		// Get audio resource
		WWW resource = new WWW ("file:///" + file.Path.Replace ('\\', '/').TrimStart (new char [] { '/' }));
		AudioClip clip = resource.audioClip;

		// Wait until file is loaded
		while (clip.loadState != AudioDataLoadState.Loaded)
		{
			if (clip.loadState == AudioDataLoadState.Failed) {
				return false;
			}
		}

		if (clip.length > 0)
		{
			// Update current audio source
			audio.clip = clip;

			// Reset time
			audio.time = 0;

			// Play audio
			audio.Play ();

			// Set as active file
			Settings.Active.File = file;

			// Set full time
			fullTime.text = FormatTime (audio.clip.length);

			// Update position
			position = files.IndexOf (file);

			// Get artists and title
			TagLib.File tags = TagLib.File.Create (file.Path);
			string artist = tags.Tag.FirstAlbumArtistSort;
			string title = tags.Tag.Title;

			// Set artists and title
			string output = "";
			if (artist != null && artist.Length > 0)
				output = artist + " – ";
			
			if (title != null && title.Length > 0) {
				output += title;
			} else {
				output += Path.GetFileNameWithoutExtension (file.Path);
			}

			this.artist.text = output;


			return true;
		}

		return false;
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
		if (audio != null && audio.isPlaying && audio.clip != null && audio.clip.length != 0 && !userChangedSlider)
		{
			// Update slider value
			timeline.value = audio.time / audio.clip.length;
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
		if (audio != null && audio.clip != null && audio.clip.length != 0 && userChangedSlider) {
			audio.time = value < 1 ? value * audio.clip.length : audio.clip.length - 0.1f;
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
		// Set volume
		audio.volume = value;

		// Update slider
		if (volumeSlider.value != value) volumeSlider.value = value;

		// Update UI
		volume.rotation = Quaternion.AngleAxis (value * VOLUME_ROTATION_MAX, Vector3.forward);
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

	public void ToggleRepeat () {
		ToggleRepeat (!audio.loop);
	}

	public void ToggleRepeat (bool state)
	{
		// Change loop
		audio.loop = state;

		// Update UI
		repeat.GetComponent<Text> ().color = audio.loop ? COLOR_ENABLED : COLOR_DISABLED;
	}

	public void ToggleShuffle () {
		ToggleShuffle (!isShuffle);
	}

	public void ToggleShuffle (bool state)
	{
		// Change shuffle
		isShuffle = state;

		// Update playlist
		if (isShuffle)
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

			// Set position
			position = files.IndexOf (Settings.Active.File);
		}
		else
		{
			// Set playlist
			SetPlaylist ();
		}

		// Update UI
		shuffle.GetComponent<Text> ().color = isShuffle ? COLOR_ENABLED : COLOR_DISABLED;
	}



	//-- PLAYER CONTROLS

	// Play next clip
	public void Next ()
	{
		if (files.Count > 0)
		{
			bool found = false;
			int tempPos = position;

			// Try to play next file
			while (!found)
			{
				// Get next element in list
				tempPos = GetFileIndex (tempPos, 1);

				// Play next element in list
				found = Play (files [tempPos]);
				if (tempPos == position) break;
			}
		}
	}

	// Play previous clip
	public void Previous ()
	{
		if (files.Count > 0)
		{
			bool found = false;
			int tempPos = position;

			// Try to play previous file
			while (!found)
			{
				// Get previous element in list
				tempPos = GetFileIndex (tempPos, -1);

				// Play previous element in list
				found = Play (files [tempPos]);
				if (tempPos == position) break;
			}
		}
	}

	public void Shuffle ()
	{
		
	}
	
	
	
	//-- HELPER METHODS
	
	public string FormatTime (float value)
	{
		TimeSpan ts = TimeSpan.FromSeconds (value);
		return ts.Minutes.ToString () + ":" + ts.Seconds.ToString ().PadLeft (2, '0');
	}

	public int GetFileIndex (int position, int step)
	{
		if (files.Count > 0)
		{
			// Next file
			if (step == 1) return position < files.Count-1 ? position+1 : 0;

			// Previous file
			else if (step == -1) return position > 0 ? position-1 : files.Count-1;
		}

		return position;
	}

}