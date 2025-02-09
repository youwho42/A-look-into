using UnityEngine;

public class FixableObject : MonoBehaviour
{
    [HideInInspector]
    public bool hasBeenFixed;
    public GameObject brokenObject;
    public GameObject fixedObject;

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
}
