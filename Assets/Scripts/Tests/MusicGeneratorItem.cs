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
        if (isActive)
        {
            MusicGenerator.instance.AddToDictionary(type);
            isInDictionary = true;
        }
            
    }

    private void OnBecameInvisible()
    {
        if (isActive)
        {
            MusicGenerator.instance.RemoveFromDictionary(type);
            isInDictionary = false;
        }
            
    }

    private void OnDisable()
    {
        if (isActive)
        {
            MusicGenerator.instance.RemoveFromDictionary(type);
            isInDictionary = false;
        }
    }

}
