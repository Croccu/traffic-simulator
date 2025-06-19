using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button playButton;
    public Button backgroundButton; // ← Must be your Image + Button combo

    private bool videoIsPlaying = false;

    void Start()
    {
        playButton.onClick.AddListener(PlayVideo);
        backgroundButton.onClick.AddListener(StopVideo);
        videoPlayer.Stop();
    }

    void PlayVideo()
    {
        videoPlayer.Play();
        playButton.gameObject.SetActive(false);
        videoIsPlaying = true;
    }

    void StopVideo()
    {
        if (videoIsPlaying)
        {
            videoPlayer.Stop();
            playButton.gameObject.SetActive(true);
            videoIsPlaying = false;
        }
    }
}




