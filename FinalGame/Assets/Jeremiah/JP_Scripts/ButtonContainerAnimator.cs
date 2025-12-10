using UnityEngine;
using System.Collections;

public class ButtonContainerAnimator : MonoBehaviour
{
    [SerializeField] CanvasGroup buttonGroup;
    [SerializeField] float duration = 1f;

    void Start()
    {
        buttonGroup.alpha = 0; 
       // StartCoroutine(FadeInGroup());
    }

    public void Play()
    {
        StartCoroutine(FadeInGroup());
    }

    IEnumerator FadeInGroup()
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            buttonGroup.alpha = progress; 
            yield return null;
        }
    }
}
