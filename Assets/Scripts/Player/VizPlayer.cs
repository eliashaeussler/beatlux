/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class VizPlayer : MonoBehaviour
{
    // Shuffle state
    private bool _isShuffled;
    private int _oldPos;
    private int _position;

    // Visualizations
    private List<VisualizationObj> _visualizations;

    // Player canvas
    public PlayerCanvas Canvas;
    public Button Logo;
    public GameObject NextButton;

    // UI elements
    public GameObject PreviousButton;
    public GameObject ShuffleButton;


    private void Start()
    {
        // Set logo onClick listener
        Logo.onClick.AddListener(delegate { Settings.Selected.Visualization = Settings.Defaults.Visualization; });
    }

    private void Update()
    {
        // Reset old position and shuffle state if playlist has changed
        if (Settings.Selected.Visualization != null &&
            !Settings.Selected.Visualization.Equals(Settings.Active.Visualization))
        {
            _oldPos = -2;
            _isShuffled = false;
        }

        // Check for updated visualization
        if (Settings.Selected.Visualization != null
            && !Settings.Selected.Visualization.Equals(Settings.Active.Visualization)
            || _visualizations == null)
            ToggleShuffle(Settings.Player.ShuffleViz);

        // Set current position
        if (Settings.Selected.Visualization != null)
            _position = _visualizations.IndexOf(Settings.Selected.Visualization);

        // Start visualization if current has changed
        if (_oldPos != _position && Settings.Selected.Visualization != null
                                 && !Settings.Selected.Visualization.Equals(Settings.Active.Visualization))
            StartViz();
    }


    private void SetVisualizations()
    {
        // Instantiate list
        _visualizations = new List<VisualizationObj>();

        // Get all visualizations
        foreach (var viz in Settings.Visualizations) _visualizations.Add(viz);
    }

    private void StartViz()
    {
        if (Settings.Selected.Visualization == null ||
            !Application.CanStreamedLevelBeLoaded(Settings.Selected.Visualization.BuildNumber)) return;

        // Set old position
        _oldPos = _position;

        // Set color scheme
        if (Settings.Selected.ColorScheme != null
            && Settings.Selected.ColorScheme.Visualization.Equals(Settings.Selected.Visualization))
            Settings.Active.ColorScheme = Settings.Selected.ColorScheme;
        else
            Settings.Active.ColorScheme = ColorScheme.GetDefault(Settings.Selected.Visualization);

        // Reset selected color scheme
        Settings.Selected.ColorScheme = null;

        // Set active visualization
        Settings.Active.Visualization = Settings.Selected.Visualization;

        // Start visualization
        Settings.MenuManager.StartLevel(Settings.Selected.Visualization.BuildNumber);

        // Reset selected visualization
        Settings.Selected.Visualization = null;

        // Keep player active
//			canvas.KeepPlayer ();
    }

    public void ToggleShuffle()
    {
        ToggleShuffle(!Settings.Player.ShuffleViz);
    }

    private void ToggleShuffle(bool state)
    {
        // Change shuffle
        Settings.Player.ShuffleViz = state;

        // Set visualizations
        if (_visualizations == null || !Settings.Player.ShuffleViz) SetVisualizations();

        // Update visualizations list
        if (Settings.Player.ShuffleViz && !_isShuffled)
        {
            // Re-order visualizations
            var rand = new Random();
            var n = _visualizations.Count;
            while (n > 1)
            {
                n--;
                var k = rand.Next(n + 1);
                var val = _visualizations[k];
                _visualizations[k] = _visualizations[n];
                _visualizations[n] = val;
            }
        }

        // Set selected visualization
        if (Settings.Selected.Visualization == null) Settings.Selected.Visualization = Settings.Active.Visualization;

        // Set shuffle
        _isShuffled = Settings.Player.ShuffleViz;

        // Update UI
        ShuffleButton.GetComponent<Text>().color =
            Settings.Player.ShuffleViz ? Player.ColorEnabled : Player.ColorDisabled;
    }


    //-- PLAYER CONTROLS

    // Select next visualization
    public void Next()
    {
        Settings.Selected.Visualization = GetViz(1);
    }

    // Select previous visualization
    public void Previous()
    {
        Settings.Selected.Visualization = GetViz(-1);
    }


    //-- HELPER METHODS

    private VisualizationObj GetViz(int step)
    {
        var pos = _position;

        if (_visualizations.Count <= 0) return _visualizations[pos];

        // Next file
        if (step == 1) pos = _position < _visualizations.Count - 1 ? _position + 1 : 0;

        // Previous file
        else if (step == -1) pos = _position > 0 ? _position - 1 : _visualizations.Count - 1;

        return _visualizations[pos];
    }
}