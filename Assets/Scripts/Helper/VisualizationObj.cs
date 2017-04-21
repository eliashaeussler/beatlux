using UnityEngine;
using System.Collections;

public class VisualizationObj {

	public int Id { get; set; }
	public string Name { get; set; }
	public Color[] ColorSchemes { get; set; }
    public int VizId { get; set; }

	public VisualizationObj ()
	{
		Id = 0;
		Name = "";
		ColorSchemes = new Color[]{};
        VizId = 0;
	}
}
