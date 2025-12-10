using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimator : MonoBehaviour
{
    [SerializeField] RectTransform[] buttons;
    [SerializeField] float moveDistance = 200f;
    [SerializeField] float duration = 1f;

    void Start()
    {
    }

    public void Play()
    {
        StartCoroutine(AnimateAllButtons());
    }

    IEnumerator AnimateAllButtons()
    {
        List<(RectTransform btn, CanvasGroup cg, Vector2 startPos, Vector2 endPos)> data
            = new List<(RectTransform, CanvasGroup, Vector2, Vector2)>();

        foreach (RectTransform btn in buttons)
        {
            CanvasGroup cg = btn.GetComponent<CanvasGroup>();
            if (cg == null) cg = btn.gameObject.AddComponent<CanvasGroup>();

            Vector2 endPos = btn.anchoredPosition;
            Vector2 startPos = endPos - new Vector2(0, moveDistance);

            btn.anchoredPosition = startPos;
            cg.alpha = 0;

            data.Add((btn, cg, startPos, endPos));
        }

        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            foreach (var item in data)
            {
                item.btn.anchoredPosition = Vector2.Lerp(item.startPos, item.endPos, progress);
                item.cg.alpha = progress;
            }

            yield return null;
        }

        // Snap to final state
        foreach (var item in data)
        {
            item.btn.anchoredPosition = item.endPos;
            item.cg.alpha = 1;
        }
    }
}