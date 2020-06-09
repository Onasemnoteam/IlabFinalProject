using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHandler : MonoBehaviour
{
    public CSSOscillator Oscillator;
    public int volume = 100;

    private int[] scale = { 58, 60, 62, 63, 65, 67, 69, 70 };
    // Update is called once per frame
    private void OnGUI()
    {
        Event e = Event.current;
        if (e.isKey && e.keyCode != KeyCode.None)
        {
            if (e.type == EventType.KeyDown && Input.GetKeyDown(e.keyCode))
            {
                switch (e.keyCode)
                {
                    case KeyCode.Q:
                        Oscillator.PlayNote(scale[0], volume); break;
                    case KeyCode.W:
                        Oscillator.PlayNote(scale[1], volume); break;
                    case KeyCode.E:
                        Oscillator.PlayNote(scale[2], volume); break;
                    case KeyCode.R:
                        Oscillator.PlayNote(scale[3], volume); break;
                    case KeyCode.T:
                        Oscillator.PlayNote(scale[4], volume); break;
                    case KeyCode.Y:
                        Oscillator.PlayNote(scale[5], volume); break;
                    case KeyCode.U:
                        Oscillator.PlayNote(scale[6], volume); break;
                    case KeyCode.I:
                        Oscillator.PlayNote(scale[7], volume); break;
                }
            }
            else if (e.type == EventType.KeyUp)
            {
                switch (e.keyCode)
                {
                    case KeyCode.Q:
                        Oscillator.StopNote(scale[0]); break;
                    case KeyCode.W:
                        Oscillator.StopNote(scale[1]); break;
                    case KeyCode.E:
                        Oscillator.StopNote(scale[2]); break;
                    case KeyCode.R:
                        Oscillator.StopNote(scale[3]); break;
                    case KeyCode.T:
                        Oscillator.StopNote(scale[4]); break;
                    case KeyCode.Y:
                        Oscillator.StopNote(scale[5]); break;
                    case KeyCode.U:
                        Oscillator.StopNote(scale[6]); break;
                    case KeyCode.I:
                        Oscillator.StopNote(scale[7]); break;
                }
            }
        }
    }
}
   