using UnityEngine;
using System.Collections;

public class Settings {
	
	public static PlaylistObj ActivePlaylist;
	public static FileObj ActiveFile;
	public static VisualizationObj ActiveVisualization;
	public static ColorSchemeObj ActiveColorScheme;

	public static PlaylistObj OpenedPlaylist;
	public static VisualizationObj OpenedVisualization;
	public static ColorSchemeObj OpenedColorScheme;

	public static string MainPath;
	public static string CurrentPath;

	public static VisualizationObj [] Visualizations = new VisualizationObj []
	{
		new VisualizationObj ("Divided", 1, 3),
		new VisualizationObj ("Hexagons", 1, 4),
		new VisualizationObj ("Kette", 1, 5),
		new VisualizationObj ("LogoVisualisation", 1, 6),
		new VisualizationObj ("Spectrum", 1, 7)
		// TODO add and modify visualizations
	};
}
