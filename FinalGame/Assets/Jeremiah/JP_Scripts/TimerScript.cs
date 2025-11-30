using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TimerScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] GameObject timerBox;
    [SerializeField] float startingTime = 60f;
    [SerializeField] GameObject playAgainPanel;
    [SerializeField] ParticleSystem heartBreakparticles;
    [SerializeField] ParticleSystem heartParticles;
    [SerializeField] TestScreenShake screenShake;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip urgentTickClip;
    [SerializeField] AudioClip heartClip;
    [SerializeField] AudioClip heartbreakClip;
  //  [SerializeField] AudioSource bgmSource;
   // [SerializeField] AudioClip normalTickClip;

    private float remainingTime;
    private int lastSecond = -1;
    private bool isBlinking = false;

    void Start()
    {
        if (playAgainPanel != null)
            playAgainPanel.SetActive(false);

        remainingTime = startingTime;
        enabled = false;

        if (timerText != null)
        {
            timerText.gameObject.SetActive(false);
        }
        if (timerBox != null)
        {
            timerBox.gameObject.SetActive(false);
        }
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
        if (timerText != null)
        {
            timerText.gameObject.SetActive(true);
        }
        if (timerBox != null)
        {
            timerBox.gameObject.SetActive(true);
        }
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
        {
            timerText.color = Color.white;
            timerText.gameObject.SetActive(false);
        }
        if (timerBox != null)
        {
            timerBox.gameObject.SetActive(false);
            StopAllCoroutines();
        }
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

        if (screenShake != null)
            screenShake.Shake();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            screenShake.Shake(0.5f, 0.5f);
        }

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
        int seconds = Mathf.CeilToInt(remainingTime % 60f);
        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (seconds != lastSecond)
        {
            lastSecond = seconds;

            if (audioSource != null && urgentTickClip != null)
            {
                audioSource.pitch = 1.1f;

                if (remainingTime <= 10f)
                {
                    audioSource.pitch = 0.5f;
                }

                audioSource.PlayOneShot(urgentTickClip);
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