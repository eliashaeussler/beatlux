using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlaylistObj : ICloneable {

	public long ID { get; set; }
	public string Name { get; set; }
	public List<FileObj> Files { get; set; }

	public PlaylistObj (string Name)
	{
		this.Name = Name;
		this.ID = 0;
		this.Files = new List<FileObj> ();
	}



	public object Clone ()
	{
		return this.MemberwiseClone ();
	}

	public override bool Equals (object obj)
	{
		PlaylistObj rhs = (PlaylistObj) obj;
		return rhs != null && this.Name == rhs.Name;
	}
}