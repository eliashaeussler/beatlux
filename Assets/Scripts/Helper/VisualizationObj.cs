/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

/// <summary>
///     Visualization object containing a visualization dataset.
/// </summary>
public class VisualizationObj
{
	/// <summary>
	///     Initializes a new instance of the <see cref="VisualizationObj" /> class and
	///     sets default values for this visualization.
	/// </summary>
	public VisualizationObj()
    {
        Id = 0;
        Name = "";
        Colors = 0;
        BuildNumber = -1;
        Skybox = null;
    }

	/// <summary>
	///     Initializes a new instance of the <see cref="VisualizationObj" /> class and
	///     applies the given values to this visualization.
	/// </summary>
	/// <param name="name">The name of this visualization to be set.</param>
	/// <param name="colors">The amount of colors of this visualization to be set.</param>
	/// <param name="buildNumber">The build number of this visualization to be set.</param>
	/// <param name="skybox">The skybox name of this visualization to be set.</param>
	public VisualizationObj(string name, int colors, int buildNumber, string skybox = null)
    {
        Id = 0;
        Name = name;
        Colors = colors;
        BuildNumber = buildNumber;
        Skybox = skybox;
    }

	/// <summary>
	///     Gets or sets the id of this visualization.
	/// </summary>
	/// <value>The id of this visualization.</value>
	public long Id { get; set; }

	/// <summary>
	///     Gets or sets the name of this visualization.
	/// </summary>
	/// <value>The name of this visualization.</value>
	public string Name { get; set; }

	/// <summary>
	///     Gets or sets the amount of colors of this visualization.
	/// </summary>
	/// <value>The amount of colors in this visualization.</value>
	public int Colors { get; set; }

	/// <summary>
	///     Gets or sets the build number of this visualization.
	/// </summary>
	/// <value>The build number of this visualization.</value>
	public int BuildNumber { get; set; }

	/// <summary>
	///     Gets or sets the skybox name of this visualization. Can be <c>null</c>, if this visualization is not
	///     displayed inside any skybox.
	/// </summary>
	/// <value>The skybox name of this visualization.</value>
	public string Skybox { get; set; }

	/// <summary>
	///     Determines whether the specified <see cref="System.Object" /> is equal to the current
	///     <see cref="VisualizationObj" />.
	/// </summary>
	/// <param name="obj">The <see cref="System.Object" /> to compare with the current <see cref="VisualizationObj" />.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="System.Object" /> is equal to the current <see cref="VisualizationObj" />;
	///     otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals(object obj)
    {
        var rhs = obj as VisualizationObj;
        return rhs != null
               && Name == rhs.Name
               && Colors == rhs.Colors
               && BuildNumber == rhs.BuildNumber;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = Name != null ? Name.GetHashCode() : 0;
            hashCode = (hashCode * 397) ^ Colors;
            hashCode = (hashCode * 397) ^ BuildNumber;
            return hashCode;
        }
    }
}