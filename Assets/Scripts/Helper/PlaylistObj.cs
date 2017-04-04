using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlaylistObj {

	public int ID { get; set; }
	public string Name { get; set; }
	public List<FileObj> Files { get; set; }

	public PlaylistObj ()
	{
		ID = 0;
		Name = "";
		Files = new List<FileObj> ();
	}
}
