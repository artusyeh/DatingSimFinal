using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSounds : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;

    [Header("Visual Flash")]
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] float flashSpeed = 2f;   
    [SerializeField] float flashStrength = 0.2f;
    private bool isHovering = false;

    public void Update()
    {
        if(isHovering && canvasGroup != null)
        {
            float alpha = 1f - (Mathf.Sin(Time.time * flashSpeed) * 0.5f + 0.5f) * flashStrength;
            canvasGroup.alpha = alpha;
        }
        else if (canvasGroup != null) 
        {
            canvasGroup.alpha = 1f;
        }

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void OnPointerExit(PointerEventData Data)
    {
        isHovering = false;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }
}
