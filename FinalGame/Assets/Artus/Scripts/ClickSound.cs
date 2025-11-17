using UnityEngine;

public class ClickSound : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip clickSound;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlayClick();
        }
    }

    void PlayClick()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
