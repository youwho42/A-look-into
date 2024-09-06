using Klaxon.GOAD;
using Klaxon.Interactable;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampfireBPInteractor : MonoBehaviour
{
    Campfire fire;
    Collider2D coll;

    private void Start()
    {
        fire = GetComponentInParent<Campfire>();
        coll = GetComponent<Collider2D>();
        GameEventManager.onDayStateChangeEvent.AddListener(CheckForBP);
    }

    private void OnDisable()
    {
        GameEventManager.onDayStateChangeEvent.RemoveListener(CheckForBP);
    }

    void CheckForBP()
    {
        StartCoroutine(CheckForBPCo());
    }

    IEnumerator CheckForBPCo()
    {
        coll.enabled = false;
        yield return new WaitForSeconds(0.1f);
        coll.enabled = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        
        int dif = (int)collision.transform.position.z - (int)transform.position.z;
        if (!collision.CompareTag("NPC") || Mathf.Abs(dif) >= 2 || fire.hasBP)
            return;

        if (collision.TryGetComponent(out GOAD_Scheduler_BP bp))
        {
            bool isDay = RealTimeDayNightCycle.instance.dayState == RealTimeDayNightCycle.DayState.Day;
            if (!fire.isLit && !isDay || fire.isLit && isDay)
            {
                fire.hasBP = true;
                bp.SetFire(fire);
            }

        }

    }
}
