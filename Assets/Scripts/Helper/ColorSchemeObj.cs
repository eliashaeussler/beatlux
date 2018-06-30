﻿/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

/// <summary>
///     Database object containing a color scheme dataset.
/// </summary>
public class ColorSchemeObj
{
	/// <summary>
	///     Initializes a new instance of the <see cref="ColorSchemeObj" /> class and
	///     sets default values for this color scheme.
	/// </summary>
	/// <param name="name">The name of this color scheme to be set.</param>
	public ColorSchemeObj(string name)
    {
        Name = name;
        Id = 0;
        Visualization = null;
        Colors = new Color[] { };
    }

	/// <summary>
	///     Initializes a new instance of the <see cref="ColorSchemeObj" /> class and
	///     sets the visualization connected to this color scheme. It also tries to
	///     set the colors of this color scheme which should be defined in
	///     <see cref="Settings.Defaults.Colors" />.
	/// </summary>
	/// <param name="name">The name of this color scheme to be set.</param>
	/// <param name="visualization">The visualization connected to this color scheme to be set.</param>
	public ColorSchemeObj(string name, VisualizationObj visualization)
    {
        Name = name;
        Visualization = visualization;
        Id = 0;

        // Set colors
        if (Settings.Defaults.Colors.ContainsKey(visualization.Name))
        {
            Colors = Settings.Defaults.Colors[visualization.Name];
        }
        else
        {
            Colors = new Color [visualization.Colors];
            for (var i = 0; i < Colors.Length; i++)
                Colors[i] = Color.white;
        }
    }

	/// <summary>
	///     Gets or sets the id of this color scheme.
	/// </summary>
	/// <value>The id of this color scheme.</value>
	public long Id { get; set; }

	/// <summary>
	///     Gets or sets the name of this color scheme.
	/// </summary>
	/// <value>The name of this color scheme.</value>
	public string Name { get; set; }

	/// <summary>
	///     Gets or sets the visualization connected to this color scheme.
	/// </summary>
	/// <value>The visualization object connected to this color scheme.</value>
	public VisualizationObj Visualization { get; set; }

	/// <summary>
	///     Gets or sets the colors of this color scheme.
	/// </summary>
	/// <value>The colors of this color scheme.</value>
	public Color[] Colors { get; set; }

	/// <summary>
	///     Determines whether the specified <see cref="System.Object" /> is equal to the current <see cref="ColorSchemeObj" />
	///     .
	/// </summary>
	/// <param name="obj">The <see cref="System.Object" /> to compare with the current <see cref="ColorSchemeObj" />.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="System.Object" /> is equal to the current <see cref="ColorSchemeObj" />;
	///     otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals(object obj)
    {
        var rhs = obj as ColorSchemeObj;
        return rhs != null
               && Name == rhs.Name
               && Visualization.Equals(rhs.Visualization);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return ((Name != null ? Name.GetHashCode() : 0) * 397) ^
                   (Visualization != null ? Visualization.GetHashCode() : 0);
        }
    }
}