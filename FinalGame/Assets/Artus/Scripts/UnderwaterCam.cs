using UnityEngine;

public class UnderwaterCam : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] float rotationAmplitude = 1.5f;  // How much the camera tilts
    [SerializeField] float rotationFrequency = 0.5f;  // How fast it moves
    [SerializeField] float positionAmplitude = 0.05f; // How much it moves up/down
    [SerializeField] float positionFrequency = 1f;    // How fast it moves vertically

    Vector3 startPos;
    Quaternion startRot;

    void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    void Update()
    {
        // looping sway using sine waves
        float time = Time.time;

        // Small vertical bob
        float yOffset = Mathf.Sin(time * positionFrequency) * positionAmplitude;

        // rotation
        float xRot = Mathf.Sin(time * rotationFrequency) * rotationAmplitude;
        float zRot = Mathf.Cos(time * rotationFrequency * 0.8f) * rotationAmplitude;

        transform.localPosition = startPos + new Vector3(0, yOffset, 0);
        transform.localRotation = startRot * Quaternion.Euler(xRot, 0, zRot);
    }
}
