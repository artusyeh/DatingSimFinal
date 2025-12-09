using UnityEngine;
using System.Collections;

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

    void Start()
    {
        clockCanvasGroup.alpha = 0;
        heartCanvasGroup.alpha = 0;
            StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        yield return StartCoroutine(FadeIn(title, fadeDuration));
      //  yield return new WaitForSeconds(delayBetween);
        yield return StartCoroutine(DropIn(clockRect, clockCanvasGroup, dropDuration)); yield return new WaitForSeconds(delayBetween);
        yield return StartCoroutine(FadeIn(heartCanvasGroup, heartDuration));

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

    IEnumerator DropIn(RectTransform rt, CanvasGroup cg, float duration)
    {
        Vector2 endPos = rt.anchoredPosition;
        Vector2 startPos = endPos + new Vector2(0, dropHeight);
        rt.anchoredPosition = startPos;
        cg.alpha = 0;

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            rt.anchoredPosition = Vector2.Lerp(startPos, endPos, progress);
            cg.alpha = progress;
 

            yield return null;
        }

        rt.anchoredPosition = endPos;
        cg.alpha = 1;
    }
}
