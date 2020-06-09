using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordSynthButton : MonoBehaviour
{

    public bool recording;
    public CSSOscillator oscillator;
 
    List<float> recTimes;
    List<Button> buttons;


    void Start()
    {
        recording = false;
        recTimes = new List<float>();
        buttons = new List<Button>();
    }

    public void Record()
    {
        recording = !recording;

        if(recording == false)
        {
            // call the playback function
            StartCoroutine(PlayBack());
        }

    }

    // this is a function that allows you to wait for specified time periods before executing code
    // it'll use the arrays to play back the notes with the right time intervals
    private IEnumerator PlayBack()
    {

        buttons[0].onClick.Invoke();
        
        for(int i = 1; i < recTimes.Count; i++)
        {
            yield return new WaitForSeconds(recTimes[i] - recTimes[i-1]);
            buttons[i].onClick.Invoke();
        }

        // at the end of playback, reset recording arrays
        recTimes = new List<float>();
        buttons = new List<Button>();

    }

    // this is added to notes 
    public void NoteClick(string name)
    {
        if (recording)
        {
            // log time of note click and audio clip
            recTimes.Add(Time.time);
            buttons.Add(GameObject.Find(name).GetComponent<Button>());
        }
    }

}
