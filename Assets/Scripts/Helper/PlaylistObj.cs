using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Playlist object containing a playlist dataset.
/// </summary>
public class PlaylistObj : ICloneable {

	/// <summary>
	/// Gets or sets the id of this playlist.
	/// </summary>
	/// <value>The id of this playlist.</value>
	public long ID { get; set; }

	/// <summary>
	/// Gets or sets the name of this playlist.
	/// </summary>
	/// <value>The name of this playlist.</value>
	public string Name { get; set; }

	/// <summary>
	/// Gets or sets the files contained in this playlist.
	/// </summary>
	/// <value>The files contained in this playlist.</value>
	public List<FileObj> Files { get; set; }


	/// <summary>
	/// Initializes a new instance of the <see cref="PlaylistObj" /> class and
	/// sets default values for this playlist
	/// </summary>
	/// <param name="Name">The name of this playlist to be set.</param>
	public PlaylistObj (string Name)
	{
		this.Name = Name;
		this.ID = 0;
		this.Files = new List<FileObj> ();
	}

	/// <seealso cref="System.Object.Clone()" />
	public object Clone () {
		return this.MemberwiseClone ();
	}

	/// <summary>
	/// Determines whether the specified <see cref="System.Object" /> is equal to the current <see cref="PlaylistObj" />.
	/// </summary>
	/// <param name="obj">The <see cref="System.Object" /> to compare with the current <see cref="PlaylistObj" />.</param>
	/// <returns>
	/// <c>true</c> if the specified <see cref="System.Object" /> is equal to the current <see cref="PlaylistObj" />;
	/// otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals (object obj)
	{
		PlaylistObj rhs = (PlaylistObj) obj;
		return rhs != null && this.Name == rhs.Name;
	}
}