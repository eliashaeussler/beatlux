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



	void Start () {
        colors = new Color[] { Settings.Active.ColorScheme.Colors[0], Settings.Active.ColorScheme.Colors[1], Settings.Active.ColorScheme.Colors[2], Settings.Active.ColorScheme.Colors[3], 
            Settings.Active.ColorScheme.Colors[4], Settings.Active.ColorScheme.Colors[5]};
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
        
        /**
        //decible stuff
        float sum = 0;

        for (int i = 0; i < qSamples; i++)
        {
            sum += samples[i] * samples[i];
        }

        rmsValue = Mathf.Sqrt(sum / qSamples);
        dbValue = 20 * Mathf.Log10(rmsValue / refValue);
        print(dbValue);
        */

        if (AudioPeer.freqBands[0] >= bassMax * 15)
        {              
            GameObject.Find("BigBang").GetComponent<ParticleSystem>().Play();
            //print(bassMax);
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
            

        }
	}
}
