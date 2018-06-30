/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

/// <summary>
///     File object containing a file dataset.
/// </summary>
public class FileObj
{
	/// <summary>
	///     Initializes a new instance of the <see cref="FileObj " /> class and sets the default path of this file.
	/// </summary>
	public FileObj()
    {
        Path = "";
    }

	/// <summary>
	///     Initializes a new instance of the <see cref="FileObj" /> class and applies the given path to this file.
	/// </summary>
	/// <param name="path">The path of this file to be set.</param>
	public FileObj(string path)
    {
        Path = path;
    }

	/// <summary>
	///     Gets or sets the id of this file.
	/// </summary>
	/// <value>The id of this file.</value>
	public long Id { get; set; }

	/// <summary>
	///     Gets or sets the path of this file.
	/// </summary>
	/// <value>The path of this file.</value>
	public string Path { get; set; }

	/// <summary>
	///     Determines whether the specified <see cref="System.Object" /> is equal to the current <see cref="FileObj" />.
	/// </summary>
	/// <param name="obj">The <see cref="System.Object" /> to compare with the current <see cref="FileObj" />.</param>
	/// <returns>
	///     <c>true</c> if the specified <see cref="System.Object" /> is equal to the current <see cref="FileObj" />;
	///     otherwise, <c>false</c>.
	/// </returns>
	public override bool Equals(object obj)
    {
        var rhs = obj as FileObj;
        return rhs != null && Path == rhs.Path;
    }

    public override int GetHashCode()
    {
        return Path != null ? Path.GetHashCode() : 0;
    }
}