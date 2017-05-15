using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using System;

public class VizPlayer : MonoBehaviour {

	// UI elements
	public GameObject previous;
	public GameObject next;
	public GameObject shuffle;
	public Button logo;

	// Visualizations
	private List<VisualizationObj> visualizations;
	private int oldPos;
	private int position;

	// Shuffle state
	private bool isShuffled = false;



	void Start ()
	{
		// Set logo onClick listener
		logo.onClick.AddListener (delegate {

			Settings.Selected.Visualization = Settings.Defaults.Visualization;

		});
	}

	void Update ()
	{
		// Check for updated visualization
		if ((Settings.Selected.Visualization != null
			&& !Settings.Selected.Visualization.Equals (Settings.Active.Visualization))
		    || visualizations == null) {

			ToggleShuffle (Settings.Player.ShuffleViz);
		}

		// Set current position
		if (Settings.Selected.Visualization != null) {
			position = visualizations.IndexOf (Settings.Selected.Visualization);
		}

		// Start visualization if current has changed
		if (oldPos != position && Settings.Selected.Visualization != null
			&& !Settings.Selected.Visualization.Equals (Settings.Active.Visualization)) {

			StartViz ();
		}
	}



	private void SetVisualizations ()
	{
		// Instantiate list
		visualizations = new List<VisualizationObj> ();

		// Get all visualizations
		foreach (VisualizationObj viz in Settings.Visualizations) {
			visualizations.Add (viz);
		}
	}

	private void StartViz ()
	{
		if (Settings.Selected.Visualization != null
			&& Application.CanStreamedLevelBeLoaded (Settings.Selected.Visualization.BuildNumber))
		{
			// Set old position
			oldPos = position;

			// Set color scheme
			if (Settings.Selected.ColorScheme != null
				&& Settings.Selected.ColorScheme.Visualization.Equals (Settings.Selected.Visualization)) {
					
				Settings.Active.ColorScheme = Settings.Selected.ColorScheme;
			} else {
				Settings.Active.ColorScheme = ColorScheme.GetDefault (Settings.Selected.Visualization);
			}

			// Reset selected color scheme
			Settings.Selected.ColorScheme = null;

			// Set active visualization
			Settings.Active.Visualization = Settings.Selected.Visualization;

			// Start visualization
			Settings.MenuManager.StartLevel (Settings.Selected.Visualization.BuildNumber);

			// Reset selected visualization
			Settings.Selected.Visualization = null;
		}
	}

	public void ToggleShuffle () {
		ToggleShuffle (!Settings.Player.ShuffleViz);
	}

	public void ToggleShuffle (bool state)
	{
		// Change shuffle
		Settings.Player.ShuffleViz = state;

		// Set visualizations
		if (visualizations == null || !Settings.Player.ShuffleViz) {
			SetVisualizations ();
		}

		// Update visualizations list
		if (Settings.Player.ShuffleViz && !isShuffled)
		{
			// Re-order visualizations
			System.Random rand = new System.Random ();
			int n = visualizations.Count;
			while (n > 1) {
				n--;
				int k = rand.Next (n + 1);
				VisualizationObj val = visualizations [k];
				visualizations [k] = visualizations [n];
				visualizations [n] = val;
			}
		}

		// Set selected visualization
		if (Settings.Selected.Visualization == null) {
			Settings.Selected.Visualization = Settings.Active.Visualization;
		}

		// Set shuffle
		isShuffled = Settings.Player.ShuffleViz;

		// Update UI
		shuffle.GetComponent<Text> ().color = Settings.Player.ShuffleViz ? Player.COLOR_ENABLED : Player.COLOR_DISABLED;
	}



	//-- PLAYER CONTROLS

	// Select next visualization
	public void Next ()
	{
		Settings.Selected.Visualization = GetViz (1);
	}

	// Select previous visualization
	public void Previous ()
	{
		Settings.Selected.Visualization = GetViz (-1);
	}



	//-- HELPER METHODS

	public VisualizationObj GetViz (int step)
	{
		int pos = position;

		if (visualizations.Count > 0)
		{
			// Next file
			if (step == 1) pos = position < visualizations.Count-1 ? position+1 : 0;

			// Previous file
			else if (step == -1) pos = position > 0 ? position-1 : visualizations.Count-1;
		}

		return visualizations [pos];
	}
}
