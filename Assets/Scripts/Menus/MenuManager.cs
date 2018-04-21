/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

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
