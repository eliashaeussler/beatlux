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

	// Visualizations
	private List<VisualizationObj> visualizations;
	private int position;



	void Update ()
	{
		ToggleShuffle (Settings.Player.ShuffleViz);
	}



	private void SetVisualizations ()
	{
		// Instantiate list
		visualizations = new List<VisualizationObj> ();

		// Take all visualizations
		foreach (VisualizationObj viz in Settings.Visualizations) {
			visualizations.Add (viz);
		}

		// Set position in list
		if (Settings.Visualizations.Contains (Settings.Active.Visualization)) {
			position = Array.IndexOf (Settings.Visualizations, Settings.Active.Visualization);
		} else {
			position = -1;
		}
	}

	private bool Select (VisualizationObj viz)
	{
		if (viz != null && Application.CanStreamedLevelBeLoaded (viz.BuildNumber))
		{
			// Set as active visualization
			Settings.Selected.Visualization = viz;

			// Reset color scheme
			Settings.Selected.ColorScheme = ColorScheme.GetDefault (viz);

			// Get level
			VisualizationObj selectedViz = Settings.MenuManager.StartVisualization (false);

			// Start level
			if (selectedViz != null) Settings.MenuManager.StartLevel (selectedViz.BuildNumber);


			return visualizations.IndexOf (selectedViz) != position;
		}

		return false;
	}

	public void ToggleShuffle () {
		ToggleShuffle (!Settings.Player.ShuffleViz);
	}

	public void ToggleShuffle (bool state)
	{
		// Change shuffle
		Settings.Player.ShuffleViz = state;

		// Set visualizations
		SetVisualizations ();

		// Update playlist
		if (Settings.Player.ShuffleViz)
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

			// Set position
			position = visualizations.IndexOf (Settings.Active.Visualization);
		}

		// Update UI
		shuffle.GetComponent<Text> ().color = Settings.Player.ShuffleViz ? Player.COLOR_ENABLED : Player.COLOR_DISABLED;
	}



	//-- PLAYER CONTROLS

	// Select next visualization
	public void Next ()
	{ 
		if (visualizations.Count > 0)
		{
			bool found = false;
			int tempPos = position;

			// Try to select next visualization
			while (!found)
			{
				// Get next element in list
				tempPos = GetVizIndex (tempPos, 1);

				// Select next element in list
				found = Select (visualizations [tempPos]);
				if (tempPos == position) break;
			}
		}
	}

	// Select previous visualization
	public void Previous ()
	{
		if (visualizations.Count > 0)
		{
			bool found = false;
			int tempPos = position;

			// Try to select previous visualization
			while (!found)
			{
				// Get previous element in list
				tempPos = GetVizIndex (tempPos, -1);

				// Select previous element in list
				found = Select (visualizations [tempPos]);
				if (tempPos == position) break;
			}
		}

		print (Settings.Active.Visualization.Name);
	}



	//-- HELPER METHODS

	public int GetVizIndex (int position, int step)
	{
		if (visualizations.Count > 0)
		{
			// Next file
			if (step == 1) return position < visualizations.Count-1 ? position+1 : 0;

			// Previous file
			else if (step == -1) return position > 0 ? position-1 : visualizations.Count-1;
		}

		return position;
	}
}
