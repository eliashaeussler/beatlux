using UnityEngine;
using System.Collections;

public class FireworkScript : MonoBehaviour {

    public int band;
    public int extraHeight;
    float[] samples = new float [1024];
    Color[] colors;
    float volMax=0;
    float bassMax = 0;

    int qSamples = 1024;
    float refValue = 0.01f;
    float rmsValue;
    float dbValue;


    // Initialising all the given colors
	void Start () {
        colors = new Color[] { Settings.Active.ColorScheme.Colors[0], Settings.Active.ColorScheme.Colors[1], Settings.Active.ColorScheme.Colors[2], Settings.Active.ColorScheme.Colors[3], 
            Settings.Active.ColorScheme.Colors[4], Settings.Active.ColorScheme.Colors[5]};
        GameObject.Find("BigBang").GetComponent<ParticleSystem>().Stop();
	}

	void Update () {
        // Getting the volumedata 
        GameObject.Find("BigBang").GetComponent<ParticleSystem>().Stop();
		AudioListener.GetOutputData(samples, 0);
        float vol = 0;
        foreach (float sample in samples)
        {
            if (sample >= 0)
            {
                vol += sample;
            }
            else vol -= sample;
        }

        if (volMax < vol) volMax = vol;
        
        // if a volumedifference is big enough, a big rocket is fired
        if (AudioPeer.freqBands[0] >= bassMax * 15)
        {
            ParticleSystem sys = GameObject.Find("SubEmitterDeathBig").GetComponent<ParticleSystem>();
            ParticleSystem sys1 = GameObject.Find("SubEmitterDeathBig2").GetComponent<ParticleSystem>();
            var col1 = sys.colorOverLifetime;
            var col2 = sys1.colorOverLifetime;
            col1.enabled = true;
            col2.enabled = true;
            Gradient grad11 = new Gradient();
            Gradient grad12 = new Gradient();
            grad11.SetKeys(new GradientColorKey[] { new GradientColorKey(colors[Random.Range(0, colors.Length)], 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
            grad12.SetKeys(new GradientColorKey[] { new GradientColorKey(colors[Random.Range(0, colors.Length)], 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
            col1.color = new ParticleSystem.MinMaxGradient(grad11, grad12);
            col2.color = new ParticleSystem.MinMaxGradient(grad11, grad12);
            GameObject.Find("BigBang").GetComponent<ParticleSystem>().Play();
        }
        bassMax = AudioPeer.freqBands[0];

        // if below a certain threshhold, nothing happens
        if (AudioPeer.freqBands[band] <= 0)
        {
            this.GetComponent<ParticleSystem>().Stop();
        }
        else
        {
            this.GetComponent<ParticleSystem>().Play();
            this.GetComponent<ParticleSystem>().startSpeed = 10+ 25 * AudioPeer.freqBands[band]+extraHeight;
            //this.GetComponent<ParticleSystem>().startLifetime = 0.5f + AudioPeer.freqBands[band];
            this.GetComponent<ParticleSystem>().emission.SetBursts(
                new ParticleSystem.Burst[]{
                new ParticleSystem.Burst(0, (short)(10*AudioPeer.freqBands[band]))
            });
            var children = this.GetComponentsInChildren<Transform>();
            foreach (var child in children)
            {
                if (child.name == "SubEmitterDeath")
                {
                    ParticleSystem paSy = child.GetComponent<ParticleSystem>();
                    paSy.startLifetime = 0.4f+(vol / volMax);
                    var col = paSy.colorOverLifetime;
                    col.enabled = true;
                    Gradient grad = new Gradient();
                    Gradient grad2 = new Gradient();
                    grad.SetKeys(new GradientColorKey[] { new GradientColorKey(colors[Random.Range(0,colors.Length)], 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
                    grad2.SetKeys(new GradientColorKey[] { new GradientColorKey(colors[Random.Range(0, colors.Length)], 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
                    col.color = new ParticleSystem.MinMaxGradient(grad, grad2);
                    paSy.emission.SetBursts(
                        new ParticleSystem.Burst[]{
                        new ParticleSystem.Burst((short)(vol), (short)(vol))
            });
                }
            }

            var children2 = this.GetComponentsInChildren<Transform>();
            foreach (var child1 in children2)
            {
                if (child1.name == "SubEmitterBirth2")
                {
                    ParticleSystem paSy1 = child1.GetComponent<ParticleSystem>();
                    var col3 = paSy1.colorOverLifetime;
                    col3.enabled = true;
                    Gradient grad3 = new Gradient();
                    Gradient grad4 = new Gradient();
                    grad3.SetKeys(new GradientColorKey[] { new GradientColorKey(colors[Random.Range(0, colors.Length)], 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
                    grad4.SetKeys(new GradientColorKey[] { new GradientColorKey(colors[Random.Range(0, colors.Length)], 0.0f) }, new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) });
                    col3.color = new ParticleSystem.MinMaxGradient(grad3, grad4);
                }
            }
        }
	}
}
