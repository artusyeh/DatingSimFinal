using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
public class WarningTextAnimation : MonoBehaviour
{
    [Header("Text Settings")]
    [SerializeField] TextMeshProUGUI tmpText;
    [SerializeField] TextMeshProUGUI headerText;
    [SerializeField] string warningText = "You thought this was just a date? Some choices have consequences you won’t forget...";
    [SerializeField] string headerString = "WARNING";
    [SerializeField] float fadeInDuration = 2f;
    [SerializeField] float pulseDuration = 1f;
    [SerializeField] Color startColor = Color.white;
    [SerializeField] Color pulseColor = Color.red;

    [Header("Font & Scale Controls")]
    [SerializeField] float fontSize = 36f;
    [SerializeField] Vector3 textScale = Vector3.one;

    [Header("Glitch Settings")]
    [SerializeField] float glitchDuration = 2f;
    [SerializeField] float minInterval = 0.05f;
    [SerializeField] float maxInterval = 0.2f;
    [SerializeField] float jitterAmount = 2f;

    [Header("Scene Transition")]
    [SerializeField] string titleSceneName = "MockTitleScreen";

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip glitchClip;

    void Start()
    {
        if (tmpText != null)
        {
            tmpText.fontSize = fontSize;
            tmpText.rectTransform.localScale = textScale;
            tmpText.text = warningText;

            if (headerText != null)
            {
                headerText.text = headerString;
                headerText.fontSize = fontSize * 1.8f;

                Vector3 pos = tmpText.rectTransform.localPosition;
                headerText.rectTransform.localPosition = pos + new Vector3(345, 100, 0);

                StartCoroutine(PlaySequence());
            }
        }

        IEnumerator PlaySequence()
        {
            tmpText.alpha = 0f;
            tmpText.color = startColor;
            float t = 0f;
            while (t < fadeInDuration)
            {
                t += Time.deltaTime;
                tmpText.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
                yield return null;
            }
            tmpText.alpha = 1f;

            if (audioSource != null && glitchClip != null)
                audioSource.PlayOneShot(glitchClip);
            t = 0f;
            while (t < pulseDuration)
            {
                t += Time.deltaTime;
                float lerp = Mathf.PingPong(t, pulseDuration / 2f) / (pulseDuration / 2f);
                tmpText.color = Color.Lerp(startColor, pulseColor, lerp);
                yield return null;
            }
            tmpText.color = pulseColor;

            // Glitch effect
            yield return StartCoroutine(GlitchRoutine());

            if (!string.IsNullOrEmpty(titleSceneName))
                SceneManager.LoadScene(titleSceneName);
        }

        IEnumerator GlitchRoutine()
        {
            float elapsed = 0f;
            Vector3 originalPos = tmpText.rectTransform.localPosition;

            while (elapsed < glitchDuration)
            {
                tmpText.alpha = Random.value > 0.5f ? 1f : 0.7f;
                tmpText.color = Random.value > 0.5f ? pulseColor : startColor;
                tmpText.rectTransform.localPosition = originalPos +
                    new Vector3(Random.Range(-jitterAmount, jitterAmount), Random.Range(-jitterAmount, jitterAmount), 0);

                float wait = Random.Range(minInterval, maxInterval);
                yield return new WaitForSeconds(wait);
                elapsed += wait;
            }

            // Reset
            tmpText.alpha = 1f;
            tmpText.color = pulseColor;
            tmpText.rectTransform.localPosition = originalPos;
        }
    }
}
