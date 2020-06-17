using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public CSSOscillator Oscillator;
    public RecordButton Recorder;
    public int volume = 100;
    int note;

    private int[] scale = { 58, 60, 62, 63, 65, 67, 69, 70 };
    // Update is called once per frame
    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.keyCode != KeyCode.None)
        {
            switch (e.keyCode)
            {
                case KeyCode.Q:
                    note = scale[0]; break;
                case KeyCode.W:
                    note = scale[1]; break;
                case KeyCode.E:
                    note = scale[2]; break;
                case KeyCode.R:
                    note = scale[3]; break;
                case KeyCode.T:
                    note = scale[4]; break;
                case KeyCode.Y:
                    note = scale[5]; break;
                case KeyCode.U:
                    note = scale[6]; break;
                case KeyCode.I:
                    note = scale[7]; break;
            }
            if (e.type == EventType.KeyDown && Input.GetKeyDown(e.keyCode))
            {
                Oscillator.PlayNote(note, volume);
                Recorder.recNote(note, volume, 1);
            }
            else if (e.type == EventType.KeyUp)
            {
                Oscillator.StopNote(note);
                Recorder.recNote(note, volume, 0);
            }
        }
    }
}
   