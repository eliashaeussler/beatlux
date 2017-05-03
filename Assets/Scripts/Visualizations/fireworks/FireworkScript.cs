using UnityEngine;
using System.Collections;

public class FireworkScript : MonoBehaviour {

    public int band;
    public float sScale, mScale;
    float[] samples = new float [1024];
    Color[] colors = new Color[] { Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow, Color.white };
	void Start () {
	
	}

	void Update () {

        // Getting the volumedata 
        GameObject.Find("AudioSource").GetComponent<AudioSource>().GetOutputData(samples, 0);
        float vol = 0;
        foreach (float sample in samples)
        {
            if (sample >= 0)
            {
                vol += sample;
            }
            else vol -= sample;
        }

        // if below a certain threshhold, nothing happens
        if (AudioPeer.freqBands[band] < 0)
        {
            this.GetComponent<ParticleSystem>().Stop();
        }
        else
        {
            this.GetComponent<ParticleSystem>().Play();
            this.GetComponent<ParticleSystem>().startSpeed = 30+25 * AudioPeer.freqBands[band];
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
            

        }
	}
}
