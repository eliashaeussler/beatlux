/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

public class MenuManager : MenuFunctions
{
    private void Start()
    {
        Player = Settings.MenuManager.Player;
        PlayerControl = Settings.MenuManager.PlayerControl;
    }
}