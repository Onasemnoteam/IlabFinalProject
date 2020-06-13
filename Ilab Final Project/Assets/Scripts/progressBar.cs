using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class progressBar : MonoBehaviour
{
    RectTransform progress;
    int x;
    // Start is called before the first frame update
    void Start()
    {
        progress = gameObject.GetComponent<RectTransform>();
        x = 0;
    }

    // Update is called once per frame
    void Update()
    {
        x++;
        progress.localPosition = new Vector3(x,0,0);
    }
}
