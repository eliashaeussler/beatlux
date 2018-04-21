/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */


/// <summary>
/// Saving all the settings made in the options menu. Can be written to a json file for permanent storage.
/// </summary>
public class GameSettings {
    public bool fullscreen;
    public bool tutorial;
    public int language;
    public int textureQuality;
    public int antialiasing;
    public int resolutionIndex;
    public int mirrors;
}
