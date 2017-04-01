using UnityEngine;
using System.Collections;

public class PlaylistObj {

	public int ID { get; set; }
	public string Name { get; set; }
	public string[] Files { get; set; }

	public PlaylistObj ()
	{
		ID = 0;
		Name = "";
		Files = new string[] { };
	}
}
