using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VideoModeText : MonoBehaviour
{
    TextMeshProUGUI text;

    void Awake() {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Update()
    {
        text.text = MonitorManager.videoMode? "Video" : "Picture";   
    }
}
