using UnityEngine;
using System.Collections;

public class FileObj {

	public long ID { get; set; }
	public string Path { get; set; }
	public string [] Artists { get; set; }
	public string Album { get; set; }
	public string Title { get; set; }

	public FileObj () {
		Path = "";
	}

	public FileObj (string Path) {
		this.Path = Path;
	}

	public override bool Equals (object obj)
	{
		FileObj rhs = (FileObj) obj;
		return rhs != null && this.Path == rhs.Path;
	}
}
