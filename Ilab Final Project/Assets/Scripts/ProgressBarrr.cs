using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    // Start is called before the first frame update
    RectTransform progress;
    float barLength = 500;
    bool looping;
    float loopDuration;
    float startTime;

    // Start is called before the first frame update
    void Start()
    {
        progress = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        if (looping)
        {
            float timeInLoop = (Time.time - startTime);
            float x = Math.Min(timeInLoop / loopDuration * barLength,barLength);
            progress.localPosition = new Vector3(x, 0, 0);
        }
    }

    public void StartLoop(float ld, float st)
    {
        loopDuration = ld;
        startTime = st;
        looping = true;
    }
}
