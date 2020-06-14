
// things still not working:
// - the recording won't always sync up (you have to press record after the first loop has restarted if you want your audio to sync nicely)
// - random bugs pop up, like weird time delays or things playing more often than they should
// - sometimes things will just take a pause from being played and hop back in later lol

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecordButton : MonoBehaviour
{

    // booleans that keep track of whether or not stuff can be done
    bool recording;
    bool canPress;
    bool canInvoke;
    bool timeOut;
    bool firstLoop;
    bool addLoop;


    // these arrays store buttons and times they were clicked during recording
    List<float> recTimes;
    List<Button> buttons;

    // all time durations are in seconds
    public float endDelay = 0.5f; // delay after one loop ends (sketchy)
    public float loopDuration = 60f; // max duration of first loop

    // keeping track of when recordings start
    float recStartTime;
    float firstLoopStart;
    float flsStored;

    // storing loops in an array and variables that clear them
    List<Coroutine> loops;

    // stuff for button color change
    Button recButton;
    ColorBlock recColors;
    Color yellow = new Color32(251, 235, 146, 255);
    Color red = new Color32(250, 129, 129, 255);


    // start function called at very beginning (initializing everything)
    void Start()
    {

        recording = false;
        canPress = true;

        recTimes = new List<float>();
        buttons = new List<Button>();

        canInvoke = true;

        timeOut = false;

        firstLoop = true;

        recButton = GameObject.Find("record").GetComponent<Button>();
        recColors = recButton.colors;

        loops = new List<Coroutine>();

    }

    void Update()
    {

        if (recording)
        {
            // turns the record button yellow while something is being recorded
            recColors.normalColor = yellow;
            recButton.colors = recColors;

            // check to see if recording has met set amount of time, and if so, stop recording
            if (Time.time - recStartTime >= loopDuration)
            {
                recording = false;
                timeOut = true;
            }

        }

        else
        {
            recColors.normalColor = red;
            recButton.colors = recColors;
        }
    }

    // attached to record button in GUI
    public void Record()
    {

        recording = !recording;

        // canPress prevents you from recording while it's playing
        if (canPress)
        {

            // if we just started recording ...
            if (recording == true)
            {
                recTimes = new List<float>();
                buttons = new List<Button>();

                recStartTime = Time.time;

                if(firstLoop == false)
                {
                    // flsStored stores the first loop start time so that it syncs up properly when "record" is pressed (used to sync loops properly)
                    flsStored = firstLoopStart;
                }
            }
            
        }

    }

    // function to play back what was recorded
    public void Play() 
    {
        if (recording == false && recTimes.Count > 0)
        {
            // call the playback function
            StartCoroutine(PlayBack());
        }
    }

    // this is a function that allows you to wait for specified time periods before executing code
    private IEnumerator PlayBack()
    {
        canPress = false;
        timeOut = false;

        // go through the array of buttons, play the right sounds, and wait for the corresponding time intervals between them

        buttons[0].onClick.Invoke();

        for (int i = 1; i < recTimes.Count; i++)
        {
            yield return new WaitForSeconds(recTimes[i] - recTimes[i - 1]);
            buttons[i].onClick.Invoke();
        }

        canPress = true;

    }


    // this is attached to the note and chord buttons in the GUI to track when they're pressed while recording
    public void NoteClick(string name)
    {
        if (recording && canInvoke && timeOut == false)
        {
            // keep track of which buttons were pressed and at what time
            recTimes.Add(Time.time);
            buttons.Add(GameObject.Find(name).GetComponent<Button>());
        }
    }


    // adds a recorded track to the loop (attached to loop button in GUI)
    public void AddLoop()
    {
        recording = false;

        timeOut = false;

        if (recTimes.Count > 0)
        {
            // making temporary copies of the arrays to call the loop

            List<float> recTimesTemp = new List<float>();
            List<Button> buttonsTemp = new List<Button>();

            for (int i = 0; i < recTimes.Count; i++)
            {
                recTimesTemp.Add(recTimes[i]);
                buttonsTemp.Add(buttons[i]);
            }

            // create and play a new loop
            Coroutine c = StartCoroutine(Loop(recTimesTemp, buttonsTemp, flsStored, recStartTime));
            loops.Add(c);

            // reset arrays and variables

            firstLoop = false;

            recTimes = new List<float>();
            buttons = new List<Button>();
        }

    }


    // this function loops tracks given the recordings you want it to play (currently it just loops them infinitely)
    private IEnumerator Loop(List<float> t, List<Button> b, float fls, float rst)
    {

        // determine which loop was the very first one created
        // when new loops are added, they must sync up with the time they were played relative to the original loop
        bool firstTrack = false;
        if (firstLoop)
        {
            firstTrack = true;
        }

        // while loop will now keep the track looping
        while (true)
        {

            // wait until the original loop has gone back to the start before starting the loop
            while (addLoop == false && firstTrack == false)
            {
                yield return new WaitForSeconds(0.01f);
            }

            // if it's the first loop, update its start time
            if (firstTrack)
            {
                firstLoopStart = Time.time;
                addLoop = false;
            }

            // the first loop won't need delay before it start looping,
            // but every other loop after that will need to sync up with the first loop
            if (firstTrack == false)
            {
                // loops will be added once the first loop has ended. before the loop is played:
                yield return new WaitForSeconds(endDelay);  // it waits for the end delay to pass
                yield return new WaitForSeconds(rst - fls); // it waits for the amount of time you waited to press record (from the start of the loop)
                     // ^ not good because loops will only fit nicely into the original loop if you press record after the loop has started again from the beginning
                yield return new WaitForSeconds(t[0] - rst);  // it waits for the amount of time it took to click a note from when you pressed record
            }

            // call the button sounds, but don't let the sounds that are being looped get recorded
            canInvoke = false;
            b[0].onClick.Invoke();
            canInvoke = true;

            // play the sounds in the loop (same as Playback function)
            for (int i = 1; i < t.Count; i++)
            {
                yield return new WaitForSeconds(t[i] - t[i - 1]);
                canInvoke = false;
                b[i].onClick.Invoke();
                canInvoke = true;
            }

            // for the first loop, addLoop is set to true to allow new loops to be added at this point
            if (firstTrack)
            {
                addLoop = true;
                yield return new WaitForSeconds(endDelay);
            }

        }
    }


    // clear button
    public void clear()
    {

        if(loops.Count > 0)
        {
            // stop running the loops
            for(int i = 0; i < loops.Count; i++)
            {
                StopCoroutine(loops[i]);
            }

            // reset arrays
            recTimes = new List<float>();
            buttons = new List<Button>();
            loops = new List<Coroutine>();

            // reset variables
            recording = false;
            canPress = true;
            canInvoke = true;
            timeOut = false;
            firstLoop = true;
        }

    }


}