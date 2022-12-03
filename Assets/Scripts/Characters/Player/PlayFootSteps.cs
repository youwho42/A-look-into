using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayFootSteps : MonoBehaviour
{
    public List<string> soundToPlay = new List<string>();
    AudioManager audioManager;
    public LayerMask groundSoundsLayer;
    public Vector3 offset;
    


    private void Start()
    {
        audioManager = AudioManager.instance;
        
    }
    public void PlayFootstepSound()
    {
        bool right = PlayerInformation.instance.playerController.facingRight;
        Vector3 newOff = right ? offset : -offset;
        Collider2D hit = Physics2D.OverlapCircle(transform.position + newOff , .01f, groundSoundsLayer);
        PlaySound(hit);

    }

    //public void PlayLeftFootstepSound()
    //{

    //    Collider2D hit = Physics2D.OverlapCircle(transform.position - offset, .01f, groundSoundsLayer);
    //    PlaySound(hit);

    //}

    //public void PlayRightFootstepSound()
    //{

    //    Collider2D hit = Physics2D.OverlapCircle(transform.position + offset, .01f, groundSoundsLayer);

    //    PlaySound(hit);
    //}

    void PlaySound(Collider2D hit)
    {
        if (hit != null)
        {

            for (int i = 0; i < soundToPlay.Count; i++)
            {
                if (soundToPlay[i] == hit.GetComponent<WalkableSound>().walkableSoundName)
                {
                    audioManager.PlaySound(soundToPlay[i]);
                    return;
                }
            }

        }
        audioManager.PlaySound(soundToPlay[0]);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + offset, 0.01f);
        Gizmos.DrawWireSphere(transform.position - offset, 0.01f);
    }
}
