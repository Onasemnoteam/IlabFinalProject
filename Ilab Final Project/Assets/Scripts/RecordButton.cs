
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
    List<int[]> buttons;
    List<List<float>> allTimes;
    List<List<int[]>> allButtons;
    public Button[] buttonIndex;

    // all time durations are in seconds
    public float endDelay = 0.5f; // delay after one loop ends (sketchy)
    public float loopDuration = 60f; // max duration of first loop
    public CSSOscillator oscillator;
    float firstLoopDuration;
    float truefirstLoopDuration;

    // keeping track of when recordings start and end
    float recStartTime;
    float firstLoopStart;
    float firstLoopEnd;
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
        buttons = new List<int[]>();
        allTimes = new List<List<float>>();
        allButtons = new List<List<int[]>>();

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

            if(recording == false)
            {
                if(firstLoop)
                {
                    firstLoopEnd = Time.time;
                    
                    for (int i = 0; i < 128; i++) oscillator.StopNote(i);
                    recTimes.Add(Time.time);
                    int[] temp = { 0, 0, 0 };
                    buttons.Add(temp);
                }
            }

            // if we just started recording ...
            if (recording == true)
            {
                recTimes = new List<float>();
                buttons = new List<int[]>();

                recStartTime = Time.time;

                if (firstLoop)
                {
                    recTimes.Add(recStartTime);
                    int[] temp = { 0, 0, 0 };
                    buttons.Add(temp);
                }
                else{
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
            
            StartCoroutine(PlayBack(recTimes, buttons, oscillator.midiInstrument));
            
        }
    }

    // this is a function that allows you to wait for specified time periods before executing code
    private IEnumerator PlayBack(List<float> t, List<int[]> b, int instrument)
    {
        canPress = false;
        timeOut = false;
        //Debug.Log("doot");
        // go through the array of buttons, play the right sounds, and wait for the corresponding time intervals between them
       

        for (int i = 1; i < t.Count; i++)
        {
            //Debug.Log(t.Count + " " + i);
            //Debug.Log(i + ":" + (t[i]-t[i-1]));
            if(i>0) yield return new WaitForSeconds(t[i] - t[i - 1]);
            int[] currNote = b[i];
            //Debug.Log(currNote[0]+" "+currNote[2]);
            //Debug.Log(currNote[0]);F
            if (currNote[2] == 1) oscillator.PlayNote(currNote[0], currNote[1],instrument);
            else if (currNote[2] == 0) oscillator.StopNote(currNote[0]);
            else if (currNote[2] == 2) buttonIndex[currNote[0]].onClick.Invoke();
            //Debug.Log("waw");
            //Debug.Log(t.Count);
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
            int buttInd = Array.IndexOf(buttonIndex, GameObject.Find(name).GetComponent<Button>());
            int[] temp = { buttInd, 0, 2 };
            buttons.Add(temp);
        }
    }

    public void recNote(int note, int volume, int on)
    {
        int[] newNote = { note, volume, on };

        recTimes.Add(Time.time);

        buttons.Add(newNote);
    }

    // adds a recorded track to the loop (attached to loop button in GUI)
    public void AddLoop()
    {
        
        recording = false;

        timeOut = false;


        if (recTimes.Count > 0)
        {
            if (firstLoop)
            {
                if (firstLoopEnd == 0)
                {
                    firstLoopEnd = Time.time;
                    for (int i = 0; i < 128; i++) oscillator.StopNote(i);

                    recTimes.Add(firstLoopEnd);
                    int[] temp = { 0, 0, 0 };
                    buttons.Add(temp);
                }
                firstLoopDuration = firstLoopEnd - recStartTime;
                truefirstLoopDuration = firstLoopEnd - recStartTime;
            }
            // making temporary copies of the arrays to call the loop

            List<float> tempt = new List<float>();
            for (int i = 0; i < recTimes.Count; i++) tempt.Add(recTimes[i]);
            allTimes.Add(tempt);
            List<int[]> tempb = new List<int[]>();
            for (int i = 0; i < buttons.Count; i++) tempb.Add(buttons[i]);
            allButtons.Add(tempb);
            // create and play a new loop
            
            Coroutine c = StartCoroutine(Loop(tempt, tempb, flsStored, recStartTime, oscillator.midiInstrument));
            loops.Add(c);
            
            

            // reset arrays and variables

            firstLoop = false;

            recTimes = new List<float>();
            buttons = new List<int[]>();
        }

    }


    // this function loops tracks given the recordings you want it to play (currently it just loops them infinitely)
    private IEnumerator Loop(List<float> t, List<int[]> b, float fls, float rst, int instrument)
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
            //while (addLoop == false && firstTrack == false)
            //{
            //    yield return new WaitForSeconds(0.01f);
            //}

            // if it's the first loop, update its start time
            if (firstTrack)
            {
                // tell progress bar that looping has started
                GameObject.Find("loop progress").GetComponent<ProgressBar>().StartLoop(firstLoopDuration, Time.time);
                firstLoopStart = Time.time;
                //addLoop = false;
            }

            // the first loop won't need delay before it start looping,
            // but every other loop after that will need to sync up with the first loop
            if (!firstTrack)
            {
                Debug.Log(t[0] - fls);
                Debug.Log(t[t.Count-1] - t[0]);
                Debug.Log((t[0] - fls) % firstLoopDuration);
                float waitTime = (t[0] - fls) % firstLoopDuration;

                // loops will be added once the first loop has ended. before the loop is played:
                yield return new WaitForSecondsRealtime(waitTime);  // it waits for the end delay to pass
            }
            //float startTime = Time.time;
            // play the sounds in the loop (same as Playback function)
            Debug.Log(t.Count+" "+b.Count);

            yield return StartCoroutine(PlayBack(t, b, instrument));

            //else StartCoroutine(PlayBack(t, b, instrument));
            //yield return new WaitForSecondsRealtime(firstLoopDuration);
            //Debug.Log(firstTrack);

                //Debug.Log("Actual Loop Length: "+(Time.time - startTime));
                //Debug.Log(t[t.Count - 1] - t[0]);
                //Debug.Log("firstLoopDuration: "+firstLoopDuration);
                // firstLoopDuration = Time.time - startTime;

                // for the first loop, addLoop is set to true to allow new loops to be added at this point
                //if (firstTrack)
                //{
                //addLoop = true;
                //yield return new WaitForSeconds();
                //}

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
            //Stop all synth notes
            for (int i = 0; i < 128; i++) oscillator.StopNote(i);
            // reset arrays
            recTimes = new List<float>();
            buttons = new List<int[]>();
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