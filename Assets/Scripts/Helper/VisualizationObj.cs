using UnityEngine;
using System.Collections;

public class VisualizationObj {

	public long ID { get; set; }
	public string Name { get; set; }
	public int Colors { get; set; }
	public int BuildNumber { get; set; }

	public VisualizationObj ()
	{
		ID = 0;
		Name = "";
		Colors = 0;
		BuildNumber = -1;
	}

	public VisualizationObj (string Name, int Colors, int BuildNumber)
	{
		this.ID = 0;
		this.Name = Name;
		this.Colors = Colors;
		this.BuildNumber = BuildNumber;
	}

	public override bool Equals (object obj)
	{
		VisualizationObj rhs = (VisualizationObj) obj;
		return rhs != null
			&& this.Name == rhs.Name
			&& this.Colors == rhs.Colors
			&& this.BuildNumber == rhs.BuildNumber;
	}
}
