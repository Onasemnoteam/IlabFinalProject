using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordButton : MonoBehaviour
{

    // booleans that keep track of whether or not functions can be called
    public bool recording;
    bool canPress;
    bool canInvoke;

    // these arrays store buttons and times they were clicked during recording
    List<float> recTimes;
    List<Button> buttons;

    // this adds some lag time before loops but the value's kinda random
    // we'll need to let them select a tempo and have the loops fit in set bars
    public float endDelay = 0.5f;

    void Start()
    {
        recording = false;
        canPress = true;

        recTimes = new List<float>();
        buttons = new List<Button>();

        canInvoke = true;
    }

    public void Record()
    {
        // canPress prevents you from recording while it's playing
        if(canPress)
        {

            if (!recording)
            {
                // reset recording arrays
                recTimes = new List<float>();
                buttons = new List<Button>();
            }

            recording = !recording;
            
        }

    }

    public void Play() 
    {
        if (recording == false && recTimes.Count > 0)
        {
            // call the playback function
            StartCoroutine(PlayBack());
        }
    }

    // this is a function that allows you to wait for specified time periods before executing code
    // it'll use the arrays to play back the notes with the right time intervals
    private IEnumerator PlayBack()
    {
        canPress = false;

        buttons[0].onClick.Invoke();
        
        for(int i = 1; i < recTimes.Count; i++)
        {
            yield return new WaitForSeconds(recTimes[i] - recTimes[i-1]);
            buttons[i].onClick.Invoke();
        }

        canPress = true;

    }

    // this is attached to the note and chord buttons to track when they're pressed while recording
    public void NoteClick(string name)
    {
        if (recording && canInvoke)
        {
            // log time of note click and audio clip
            recTimes.Add(Time.time);
            buttons.Add(GameObject.Find(name).GetComponent<Button>());
        }
    }

    public void AddLoop()
    {
        recording = false;

        if(recTimes.Count > 0)
        {
            // making temporary copies of the arrays to call the loop

            List<float> recTimesTemp = new List<float>();
            List<Button> buttonsTemp = new List<Button>();

            for(int i = 0; i < recTimes.Count; i++)
            {
                recTimesTemp.Add(recTimes[i]);
                buttonsTemp.Add(buttons[i]);
            }

            // loops current recording
            StartCoroutine(Loop(recTimesTemp, buttonsTemp));

            recTimes = new List<float>();
            buttons = new List<Button>();
        }

    }


    private IEnumerator Loop(List<float> t, List<Button> b)
    {
        while(true)
        {
            // call the button sounds, but don't let the sounds that are being looped get recorded
            canInvoke = false;
            b[0].onClick.Invoke();
            canInvoke = true;

            for (int i = 1; i < t.Count; i++)
            {
                yield return new WaitForSeconds(t[i] - t[i - 1]);
                canInvoke = false;
                b[i].onClick.Invoke();
                canInvoke = true;
            }

            yield return new WaitForSeconds(endDelay);
        }
    }


}
