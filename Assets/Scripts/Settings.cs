using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Settings {

	// Available visualizations
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
	public abstract class Selected
	{
		public static PlaylistObj Playlist;
		public static FileObj File;
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
		public static VisualizationObj Visualization = new VisualizationObj ("beatlux", 1, 3);


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
				new Color []
				{
					GetColor (243, 233, 177),
					GetColor (172, 231, 243),
					GetColor (243, 205, 233),
					GetColor (190, 255, 252)
				}
			},

			// Hexagons
			{
				Settings.Visualizations [1].Name,
				new Color []
				{
					GetColor (66, 70, 110),
					GetColor (58, 98, 171)
				}
			},

			// Lichterketten
			{
				Settings.Visualizations [2].Name,
				new Color []
				{
					GetColor (242, 80, 166),
					GetColor (242, 209, 60),
					GetColor (84, 191, 60),
					GetColor (37, 176, 217),
					GetColor (242, 84, 44)
				}
			},

			// Particle Fountains
			{
				Settings.Visualizations [3].Name,
				new Color []
				{
					GetColor (243, 225, 125),
					GetColor (125, 243, 156),
					GetColor (243, 153, 149)
				}
			},

			// Spektrum
			{
				Settings.Visualizations [4].Name,
				new Color []
				{
					GetColor (6, 104, 255),
					GetColor (0, 242, 255),
					GetColor (27, 240, 96)
				}
			}
		};
	}

	//-- INPUT SETTINGS
	public abstract class Input
	{
		public static int MaxLength = 30;
		public static Color InfoColor = GetColor (250, 80, 80);
	}



	//-- HELPER METHODS
	public static Color GetColor (int r, int g, int b)
	{
		if (r >= 0 && r <= 255 &&
		    g >= 0 && g <= 255 &&
		    b >= 0 && b <= 255) {

			return new Color (r / 255.0f, g / 255.0f, b / 255.0f);
		}

		return Color.clear;
	}
}
