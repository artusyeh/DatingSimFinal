using UnityEngine;

public class Buttons : MonoBehaviour
{

    [SerializeField] ParticleSystem heartParticles;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayHearts()
    {
        if (heartParticles != null)
        {
            heartParticles.Play();
        }        
    }
    public void PlayHeartsAtButton(Transform buttonTransform)
    {
        heartParticles.transform.position = buttonTransform.position;
        heartParticles.Play();
    }
}
