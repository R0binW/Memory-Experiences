using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class AudioManager : MonoBehaviour
{
    private AudioSource source;
    public AudioClip loadedImage;
    public AudioClip loadingClip;
    public AudioClip BGM;
    public AudioClip onClick;

    public AudioClip monitorButtonPress;

    public VideoPlayer videoPlayer;
    public bool isMuted;

    public InputFieldPrompt inputField;
    public static AudioManager instance { get; private set;}
  
    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);

        source = GetComponent<AudioSource>();
        PlayBGM();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M) && !inputField.activated)
        {
            if (!isMuted)
            {
                isMuted = true;
                source.volume = 0;
                videoPlayer.SetDirectAudioVolume(0, 0.3f);
            }
            else
            {
                isMuted = false;
                source.volume = 0.5f;
                videoPlayer.SetDirectAudioVolume(0, 0);  //Mute the audio
            }
        }
    }

    public void PlayBGM()
    {
        source.clip = BGM;
        source.Play(0);
    }
    public void PlayRecordClip()
    {
        source.PlayOneShot(onClick);
    }

    public void PlayMonitorButtonClip()
    {
        source.PlayOneShot(monitorButtonPress);
    }
}
