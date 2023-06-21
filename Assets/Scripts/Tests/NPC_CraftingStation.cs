using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_CraftingStation : MonoBehaviour
{
    public Animator animator;
    public GameObject particles;
    public AudioSource audioSource;

    private void Start()
    {
        if (particles)
            SetParticles(false);
    }

    public void SetCraftingOn()
    {
        animator.SetBool("IsCrafting", true);
        if(particles)
            SetParticles(true);
        audioSource.Play();
    }

    public void SetCraftingOff()
    {
        animator.SetBool("IsCrafting", false);
        if (particles)
            SetParticles(false);

        audioSource.Stop();
    }

    void SetParticles(bool active)
    {
        if (particles)
            particles.SetActive(active);
    }
}
