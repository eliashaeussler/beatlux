using UnityEngine;
using System.Collections;

public class FileObj {

	public int ID { get; set; }
	public string Name { get; set; }
	public string Path { get; set; }

	public FileObj ()
	{
		ID = 0;
		Name = "";
		Path = "";
	}
}
