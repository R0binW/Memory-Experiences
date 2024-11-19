using System.Collections.Generic;
using UnityEngine;

public class MonitorManager : MonoBehaviour
{
    public List<Artifact> artifacts { get; set; }
    public Transform rearSwapPos;
    public static bool videoMode = false;
    public InputFieldPrompt inputFieldPrompt;

    private Monitor temp;
    public Monitor left;
    public Monitor middle;
    public Monitor right;

    private Vector3 rearSwapRotation;
    private Vector3 leftPos;
    private Vector3 leftRotation;
    private Vector3 middlePos;
    private Vector3 middleRotation;
    private Vector3 rightPos;
    private Vector3 rightRotation;

    // Start is called before the first frame update
    void Start()
    {
        UpdatePositions();
    }

    void UpdatePositions()
    {
        rearSwapRotation = rearSwapPos.rotation.eulerAngles;
        leftPos = left.transform.position;
        leftRotation = left.transform.rotation.eulerAngles;
        middlePos = middle.transform.position;
        middleRotation = middle.transform.rotation.eulerAngles;
        rightPos = right.transform.position;
        rightRotation = right.transform.rotation.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        if (left.Busy() || middle.Busy() || right.Busy()) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Switchleft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Switchright();
        }

        if (!inputFieldPrompt.activated && Input.GetKeyDown(KeyCode.Space)) {
            ToggleVideoMode();
        }
    }

    public void Switchleft()
    {
        if (left.index == 0) return;

        AudioManager.instance.PlayMonitorButtonClip();
        UpdatePositions();
        
        right.MoveTo(rearSwapPos.position, rearSwapRotation, leftPos, leftRotation);
        middle.MoveTo(rightPos, rightRotation);
        left.MoveTo(middlePos, middleRotation);

        // L M R
        // N L M

        temp = right;
        right = middle;
        middle = left;
        left = temp;

        left.SetArtifact(artifacts[middle.index - 1], middle.index - 1);
    }

    public void Switchright()
    {
        if (right.index == artifacts.Count - 1) return;

        AudioManager.instance.PlayMonitorButtonClip();
        UpdatePositions();

        left.MoveTo(rearSwapPos.position, rearSwapRotation, rightPos, rightRotation);
        middle.MoveTo(leftPos, leftRotation);
        right.MoveTo(middlePos, middleRotation);

        // L M R
        // M R N

        temp = left;
        left = middle;
        middle = right;
        right = temp;

        right.SetArtifact(artifacts[middle.index + 1], middle.index + 1);
    }

    public void AddArtifact(Artifact a) {
        bool onLastItem = right.index == artifacts.Count - 1;
        artifacts.Add(a);
        if (onLastItem) {
            Switchright();
        }
    }

    public void ToggleVideoMode() {
        videoMode = !videoMode;
        if (videoMode) {
            left.ShowVideo();
            middle.ShowVideo();
            right.ShowVideo();
        } else {
            left.HideVideo();
            middle.HideVideo();
            right.HideVideo();
        }
    }
}
