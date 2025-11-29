using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float startingTime = 60f;
    [SerializeField] GameObject playAgainPanel;
    [SerializeField] ParticleSystem heartBreakparticles;
    [SerializeField] ParticleSystem heartParticles;
    [SerializeField] ScreenShake screenShake;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip tickClip;
    [SerializeField] AudioClip heartClip;
    [SerializeField] AudioClip heartbreakClip;

    private float remainingTime;
    private int lastSecond = -1;
    private bool isBlinking = false;

    void Start()
    {
        if (playAgainPanel != null)
            playAgainPanel.SetActive(false);

        remainingTime = startingTime;
        enabled = false;
    }

    /// <summary>
    /// Start or resume the timer.
    /// If reset == true, reset remaining time to startingTime.
    /// Does NOT play any FX by itself.
    /// </summary>
    public void StartTimer(bool reset = false)
    {
        if (reset)
            remainingTime = startingTime;

        enabled = true;
    }

    /// <summary>
    /// Stop counting down; do NOT change remaining time.
    /// Also stop blinking and reset text color.
    /// Does NOT play any FX by itself.
    /// </summary>
    public void StopTimer()
    {
        enabled = false;
        isBlinking = false;
        if (timerText != null)
            timerText.color = Color.white;

        StopAllCoroutines();
    }

    /// <summary>
    /// Fully reset timer to starting value and stop it.
    /// </summary>
    public void ResetTimer()
    {
        remainingTime = startingTime;
        StopTimer();
    }

    /// <summary>
    /// Play heartbreak FX (bad outcome / wrong choice).
    /// Explicitly stops heartParticles first.
    /// </summary>
    public void PlayHeartbreakFX()
    {
        if (heartParticles != null)
            heartParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (heartBreakparticles != null)
            heartBreakparticles.Play();

        if (audioSource != null && heartbreakClip != null)
            audioSource.PlayOneShot(heartbreakClip);

        // if (screenShake != null)
        //     StartCoroutine(screenShake.Shaking());
    }

    /// <summary>
    /// Play positive heart FX (good outcome / reconciliation).
    /// Explicitly stops heartbreakParticles first.
    /// </summary>
    public void PlayHeartFX()
    {
        if (heartBreakparticles != null)
            heartBreakparticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (heartParticles != null)
            heartParticles.Play();

        if (audioSource != null && heartClip != null)
            audioSource.PlayOneShot(heartClip);
    }

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime <= 10 && !isBlinking)
            {
                StartCoroutine(BlinkText());
                isBlinking = true;
            }
        }
        else
        {
            remainingTime = 0;
            EndGame();
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (remainingTime <= 10)
        {
            if (seconds != lastSecond)
            {
                lastSecond = seconds;
                if (audioSource != null && tickClip != null)
                {
                    audioSource.PlayOneShot(tickClip);
                }
            }
        }
    }

    void EndGame()
    {
        enabled = false;
        isBlinking = false;
        if (timerText != null)
            timerText.color = Color.white;

        if (playAgainPanel != null)
            playAgainPanel.SetActive(true);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator BlinkText()
    {
        while (remainingTime > 0 && remainingTime <= 10)
        {
            if (timerText != null)
                timerText.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            if (timerText != null)
                timerText.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }
}