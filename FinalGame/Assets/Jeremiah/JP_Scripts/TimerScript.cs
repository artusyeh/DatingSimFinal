using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class TimerScript : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime = 60f; 
    [SerializeField] GameObject playAgainPanel; 

    private bool isBlinking = false;

    void Start()
    {
        if (playAgainPanel != null)
            playAgainPanel.SetActive(false);
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
    }

    void EndGame()
    {
        enabled = false;

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
            timerText.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            timerText.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }
}