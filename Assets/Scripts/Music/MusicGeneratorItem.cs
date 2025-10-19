using UnityEngine;

public class MusicGeneratorItem : MonoBehaviour
{

    public SoundType type;
    [HideInInspector]
    public bool isActive;
    [HideInInspector]
    public bool isInDictionary;

    private void Start()
    {
        isActive = true;
    }

    private void OnBecameVisible()
    {
        AddToDictionary();
    }

    private void OnBecameInvisible()
    {
        RemoveFromDictionary();
    }

    private void OnDisable()
    {
        RemoveFromDictionary();
    }

    public void AddToDictionary()
    {
        if (isActive)
        {
            MusicGenerator.instance.AddToDictionary(type);
            isInDictionary = true;
        }
    }

    public void RemoveFromDictionary()
    {
        if (isActive)
        {
            MusicGenerator.instance.RemoveFromDictionary(type);
            isInDictionary = false;
        }
    }
}
