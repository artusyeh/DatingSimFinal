using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContentWarningController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_Text warningText;

    [TextArea]
    [SerializeField] private string fullWarningText;

    [Header("Timing")]
    [SerializeField] private float delayBeforeTyping = 2f;
    [SerializeField] private float typewriterSpeed = 0.04f;
    [SerializeField] private float totalDurationBeforeSceneChange = 8f;

    [Header("Fade Out")]
    [SerializeField] private float fadeOutDuration = 1.5f;

    [Header("Next Scene")]
    [SerializeField] private string nextSceneName;

    void Start()
    {
        if (warningText != null)
        {
            warningText.text = "";
            SetTextAlpha(1f);
        }

        StartCoroutine(ContentWarningRoutine());
    }

    IEnumerator ContentWarningRoutine()
    {

        yield return new WaitForSeconds(delayBeforeTyping);

        // Typewriter effect
        yield return StartCoroutine(TypeText());


        float timeBeforeFade = totalDurationBeforeSceneChange
                    - delayBeforeTyping
                - fadeOutDuration;

        if (timeBeforeFade > 0)
            yield return new WaitForSeconds(timeBeforeFade);

        // Fade out text
        yield return StartCoroutine(FadeOutText());

        // Load next scene
        if (!string.IsNullOrEmpty(nextSceneName))
            SceneManager.LoadScene(nextSceneName);
        else
            Debug.LogWarning("ContentWarningController: Next scene name not set.");
    }

    IEnumerator TypeText()
    {
        warningText.text = "";

        foreach (char c in fullWarningText)
        {
            warningText.text += c;
            yield return new WaitForSeconds(typewriterSpeed);
        }
    }

    IEnumerator FadeOutText()
    {
        float elapsed = 0f;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeOutDuration);
            SetTextAlpha(alpha);
            yield return null;
        }

        SetTextAlpha(0f);
    }

    void SetTextAlpha(float alpha)
    {
        if (warningText == null) return;

        Color c = warningText.color;
        c.a = alpha;
        warningText.color = c;
    }
}
