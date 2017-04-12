using UnityEngine;
using System.Collections;

public class FileObj {

	public long ID { get; set; }
	public string Name { get; set; }
	public string Path { get; set; }

	public FileObj ()
	{
		ID = 0;
		Name = "";
		Path = "";
	}

	public override bool Equals(object obj)
	{
		FileObj rhs = (FileObj) obj;
		return rhs != null && this.Name == rhs.Name && this.Path == rhs.Path;
	}
}
