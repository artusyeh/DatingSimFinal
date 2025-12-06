using UnityEngine;
using System.Collections;
using Unity.Mathematics;

public class ZoomIn : MonoBehaviour
{
    [SerializeField] Camera cam;

    public void StartZoom(float targetZoom, float duration)
    {
        StartCoroutine(LerpZoom(cam.orthographicSize, targetZoom, duration));
    }

    IEnumerator LerpZoom(float startZoom, float endZoom, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;

            float newZoom = Mathf.SmoothStep(startZoom, endZoom, t / duration);

            cam.orthographicSize = newZoom;
            yield return null;
        }
    }
}
