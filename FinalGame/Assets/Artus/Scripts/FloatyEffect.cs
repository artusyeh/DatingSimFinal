using UnityEngine;


public class FloatyEffect : MonoBehaviour
{
    [Header("Float Settings")]
    public float floatAmplitude = 10f;   // How far it moves up/down
    public float floatSpeed = 1f;        // Speed of the floating motion

    [Header("Rotation Sway (Optional)")]
    public bool enableSway = true;
    public float swayAngle = 5f;         // Max angle tilt
    public float swaySpeed = 1f;         // Speed of rotation sway

    private RectTransform rectTransform;
    private Vector3 startPos;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform != null ? rectTransform.anchoredPosition : transform.localPosition;
    }

    void Update()
    {
        float time = Time.time;

        // Vertical float
        float floatOffset = Mathf.Sin(time * floatSpeed) * floatAmplitude;

        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = startPos + new Vector3(0f, floatOffset, 0f);
        }
        else
        {
            transform.localPosition = startPos + new Vector3(0f, floatOffset, 0f);
        }

        // Rotation sway
        if (enableSway)
        {
            float angle = Mathf.Sin(time * swaySpeed) * swayAngle;
            transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        }
    }
}
