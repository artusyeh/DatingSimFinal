using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TitleAnimation : MonoBehaviour
{
    [Header("Logo Pieces")]
    [SerializeField] CanvasGroup title;
    [SerializeField] RectTransform clockRect;
    [SerializeField] CanvasGroup clockCanvasGroup;
    [SerializeField] CanvasGroup heartCanvasGroup;

    [Header("Timing")]
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] float dropDuration = 1f;
    [SerializeField] float heartDuration = 1f;
    [SerializeField] float delayBetween = 0.5f;

    [Header("Clock Drop Settings")]
    [SerializeField] float dropHeight = 300f;

    [Header("Button Animation Settings")]
    [SerializeField] ButtonAnimator buttonAnimator;
    [SerializeField] ButtonContainerAnimator buttonContainerAnimator;

    [Header("Flash Settings")]
    [SerializeField] Image flashImage;
    [SerializeField] float flashDuration = 0.3f;

    void Start()
    {
        clockCanvasGroup.alpha = 0;
        heartCanvasGroup.alpha = 0;
        StartCoroutine(PlaySequence());

        if (flashImage != null)
            flashImage.color = new Color(1f, 1f, 1f, 0f);
    }

    IEnumerator PlaySequence()
    {
        yield return StartCoroutine(FadeIn(title, fadeDuration));
        yield return StartCoroutine(DropIn(clockRect, clockCanvasGroup, dropDuration));
        yield return new WaitForSeconds(delayBetween);

        StartCoroutine(FadeIn(heartCanvasGroup, heartDuration));
        buttonAnimator.Play();

        yield return new WaitForSeconds(heartDuration);

        if (flashImage != null)
            yield return StartCoroutine(ScreenFlash());

        buttonContainerAnimator.Play();
    }

    IEnumerator FadeIn(CanvasGroup cg, float duration)
    {
        cg.alpha = 0;
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = t / duration;
            yield return null;
        }
        cg.alpha = 1;
    }

    // ----------------------------
    // NEW BOUNCE DROP-IN ANIMATION
    // ----------------------------
    IEnumerator DropIn(RectTransform rt, CanvasGroup cg, float duration)
    {
        Vector2 endPos = rt.anchoredPosition;
        Vector2 startPos = endPos + new Vector2(0, dropHeight);

        rt.anchoredPosition = startPos;
        cg.alpha = 0f;

        float t = 0f;
        float s = 1.70158f; // bounce strength

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            // EaseOutBack bounce
            float eased = 1f + (--progress) * progress * ((s + 1f) * progress + s);
            eased = Mathf.Clamp01(eased);

            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, eased);
            cg.alpha = eased;

            yield return null;
        }

        // OPTIONAL: Squash + stretch impact bounce for extra juice
        rt.localScale = new Vector3(1.05f, 0.95f, 1f);
        yield return new WaitForSeconds(0.05f);
        rt.localScale = Vector3.one;

        rt.anchoredPosition = endPos;
        cg.alpha = 1f;
    }

    IEnumerator ScreenFlash()
    {
        float half = flashDuration / 2f;
        float t = 0f;

        // Fade in
        while (t < half)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(0f, 1f, t / half);
            flashImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        t = 0f;
        // Fade out
        while (t < half)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, t / half);
            flashImage.color = new Color(1f, 1f, 1f, alpha);
            yield return null;
        }

        flashImage.color = new Color(1f, 1f, 1f, 0f);
    }
}
