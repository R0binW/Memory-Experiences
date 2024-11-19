using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoLoader : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public string videoUrl;
    
    void PlayVideo()
    {
        videoPlayer.url = videoUrl;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.EnableAudioTrack(0, true);
        videoPlayer.SetDirectAudioVolume(0, 0.3f);
        videoPlayer.Prepare();
    }

    public void SetNewVideoURL(string url)
    {
        videoUrl = url;
        PlayVideo();
    }
}
