using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordSynthButton : MonoBehaviour
{

    public bool recording;
    bool canPress;
    public CSSOscillator oscillator;

    List<float> recTimes;
    List<int[]> buttons;


    void Start()
    {
        recording = false;
        canPress = true;

        recTimes = new List<float>();
        buttons = new List<int[]>();
    }

    public void Record()
    {
        if (canPress) {
            recording = !recording;

            if (!recording)
            {
                // call the playback function
                StartCoroutine(PlayBack());
            }
        }

    }

    // this is a function that allows you to wait for specified time periods before executing code
    // it'll use the arrays to play back the notes with the right time intervals
    private IEnumerator PlayBack()
    {
        canPress = false;
        int[] currNote = buttons[0];
        if (currNote[2] == 1) oscillator.PlayNote(currNote[0],currNote[1]);
        else if (currNote[2] == 0) oscillator.StopNote(currNote[0]);

        for (int i = 1; i < recTimes.Count; i++)
        {
            yield return new WaitForSeconds(recTimes[i] - recTimes[i - 1]);
            currNote = buttons[i];
            if (currNote[2] == 1) oscillator.PlayNote(currNote[0], currNote[1]);
            else if (currNote[2] == 0) oscillator.StopNote(currNote[0]);
        }

        // at the end of playback, reset recording arrays
        recTimes = new List<float>();
        buttons = new List<int[]>();
        canPress = true;
    }

    // this is added to notes 
    public void recNote(int note, int volume, int on)
    {
        int[] newNote = { note, volume, on};
        
        recTimes.Add(Time.time);
       
        buttons.Add(newNote);
    }
}
    

