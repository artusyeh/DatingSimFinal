using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NonVideoSceneTransitions : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] Animator transition;
    [SerializeField] float transitionTime = 1f;

    [Header("UI Sounds")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;

    public void LoadNextScene()
    {
        StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void LoadSceneByName(string sceneName)
    {
        StartCoroutine(LoadSceneByNameCoroutine(sceneName));
    }

    IEnumerator LoadScene(int sceneIndex)
    {
        if (transition != null)
            transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneIndex);
    }

   public IEnumerator LoadSceneByNameCoroutine(string sceneName)
    {
        if (transition != null)
            transition.SetTrigger("Start");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneName);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null && audioSource != null)
            audioSource.PlayOneShot(hoverSound);
    }

    public void PlayClickSound()
    {
        if (clickSound != null && audioSource != null)
            audioSource.PlayOneShot(clickSound);
    }
}
