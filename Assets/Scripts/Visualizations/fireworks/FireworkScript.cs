/*
 * Copyright (c) 2018 Elias Haeussler <mail@elias-haeussler.de> (www.elias-haeussler.de).
 */

using UnityEngine;

public class FireworkScript : MonoBehaviour
{
    private readonly float[] _samples = new float [1024];
    private float _bassMax;
    private Color[] _colors;
    private float _dbValue;

    private int _qSamples = 1024;
    private float _refValue = 0.01f;
    private float _rmsValue;
    private float _volMax;

    public int Band;
    public int ExtraHeight;


    // Initialising all the given colors
    private void Start()
    {
        _colors = new[]
        {
            Settings.Active.ColorScheme.Colors[0], Settings.Active.ColorScheme.Colors[1],
            Settings.Active.ColorScheme.Colors[2], Settings.Active.ColorScheme.Colors[3],
            Settings.Active.ColorScheme.Colors[4], Settings.Active.ColorScheme.Colors[5]
        };
        GameObject.Find("BigBang").GetComponent<ParticleSystem>().Stop();
    }

    private void Update()
    {
        // Getting the volumedata 
        GameObject.Find("BigBang").GetComponent<ParticleSystem>().Stop();
        AudioListener.GetOutputData(_samples, 0);
        float vol = 0;
        foreach (var sample in _samples)
            if (sample >= 0)
                vol += sample;
            else
                vol -= sample;

        if (_volMax < vol) _volMax = vol;

        // if a volumedifference is big enough, a big rocket is fired
        if (AudioPeer.FreqBands[0] >= _bassMax * 15)
        {
            var sys = GameObject.Find("SubEmitterDeathBig").GetComponent<ParticleSystem>();
            var sys1 = GameObject.Find("SubEmitterDeathBig2").GetComponent<ParticleSystem>();
            var col1 = sys.colorOverLifetime;
            var col2 = sys1.colorOverLifetime;
            col1.enabled = true;
            col2.enabled = true;
            var grad11 = new Gradient();
            var grad12 = new Gradient();
            grad11.SetKeys(new[] {new GradientColorKey(_colors[Random.Range(0, _colors.Length)], 0.0f)},
                new[] {new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)});
            grad12.SetKeys(new[] {new GradientColorKey(_colors[Random.Range(0, _colors.Length)], 0.0f)},
                new[] {new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)});
            col1.color = new ParticleSystem.MinMaxGradient(grad11, grad12);
            col2.color = new ParticleSystem.MinMaxGradient(grad11, grad12);
            GameObject.Find("BigBang").GetComponent<ParticleSystem>().Play();
        }

        _bassMax = AudioPeer.FreqBands[0];

        // if below a certain threshhold, nothing happens, else particles are created
        if (AudioPeer.FreqBands[Band] <= 0)
        {
            GetComponent<ParticleSystem>().Stop();
        }
        else
        {
            GetComponent<ParticleSystem>().Play();
            GetComponent<ParticleSystem>().startSpeed = 10 + 25 * AudioPeer.FreqBands[Band] + ExtraHeight;
            GetComponent<ParticleSystem>().emission.SetBursts(
                new[]
                {
                    new ParticleSystem.Burst(0, (short) (10 * AudioPeer.FreqBands[Band]))
                });
            var children = GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child.name != "SubEmitterDeath") continue;

                var paSy = child.GetComponent<ParticleSystem>();
                paSy.startLifetime = 0.4f + vol / _volMax;
                var col = paSy.colorOverLifetime;
                col.enabled = true;
                var grad = new Gradient();
                var grad2 = new Gradient();
                grad.SetKeys(new[] {new GradientColorKey(_colors[Random.Range(0, _colors.Length)], 0.0f)},
                    new[] {new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)});
                grad2.SetKeys(new[] {new GradientColorKey(_colors[Random.Range(0, _colors.Length)], 0.0f)},
                    new[] {new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)});
                col.color = new ParticleSystem.MinMaxGradient(grad, grad2);
                paSy.emission.SetBursts(
                    new[]
                    {
                        new ParticleSystem.Burst((short) vol, (short) vol)
                    });
            }

            var children2 = GetComponentsInChildren<Transform>();
            // for each subemmitter another subemitter is created
            foreach (var child1 in children2)
            {
                if (child1.name != "SubEmitterBirth2") continue;

                var paSy1 = child1.GetComponent<ParticleSystem>();
                var col3 = paSy1.colorOverLifetime;
                col3.enabled = true;
                var grad3 = new Gradient();
                var grad4 = new Gradient();
                grad3.SetKeys(new[] {new GradientColorKey(_colors[Random.Range(0, _colors.Length)], 0.0f)},
                    new[] {new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)});
                grad4.SetKeys(new[] {new GradientColorKey(_colors[Random.Range(0, _colors.Length)], 0.0f)},
                    new[] {new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f)});
                col3.color = new ParticleSystem.MinMaxGradient(grad3, grad4);
            }
        }
    }
}