using UnityEngine;
using System.Collections;


public class TestScreenShake : MonoBehaviour
{
    Vector3 originalPos;

    void Awake()
    {
        originalPos = transform.localPosition;
    }

    public void Shake(float duration = 0.7f, float magnitude = 0.3f)
    {
        StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float damper = 1f - (elapsed / duration); 
            float x = Mathf.Sin(elapsed * 40f) * magnitude * damper;
            float y = Mathf.Cos(elapsed * 35f) * magnitude * damper;

            transform.localPosition = originalPos + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
