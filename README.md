[![Build Status](https://travis-ci.org/eliashaeussler/beatlux.svg?branch=master)](https://travis-ci.org/eliashaeussler/beatlux)

# beatlux
Beatlux is a music visualisation software. It features a music player with all the standard functions, as well as some visualisations that react to the music that is playing. You are able to create playlists, customize the visualisations and move through the visualisations freely.

## Code Example
    void Start()
    {
        // Initializing the scene, creating a circle of cubes in the radius set above.
        for (int i = 0; i < numberOfObjects; i++)
        {
            float angle = i * Mathf.PI * 2 / numberOfObjects;
            Vector3 pos = new Vector3 (Mathf.Cos (angle), 0, Mathf.Sin (angle)) * radius;
            Instantiate (prefab, pos, Quaternion.identity, Settings.MenuManager.vizContents);
        }
        cubes = GameObject.FindGameObjectsWithTag ("Cubes");
    }
    void Update()
    {
        // Reading the spectrum from the song put into the AudioListener 
        float[] spectrum = Settings.MenuManager.audio.GetSpectrumData (1024, 0, FFTWindow.Hamming);
    }
New visualisations can be added to the programm as a new scene in Unity. They also have to be added to the class "Settings".

## Motivation
The creation of Beatlux was the result of a student project of the study course "Media-Informatics" at Harz University of Applied Science Wernigerode. The task was to visualize music in any form, so we decided to create a program that does just that.

## Installation
To install the program, simply download the appropriate files for your operating system from our website (http://beatlux.hs-harz.de) and run it on your computer.

## API Reference
https://unity3d.com/de/get-unity/download
https://docs.unity3d.com/ScriptReference/

## Tests
Just build the project in Unity for your operating system, run it and try it out.

## Contributors
Anyone who wants to participate in the development of Beatlux can simply fork the project and change anything, since it's open source.

## Used packages and resources
* mpg123.net (http://mpg123.net/)
* SQLite (https://www.sqlite.org/)
* TagLib# (https://github.com/mono/taglib-sharp)
* HSV-Color-Picker-Unity (https://github.com/judah4/HSV-Color-Picker-Unity/tree/master/Assets/HSVPicker)
* beatlux Font (created using IcoMoon: https://icomoon.io/)
* Skybox Purple Nebula (https://www.assetstore.unity3d.com/en/#!/content/2967)
* TextUnicode.cs (http://forum.unity3d.com/threads/image-fonts-fontawesome.281746/#post-1862245)
* MainThreadDispatcher.cs (https://github.com/PimDeWitte/UnityMainThreadDispatcher)
* Mutli-Lang Support (https://forum.unity3d.com/threads/add-multiple-language-support-to-your-unity-projects.206271/)

## License
The MIT License (MIT)
Copyright (c) 2016 Judah Perez
 
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.
 
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
