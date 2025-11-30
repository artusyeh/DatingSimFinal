using System.Collections;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public class TestScreenShake : MonoBehaviour
{
    Vector3 originalPos;

    [Header("Good Choice Shake)")]
    [SerializeField] float goodDuration = 0.3f;
    [SerializeField] float goodMagnitude = 0.2f;

    [Header("Bad Choice Shake)")]
    [SerializeField] float badDuration = 0.5f;
    [SerializeField] float badMagnitude = 0.4f;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void ShakeGood()
    {
        StartCoroutine(DoVerticalShake(goodDuration, goodMagnitude));
    }

    public void ShakeBad()
    {
        StartCoroutine(DoHorizontalShake(badDuration, badMagnitude));
    }

    IEnumerator DoVerticalShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float damper = 1f - (elapsed / duration);
            float y = Mathf.Sin(elapsed * 20f) * magnitude * damper;

            transform.localPosition = originalPos + new Vector3(0, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }

    IEnumerator DoHorizontalShake(float duration, float magnitude)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float damper = 1f - (elapsed / duration);
            float x = Mathf.Sin(elapsed * 40f) * magnitude * damper;

            transform.localPosition = originalPos + new Vector3(x, 0, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPos;
    }
}

