using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
    public double frequency = 261.63;
    public float gain = 0f;

    double increment;
    double phase;
    double samplingFrequency = 48000.0;

    public float volume = 0.1f;

    public bool usingAudio;

    public float[] frequencies;
    public int index;
    public int index2;

    public AudioClip c;
    public AudioClip d;
    public AudioClip e;
    public AudioClip f;
    public AudioClip g;
    public AudioClip a;
    public AudioClip b;
    public AudioClip c2;

    AudioClip[] audioClips;

    AudioSource audioSource;

    const float PI = Mathf.PI;


    private void Start()
    {
        index = 0;
        index2 = 0;
        audioSource = GetComponent<AudioSource>();

        frequencies = new float[13];

        float startFreq = 261.6f;

        for (int i = 0; i < frequencies.Length; i++)
        {
            frequencies[i] = startFreq * Mathf.Pow(2, (float)i / 12);
        }

        audioClips = new AudioClip[8];
        audioClips[0] = c;
        audioClips[1] = d;
        audioClips[2] = e;
        audioClips[3] = f;
        audioClips[4] = g;
        audioClips[5] = a;
        audioClips[6] = b;
        audioClips[7] = c2;

    }


    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (usingAudio)
            {
                audioSource.clip = audioClips[index2];
                audioSource.Play();
                index2++;
                index2 %= audioClips.Length;

            }

            else
            {
                gain = volume;
            }

            frequency = frequencies[index];
            index++;
            index %= frequencies.Length;
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            gain = 0;
        }
        
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (usingAudio == false)
        {
            increment = frequency * 2.0 * PI / samplingFrequency;

            for (int i = 0; i < data.Length; i++)
            {
                phase += increment;

                data[i] = (float)gain * Mathf.Sin((float)phase);

                if (phase > (Mathf.PI * 2))
                {
                    phase = 0.0;
                }
            }
        }

    }
}
