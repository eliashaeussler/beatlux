using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaylistObj {

	public long ID { get; set; }
	public string Name { get; set; }
	public List<FileObj> Files { get; set; }

	public PlaylistObj (string Name)
	{
		this.Name = Name;
		this.ID = 0;
		this.Files = new List<FileObj> ();
	}
}