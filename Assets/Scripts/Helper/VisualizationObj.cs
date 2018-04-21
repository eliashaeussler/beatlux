/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;
using System.Collections;


/// <summary>
/// Visualization object containing a visualization dataset.
/// </summary>
public class VisualizationObj {

	/// <summary>
	/// Gets or sets the id of this visualization.
	/// </summary>
	/// <value>The id of this visualization.</value>
	public long ID { get; set; }

	/// <summary>
	/// Gets or sets the name of this visualization.
	/// </summary>
	/// <value>The name of this visualization.</value>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the amount of colors of this visualization.
	/// </summary>
	/// <value>The amount of colors in this visualization.</value>
	public int Colors { get; set; }

	/// <summary>
	/// Gets or sets the build number of this visualization.
	/// </summary>
	/// <value>The build number of this visualization.</value>
	public int BuildNumber { get; set; }

	/// <summary>
	/// Gets or sets the skybox name of this visualization. Can be <c>null</c>, if this visualization is not
	/// displayed inside any skybox.
	/// </summary>
	/// <value>The skybox name of this visualization.</value>
	public string Skybox { get; set; }


	/// <summary>
	/// Initializes a new instance of the <see cref="VisualizationObj" /> class and
	/// sets default values for this visualization.
	/// </summary>
	public VisualizationObj ()
	{
		ID = 0;
		Name = "";
		Colors = 0;
		BuildNumber = -1;
		Skybox = null;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="VisualizationObj" /> class and
	/// applies the given values to this visualization.
	/// </summary>
	/// <param name="Name">The name of this visualization to be set.</param>
	/// <param name="Colors">The amount of colors of this visualization to be set.</param>
	/// <param name="BuildNumber">The build number of this visualization to be set.</param>
	/// <param name="Skybox">The skybox name of this visualization to be set.</param>
	public VisualizationObj (string Name, int Colors, int BuildNumber, string Skybox)
	{
		this.ID = 0;
		this.Name = Name;
		this.Colors = Colors;
		this.BuildNumber = BuildNumber;
		this.Skybox = Skybox;
	}

	/// <summary>
	/// Determines whether the specified <see cref="System.Object" /> is equal to the current <see cref="VisualizationObj" />.
	/// </summary>
	/// <param name="obj">The <see cref="System.Object" /> to compare with the current <see cref="VisualizationObj" />.</param>
	/// <returns>
	/// <c>true</c> if the specified <see cref="System.Object" /> is equal to the current <see cref="VisualizationObj" />;
	/// otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals (object obj)
	{
		VisualizationObj rhs = (VisualizationObj) obj;
		return rhs != null
			&& this.Name == rhs.Name
			&& this.Colors == rhs.Colors
			&& this.BuildNumber == rhs.BuildNumber;
	}
}
