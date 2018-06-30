/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

public class FountainScript : MonoBehaviour
{
    private readonly float[] _samples = new float[1024];
    private bool _bol;
    private Color _col1;
    private Color _col2;
    private Color _col3;
    private Color[] _col4;
    private Color _col5;
    private Color _col6;
    private Color _col7;
    private int _colVal1;
    private int _colVal2;
    private float _comp;
    private int _count;
    private float _maxVol;


    // Initialisation of the given colors
    private void Start()
    {
        GetComponent<ParticleSystem>().Play();
        if (Settings.Active.ColorScheme == null)
        {
            //add colors here
        }
        else
        {
            _col1 = Settings.Active.ColorScheme.Colors[0];
            _col2 = Settings.Active.ColorScheme.Colors[1];
            _col3 = Settings.Active.ColorScheme.Colors[2];
            _col5 = Settings.Active.ColorScheme.Colors[3];
            _col6 = Settings.Active.ColorScheme.Colors[4];
            _col7 = Settings.Active.ColorScheme.Colors[5];
        }

        _col4 = new[] {_col1, _col2, _col3, _col5, _col6, _col7};
    }

    private void Update()
    {
        AudioListener.GetOutputData(_samples, 0);

        //makes fountains dependend on volume
        float vol = 0;
        foreach (var sample in _samples)
            if (sample >= 0)
                vol += sample;
            else
                vol -= sample;
        if (_maxVol < vol) _maxVol = vol;
        if (vol <= 0)
        {
            _bol = true;
            GetComponent<ParticleSystem>().Stop();
        }
        else if (vol > 0 && _bol)
        {
            GetComponent<ParticleSystem>().Play();
            _bol = false;
        }


        switch (_count)
        {
            //switches between selected colors
            case 0:
                _colVal1 = Random.Range(0, _col4.Length);
                _colVal2 = Random.Range(0, _col4.Length);
                break;
            case 100:
                _colVal1 = Random.Range(0, _col4.Length);
                _colVal2 = Random.Range(0, _col4.Length);
                break;
            case 200:
                _colVal1 = Random.Range(0, _col4.Length);
                _colVal2 = Random.Range(0, _col4.Length);
                break;
            case 300:
                _count = 0;
                break;
        }

        // controlls color
        var ps = GetComponent<ParticleSystem>();
        ps.startLifetime = 3.5f * vol / _maxVol;
        ps.startColor = Color.Lerp(_col4[_colVal1], _col4[_colVal2], Time.time);

        _count++;

        // controls angle
        var sh = ps.shape;
        sh.enabled = true;
        if (_comp < AudioPeer.FreqBands[0])
        {
            _comp = AudioPeer.FreqBands[0];
            sh.angle = sh.angle + 0.4f;
        }
        else if (_comp > AudioPeer.FreqBands[0] && sh.angle > 3.0)
        {
            _comp = AudioPeer.FreqBands[0];
            sh.angle = sh.angle - 0.4f;
        }
    }
}