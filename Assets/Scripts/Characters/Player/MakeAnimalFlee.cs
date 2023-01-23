using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeAnimalFlee : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var currentClipInfo = PlayerInformation.instance.playerAnimator.GetCurrentAnimatorClipInfo(0);
        if (currentClipInfo[0].clip.name == "SitOnGround")
            return;

        if (collision.TryGetComponent(out IAnimal animal))
        {
           animal.FleePlayer();
        }
    }

}
