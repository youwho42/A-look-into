using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePlant : MonoBehaviour
{
    public GrassSway sway;
    public Bubbles bubbles;
    
    

    private void Start()
    {
        EndBubbles();
    }

    void EndBubbles()
    {
        bubbles.bubblesActive = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && collision.transform.position.z == transform.position.z)
        {
            sway.SwaySoft();
            bubbles.bubblesActive = true;
            CancelInvoke();
            Invoke("EndBubbles", 15);
        }
    }
}
