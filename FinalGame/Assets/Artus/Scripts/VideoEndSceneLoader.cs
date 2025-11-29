using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class VideoEndSceneLoader : MonoBehaviour
{
    [Tooltip("Name of the scene to load after this video finishes.")]
    public string nextSceneName;

    private VideoPlayer videoPlayer;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        if (videoPlayer == null)
        {
            Debug.LogError("VideoEndSceneLoader: No VideoPlayer found on this GameObject.");
        }
    }

    private void OnEnable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    private void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
        }
    }

    private void OnVideoEnd(VideoPlayer vp)
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("VideoEndSceneLoader: nextSceneName is not set.");
            return;
        }

        SceneManager.LoadScene(nextSceneName);
    }
}