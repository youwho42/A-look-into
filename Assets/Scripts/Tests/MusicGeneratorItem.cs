using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MusicGeneratorItem : MonoBehaviour
{

    public SoundType type;

    private void OnBecameVisible()
    {
        MusicGenerator.instance.AddToDictionary(type);
    }

    private void OnBecameInvisible()
    {
        MusicGenerator.instance.RemoveFromDictionary(type);
    }
}
