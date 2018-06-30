/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

/// <inheritdoc />
/// <summary>
///     Provides variables to display icons using the icon font.
/// </summary>
public class IconFont : MonoBehaviour
{
    public const string Visualization = "\ue900";
    public const string DropdownOpened = "\ue901";
    public const string DropdownClosed = "\ue902";
    public const string Options = "\ue903";
    public const string Listening = "\ue904";
    public const string Search = "\ue905";
    public const string Trash = "\ue906";
    public const string Music = "\ue907";
    public const string Folder = "\ue908";
    public const string Pause = "\ue909";
    public const string Play = "\ue90a";
    public const string Add = "\ue90b";
    public const string CloseCompressed = "\ue90c";
    public const string Close = "\ue90d";
    public const string Shuffle = "\ue90e";
    public const string Edit = "\ue90f";
    public const string FastForward = "\ue910";
    public const string Rewind = "\ue911";
    public const string Repeat = "\ue912";
    public const string ArrowLeft = "\ue913";
    public const string ArrowBack = "\ue914";
    public const string Home = "\ue915";
    public const string VizNext = "\ue916";
    public const string VizPrev = "\ue917";
    public const string VizNextOld = "\ue918";
    public const string VizPrevOld = "\ue919";
    public const string Lock = "\ue91a";
    public const string RepeatSingle = "\ue91b";

	/// <summary>
	///     The icon font resource.
	/// </summary>
	public static readonly Font Font = Resources.Load<Font>("Fonts/beatlux");
}