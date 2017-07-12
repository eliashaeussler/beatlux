using UnityEngine;

using System.Collections;
using System.IO;

public class MenuManager : MenuFunctions {

	void Start ()
	{
		player = Settings.MenuManager.player;
		playerControl = Settings.MenuManager.playerControl;
	}
}
