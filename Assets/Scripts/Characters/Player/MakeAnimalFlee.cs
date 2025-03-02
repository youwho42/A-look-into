using Klaxon.GOAD;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeAnimalFlee : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!enabled) return;
        if (!collision.gameObject.CompareTag("Animal"))
            return;
        if (collision.transform.position.z != transform.position.z)
            return;

        if (collision.TryGetComponent(out IAnimal animal))
        {
            var currentClipInfo = PlayerInformation.instance.playerAnimator.GetCurrentAnimatorClipInfo(0);
            if (currentClipInfo[0].clip.name == "SitOnGround")
                return;

            animal.FleePlayer(transform);
            return;
        }
        if (collision.TryGetComponent(out GOAD_Scheduler_CF cokernutFlump))
        {
            cokernutFlump.FleePlayer(transform);
        }
    }

}
