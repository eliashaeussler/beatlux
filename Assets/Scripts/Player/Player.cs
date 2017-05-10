using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using NLayer;

public class Player : MonoBehaviour {

	// Color constants
	public static Color COLOR_DISABLED = new Color (0.4f, 0.4f, 0.4f);
	public static Color COLOR_ENABLED = Color.white;

	// Volume rotation constants
	public static int VOLUME_ROTATION_MAX = 137;



	// Player canvas
	public PlayerCanvas canvas;

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
	public Transform volumeLeft;
	public Transform volumeRight;
	public Slider volumeSliderLeft;
	public Slider volumeSliderRight;

	// Defines player states (set by user or default)
	private bool userChangedSlider = false;
	private bool continuePlay = true;

	// Playlist
	private List<FileObj> files;
	private int position;

	// MP3 reading
	private bool mp3Reading = false;
	


	void Start ()
	{
		// Set audio source
		audio = Settings.MenuManager.audio;

		// Set volume
		SetVolume (Settings.Player.Volume);

		// Set null position
		position = -1;
	}

	void Update ()
	{
		// Set visualization
		if (Settings.Active.Visualization != null) {
			visualization.text = Settings.Active.Visualization.Name;
		}

		// Update UI elements
		ToggleRepeat (Settings.Player.Repeat);
		UpdatePlayButton ();
		UpdateSlider ();

		// Current states
		bool playlistChanged = Settings.Selected.Playlist != null && !Settings.Selected.Playlist.Equals (Settings.Active.Playlist);
		bool fileChanged = Settings.Selected.File != null && !Settings.Selected.File.Equals (Settings.Active.File);

		// Play first file if playlist was changed
		if (playlistChanged) {
			SetPlaylist ();
			ToggleShuffle (Settings.Player.Shuffle);
		}

		// Play file if selected file was changed
		if (fileChanged || playlistChanged) {
			Play (Settings.Selected.File);
		}

		// Play next file
		if (!audio.isPlaying && continuePlay && !mp3Reading) {
			Next ();
		}

		// Show player if mouse moves
		if (Input.mousePosition.y <= transform.position.y && (Input.GetAxis ("Mouse X") != 0 || Input.GetAxis ("Mouse Y") != 0)) {
			canvas.KeepPlayer ();
		}
	}



	private void SetPlaylist ()
	{
		// Instantiate list
		files = new List<FileObj> ();

		// Set active playlist
		if (Settings.Selected.Playlist != null) {
			Settings.Active.Playlist = Settings.Selected.Playlist;
			Settings.Selected.Playlist = null;
		}

		// Get files from active playlist
		if (Settings.Active.Playlist != null)
		{
			foreach (FileObj file in Settings.Active.Playlist.Files) {
				if (File.Exists (file.Path)) {
					files.Add (file);
				}
			}
		}
	}

	private bool Play (FileObj file)
	{
		if (File.Exists (file.Path))
		{
			if (Path.GetExtension (file.Path) == ".mp3")
			{
				// Get mp3 file
				MpegFile mp3 = new MpegFile (file.Path);
				int samples = (int) (mp3.Length / mp3.Channels) / sizeof (float);

				// Create audio clip
				clip = AudioClip.Create (Path.GetFileName (file.Path), samples, mp3.Channels, mp3.SampleRate, false);

				// Read samples
				StartCoroutine (ReadSamples (file, mp3, samples));

				if (clip != null && samples > 0)
				{
					artist.text = "Wird geladen...";
					return true;
				}
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
						return false;
					}
				}

				if (clip != null && clip.length > 0)
				{
					StartPlay (file);
					return true;
				}
			}
		}

		return false;
	}

	private void StartPlay (FileObj file)
	{
		// Update current audio source
		audio.clip = clip;

		// Reset time
		audio.time = 0;

		// Play audio
		audio.Play ();

		// Set as active file
		Settings.Active.File = file;
		Settings.Selected.File = null;

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
	}

	private IEnumerator ReadSamples (FileObj file, MpegFile mp3, int samples)
	{
		mp3Reading = true;

		// Settings
		long pos = 0;
		long step = mp3.SampleRate * mp3.Channels;
		float[] buffer = new float [samples * mp3.Channels];

		// Read samples
		while (pos < buffer.Length)
		{
			// Keep player opened
			canvas.KeepPlayer ();

			// Temporary buffer
			float[] temp = new float[step];

			// Read samples
			mp3.ReadSamples (temp, 0, (int) step);

			// Insert into buffer
			long length = pos + step <= buffer.Length ? step : buffer.Length - pos;
			Array.Copy (temp, 0, buffer, pos, length);

			// Update position
			pos += step;
			yield return pos;
		}

		// Set data for clip
		clip.SetData (buffer, 0);

		// Start play
		StartPlay (file);
		mp3Reading = false;
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
		if (audio != null && audio.clip != null && audio.clip.length != 0 && userChangedSlider)
		{
			audio.time = value < 1 ? value * audio.clip.length : audio.clip.length - 0.1f;
		}
	}

	public void UpdateTime (float value)
	{
		if (audio != null && audio.clip != null && audio.clip.length > 0)
		{
			currentTime.text = FormatTime (value * audio.clip.length);
		}
		else
		{
			timeline.value = 0;
		}
	}

	public void SetVolume (float value)
	{
		// Set volume
		audio.volume = value;

		// Update slider
		if (volumeSliderLeft.value != value) volumeSliderLeft.value = value;
		if (volumeSliderRight.value != value) volumeSliderRight.value = value;

		// Update UI
		volumeLeft.rotation = Quaternion.AngleAxis (value * VOLUME_ROTATION_MAX, Vector3.forward);
		volumeRight.rotation = Quaternion.AngleAxis (-value * VOLUME_ROTATION_MAX, Vector3.forward);
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
		ToggleRepeat (!Settings.Player.Repeat);
	}

	public void ToggleRepeat (bool state)
	{
		// Change loop
		Settings.Player.Repeat = state;
		audio.loop = state;

		// Update UI
		repeat.GetComponent<Text> ().color = audio.loop ? COLOR_ENABLED : COLOR_DISABLED;
	}

	public void ToggleShuffle () {
		ToggleShuffle (!Settings.Player.Shuffle);
	}

	public void ToggleShuffle (bool state)
	{
		// Change shuffle
		Settings.Player.Shuffle = state;

		// Update playlist
		if (Settings.Player.Shuffle)
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
		else
		{
			SetPlaylist ();
		}

		// Set position
		position = files.IndexOf (Settings.Active.File);

		// Update UI
		shuffle.GetComponent<Text> ().color = Settings.Player.Shuffle ? COLOR_ENABLED : COLOR_DISABLED;
	}



	//-- PLAYER CONTROLS

	// Play next clip
	public void Next ()
	{
		if (files != null && files.Count > 0)
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
		if (files != null && files.Count > 0)
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