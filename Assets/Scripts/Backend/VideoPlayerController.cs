using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoPlayController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Button playButton;
    public Button backgroundButton;

    private bool videoIsPlaying = false;

    // ✅ Replace this with your actual GitHub RAW link
    private string githubVideoUrl = "https://raw.githubusercontent.com/MarcusPuust/Traffic_video/main/output.mp4";

    void Start()
    {
        // Set the video URL from GitHub
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = githubVideoUrl;

        // Prepare video (optional: preload)
        videoPlayer.Prepare();

        // UI listeners
        playButton.onClick.AddListener(PlayVideo);
        backgroundButton.onClick.AddListener(StopVideo);

        videoPlayer.Stop();
    }

    void PlayVideo()
    {
        if (videoPlayer.isPrepared)
        {
            videoPlayer.Play();
            playButton.gameObject.SetActive(false);
            videoIsPlaying = true;
        }
        else
        {
            Debug.LogWarning("Video not prepared yet.");
        }
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





