using UnityEngine;
using System.Collections;

public class ColorSchemeObj {

    public long ID { get; set; }
    public string Name { get; set; }
	public VisualizationObj Visualization { get; set; }
	public Color [] Colors { get; set; }

	public ColorSchemeObj (string Name)
	{
		this.Name = Name;
        ID = 0;
		Visualization = null;
		Colors = new Color [] { };
	}

	public ColorSchemeObj (string Name, VisualizationObj Visualization)
	{
		this.Name = Name;
		this.Visualization = Visualization;
		ID = 0;

		// Set colors
		if (Settings.Defaults.Colors.ContainsKey (Visualization.Name))
		{
			Colors = Settings.Defaults.Colors [Visualization.Name];
		}
		else
		{
			Colors = new Color [Visualization.Colors];
			for (int i = 0; i < Colors.Length; i++)
				Colors [i] = Color.white;
		}
	}

	public override bool Equals (object obj)
	{
		ColorSchemeObj rhs = (ColorSchemeObj) obj;
		return rhs != null
		&& this.Name == rhs.Name
		&& this.Visualization.Equals (rhs.Visualization);
	}
}
