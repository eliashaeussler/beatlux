/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using System;
using System.Collections.Generic;

/// <summary>
///     Playlist object containing a playlist dataset.
/// </summary>
public class PlaylistObj : ICloneable
{
	/// <summary>
	///     Initializes a new instance of the <see cref="PlaylistObj" /> class and
	///     sets default values for this playlist
	/// </summary>
	/// <param name="name">The name of this playlist to be set.</param>
	public PlaylistObj(string name)
    {
        Name = name;
        Id = 0;
        Files = new List<FileObj>();
    }

	/// <summary>
	///     Gets or sets the id of this playlist.
	/// </summary>
	/// <value>The id of this playlist.</value>
	public long Id { get; set; }

	/// <summary>
	///     Gets or sets the name of this playlist.
	/// </summary>
	/// <value>The name of this playlist.</value>
	public string Name { get; set; }

	/// <summary>
	///     Gets or sets the files contained in this playlist.
	/// </summary>
	/// <value>The files contained in this playlist.</value>
	public List<FileObj> Files { get; set; }

    /// <seealso cref="object.Clone()" />
    public object Clone()
    {
        return MemberwiseClone();
    }

	/// <summary>
	///     Determines whether the specified <see cref="System.Object" /> is equal to the current <see cref="PlaylistObj" />.
	/// </summary>
	/// <param name="obj">The <see cref="System.Object" /> to compare with the current <see cref="PlaylistObj" />.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="System.Object" /> is equal to the current <see cref="PlaylistObj" />;
	///     otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals(object obj)
    {
        var rhs = obj as PlaylistObj;
        return rhs != null && Name == rhs.Name;
    }

    public override int GetHashCode()
    {
        return Name != null ? Name.GetHashCode() : 0;
    }
}