using Klaxon.Interactable;
using System.Collections;
using UnityEngine;

public class FixableObject : MonoBehaviour
{
    [HideInInspector]
    public bool hasBeenFixed;
    public GameObject brokenObject;
    public GameObject fixedObject;
    public ParticleSystem particles;
    public FixingSounds fixSound;
    private void Start()
    {
        brokenObject.SetActive(true);
        fixedObject.SetActive(false);
    }

    public void SetObjectFromSave(bool state)
    {
        hasBeenFixed = state;
        if (!state)
            return;
        brokenObject.SetActive(false);
        fixedObject.SetActive(true);

    }

    public void StartBreakObject(float time)
    {
        StartCoroutine(BreakObjectCo(time));
    }

    IEnumerator BreakObjectCo(float time)
    {
        
        yield return new WaitForSeconds(time);
        hasBeenFixed = false;
        brokenObject.SetActive(true);
        brokenObject.GetComponent<SpriteRenderer>().color = Color.white;
        
        fixedObject.SetActive(false);
    }
}
