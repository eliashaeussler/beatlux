using UnityEngine;
using System.Collections;

public class VisualizationObj {

	public long ID { get; set; }
	public string Name { get; set; }
	public Color[] ColorSchemes { get; set; }

	public VisualizationObj ()
	{
		ID = 0;
		Name = "";
		ColorSchemes = new Color[] { };
	}
}
