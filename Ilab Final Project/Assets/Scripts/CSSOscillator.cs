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
    public int midiInstrument = 0;
    public float gain = 1f;
    public RecordButton Recorder;

    //Private
    private int bufferSize = 1024;
    private int midiNote = 60;
    private int midiNoteVolume = 100;
    private float[] sampleBuffer;
    private MidiSequencer midiSequencer;
    private StreamSynthesizer midiStreamSynthesizer;

    int[] instruments = { 0, 4, 24, 45, 53, 56, 75, 115, 116, 117 };
    TMPro.TMP_Dropdown instrumentSelect;

    void Awake()
    {
        midiStreamSynthesizer = new StreamSynthesizer(44100, 2, bufferSize, 40);
        sampleBuffer = new float[midiStreamSynthesizer.BufferSize];

        midiStreamSynthesizer.LoadBank(bankFilePath);

        midiSequencer = new MidiSequencer(midiStreamSynthesizer);

        instrumentSelect = GameObject.Find("Instrument Select").GetComponent<TMPro.TMP_Dropdown>();

        //These will be fired by the midiSequencer when a song plays. Check the console for messages if you uncomment these
        //midiSequencer.NoteOnEvent += new MidiSequencer.NoteOnEventHandler (MidiNoteOnHandler);
        //midiSequencer.NoteOffEvent += new MidiSequencer.NoteOffEventHandler (MidiNoteOffHandler);			
    }

    void Update()
    {
        midiInstrument = instruments[instrumentSelect.value];
    }

    public void PlayNote(int note, int volume, int instrument = -1) {

        if(instrument == -1)
        {
            instrument = midiInstrument;
        }

        midiNote = note;
        midiNoteVolume = volume;
        midiStreamSynthesizer.NoteOn(0, midiNote, midiNoteVolume, instrument);
        Recorder.recNote( note, volume, 1);
    }

    public void StopNote(int note) {
        midiStreamSynthesizer.NoteOff(0, note);
        Recorder.recNote( note, 0, 0);
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
