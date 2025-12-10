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
            timerText.gameObject.SetActive(false);

        if (timerBox != null)
            timerBox.gameObject.SetActive(false);
    }

    ///    /// Start or resume the timer.

    public void StartTimer(bool reset = false)
    {
        if (reset)
            remainingTime = startingTime;

        enabled = true;

        if (timerText != null)
            timerText.gameObject.SetActive(true);

        if (timerBox != null)
            timerBox.gameObject.SetActive(true);
    }

    /// Stop timer and hide UI.
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

    public void ResetTimer()
    {
        remainingTime = startingTime;
        StopTimer();
    }

    public void PlayHeartbreakFX()
    {
        if (heartParticles != null)
            heartParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (heartBreakparticles != null)
            heartBreakparticles.Play();

        if (audioSource != null && heartbreakClip != null)
            audioSource.PlayOneShot(heartbreakClip);

        if (screenShake != null)
            screenShake.ShakeBad();
    }

    public void PlayHeartFX()
    {
        if (heartBreakparticles != null)
            heartBreakparticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (heartParticles != null)
            heartParticles.Play();

        if (audioSource != null && heartClip != null)
            audioSource.PlayOneShot(heartClip);

        if (screenShake != null)
            screenShake.ShakeGood();
    }

    public void DeductTime(float seconds)
    {
        remainingTime -= seconds;
        if (remainingTime < 0)
            remainingTime = 0;
    }

    public void ForceEndTimer()
    {
        remainingTime = 0;
        EndGame();
    }

    void Update()
    {
        // Countdown logic
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

        // FIXED TIMER DISPLA   Y LOGIC
        // Always show clean "01:00" when starting at 60
        if (Mathf.Approximately(remainingTime, startingTime))
        {
            if (timerText != null)
                timerText.text = "01:00";
            return;
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.CeilToInt(remainingTime % 60f);

        // Prevent weird "0:60" case
        if (seconds == 60)
        {
            minutes += 1;
            seconds = 0;
        }

        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        // Play ticking sound once per second
        if (seconds != lastSecond)
        {
            lastSecond = seconds;

            if (audioSource != null && urgentTickClip != null)
            {
                audioSource.pitch = 1.1f;

                if (remainingTime <= 10f)
                    audioSource.pitch = 0.5f;

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

        DialogueManager dm = Object.FindFirstObjectByType<DialogueManager>();
        if (dm != null)
            dm.enabled = false;
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
