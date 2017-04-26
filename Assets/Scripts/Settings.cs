using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Settings {

	//-- VISUALIZATIONS
	public static VisualizationObj [] Visualizations = new VisualizationObj []
	{
		new VisualizationObj ("Bubbles",			4,	-1), // TODO: fix build number
		new VisualizationObj ("Hexagons",			2,	4),
		new VisualizationObj ("Lichterketten",		5,	5),
		new VisualizationObj ("Particle Fountains",	3,	-1), // TODO: fix build number
		new VisualizationObj ("Spektrum",			3,	6)
	};



	//-- ACTIVE ELEMENTS
	public abstract class Active
	{
		public static PlaylistObj Playlist;
		public static FileObj File;
		public static VisualizationObj Visualization;
		public static ColorSchemeObj ColorScheme;
	}

	//-- OPENED ELEMENTS
	public abstract class Opened
	{
		public static PlaylistObj Playlist;
		public static VisualizationObj Visualization;
		public static ColorSchemeObj ColorScheme;
	}

	//-- SOURCE FOLDER PATHS
	public abstract class Source
	{
		public static string Main;
		public static string Current;
	}

	//-- DEFAULT SETTINGS
	public abstract class Defaults
	{
		public static VisualizationObj Visualization = new VisualizationObj ("beatlux", 1, 5);

		//-- DEFAULT COLORS
		public static Dictionary<string, Color[]> Colors = new Dictionary<string, Color[]>
		{
			// Default visualization (invisible)
			{
				Visualization.Name,
				new Color [] {}
			},



			// Bubbles
			{
				Settings.Visualizations [0].Name,
				new Color [] {
					new Color (243 / 255.0f,	233 / 255.0f,	177 / 255.0f),
					new Color (172 / 255.0f,	231 / 255.0f,	243 / 255.0f),
					new Color (243 / 255.0f,	205 / 255.0f,	233 / 255.0f),
					new Color (190 / 255.0f,	255 / 255.0f,	252 / 255.0f)
				}
			},

			// Hexagons
			{
				Settings.Visualizations [1].Name,
				new Color [] {
					new Color ( 66 / 255.0f,	 70 / 255.0f,	110 / 255.0f),
					new Color ( 58 / 255.0f,	 98 / 255.0f,	171 / 255.0f)
				}
			},

			// Lichterketten
			{
				Settings.Visualizations [2].Name,
				new Color [] {
					new Color (242 / 255.0f,	 80 / 255.0f,	166 / 255.0f),
					new Color (242 / 255.0f,	209 / 255.0f,	 60 / 255.0f),
					new Color ( 84 / 255.0f,	191 / 255.0f,	 60 / 255.0f),
					new Color ( 37 / 255.0f,	176 / 255.0f,	217 / 255.0f),
					new Color (242 / 255.0f,	 84 / 255.0f,	 44 / 255.0f)
				}
			},

			// Particle Fountains
			{
				Settings.Visualizations [3].Name,
				new Color [] {
					new Color (243 / 255.0f,	225 / 255.0f,	125 / 255.0f),
					new Color (125 / 255.0f,	243 / 255.0f,	156 / 255.0f),
					new Color (243 / 255.0f,	153 / 255.0f,	149 / 255.0f)
				}
			},

			// Spektrum
			{
				Settings.Visualizations [4].Name,
				new Color [] {
					new Color (  6 / 255.0f,	104 / 255.0f,	255 / 255.0f),
					new Color (  0 / 255.0f,	242 / 255.0f,	255 / 255.0f),
					new Color ( 27 / 255.0f,	240 / 255.0f,	 96 / 255.0f)
				}
			}
		};
	}
}
