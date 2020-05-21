using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSharpSynth.Synthesis;
using CSharpSynth.Sequencer;

[RequireComponent(typeof(AudioSource))]
public class CSSOscillator : MonoBehaviour
{
    //Public
    public string bankFilePath = "GM Bank/gm";
    public int bufferSize = 1024;
    [Range(0,127)]
    public int midiNote = 60;
    public int midiNoteVolume = 100;
    [Range(0, 127)] //From Piano to Gunshot
    public int midiInstrument = 0;
    public float gain = 1f;

    //Private
    private float[] sampleBuffer;
    private MidiSequencer midiSequencer;
    private StreamSynthesizer midiStreamSynthesizer;

    void Awake()
    {
        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];

        midiStreamSynthesizer.LoadBank(bankFilePath);

        midiSequencer = new MidiSequencer(midiStreamSynthesizer);

        //These will be fired by the midiSequencer when a song plays. Check the console for messages if you uncomment these
        //midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler (MidiNoteOnHandler);
        //midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler (MidiNoteOffHandler);			
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) {
            //Debug.Log("Key P Pressed");
            midiStreamSynthesizer.NoteOn(0, midiNote, midiNoteVolume, midiInstrument);
        }
        if (Input.GetKeyUp(KeyCode.P))
        {
            //Debug.Log("Key P Released");
            midiStreamSynthesizer.NoteOff(0, midiNote);
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            midiNote++;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            midiNote--;
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            midiInstrument++;
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            midiInstrument--;
        }


    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        midiStreamSynthesizer.GetNext(sampleBuffer);

        for (int i = 0; i < data.Length; i++)
        {
            data[i] = sampleBuffer[i] * gain;
        }
    }
    public void MidiNoteOnHandler(int channel, int note, int velocity)
    {
        Debug.Log("NoteOn: " + note.ToString() + " Velocity: " + velocity.ToString());
    }

    public void MidiNoteOffHandler(int channel, int note)
    {
        Debug.Log("NoteOff: " + note.ToString());
    }
}
