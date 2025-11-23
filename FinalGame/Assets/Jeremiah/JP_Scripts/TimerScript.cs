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
    [SerializeField] ScreenShake screenShake;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip tickClip;

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

    public void StartTimer(bool reset = false)
    {
        if (reset)
            remainingTime = startingTime;

        enabled = true;

        if (heartBreakparticles != null)
            heartBreakparticles.Play();

        if (screenShake != null)
            StartCoroutine(screenShake.Shaking());
    }
    public void StopTimer()
    {
        enabled = false;              
        isBlinking = false;           
        timerText.color = Color.white; 
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


    void EndGame()
    {
        enabled = false;
        isBlinking = false;
        timerText.color = Color.white;

        if (playAgainPanel != null)
            playAgainPanel.SetActive(true);
    }
        }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator BlinkText()
    {
        while (remainingTime > 0 && remainingTime <= 10)
        {
            timerText.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            timerText.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }

  /*  public void OnNoButtonClick()
    {
        remainingTime -= 5f;
        if (remainingTime < 0) remainingTime = 0;

        if (heartBreakparticles != null)
        {
            heartBreakparticles.Play();
        }

        if (screenShake != null)
        {
            StartCoroutine(screenShake.Shaking());
        }
       
        HideButtons();
        
    }
    public void HideButtons()
    {
        if (yesButton != null) yesButton.gameObject.SetActive(false);
        if (noButton != null) noButton.gameObject.SetActive(false);
    }*/
}