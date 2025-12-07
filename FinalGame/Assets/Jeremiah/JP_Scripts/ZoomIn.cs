using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

public class ZoomIn : MonoBehaviour
{
    [SerializeField] Camera cam;

    private float defaultZoom;
    private Vector3 defaultPos;

    void Awake()
    {
        defaultZoom = cam.orthographicSize;
        defaultPos = cam.transform.position;
    }
    public void StartZoom(float targetZoom, float duration)
    {
        StartCoroutine(LerpZoom(cam.orthographicSize, targetZoom, duration));
    }
    public void StartZoomAndPan(float targetZoom, float duration, Vector3 targetPosition, float holdTime)
    {
        StartCoroutine(LerpZoomAndPan(
            cam.orthographicSize, targetZoom, duration,
            cam.transform.position, targetPosition, holdTime));
    }

    IEnumerator LerpZoom(float startZoom, float endZoom, float duration)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;

            cam.orthographicSize = Mathf.SmoothStep(startZoom, endZoom, progress);

            yield return null;
        }
    }

    IEnumerator LerpZoomAndPan(float startZoom, float endZoom, float duration,
                               Vector3 startPos, Vector3 endPos, float holdTime)
    {
        float t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            cam.orthographicSize = Mathf.SmoothStep(startZoom, endZoom, progress);

            cam.transform.position = Vector3.Lerp(startPos, endPos, progress);

            yield return null;
        }

        yield return new WaitForSeconds(holdTime);

        float resetTime = 0;
        float resetDuration = 2f; 
        while (resetTime < resetDuration)
        {
            resetTime += Time.deltaTime;
            float progress = resetTime / resetDuration;

            cam.orthographicSize = Mathf.SmoothStep(endZoom, defaultZoom, progress);
            cam.transform.position = Vector3.Lerp(endPos, defaultPos, progress);

            yield return null;
        }
    }
}

