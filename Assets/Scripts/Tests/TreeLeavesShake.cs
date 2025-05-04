using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeLeavesShake : MonoBehaviour
{
    public List<Transform> leafObjects = new List<Transform>();
    public float amplitude = 3;
    public float frequency = 10;
    public float increment = 0.01f;
    public TreeShadows shadows;



    [ContextMenu("shake!")]
    public void ShakeLeaves()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeLeavesCo());
    }





    IEnumerator ShakeLeavesCo()
    {
        float angle = 0;
        float freq = frequency;
        float amp = amplitude;
        List<int> dir = new List<int>();
        List<float> angleOffsets = new List<float>();
        for (int i = 0; i < leafObjects.Count; i++)
        {
            dir.Add(Random.value < 0.5f ? -1 : 1);
            angleOffsets.Add(Random.Range(-Mathf.PI, Mathf.PI));
        }
        while (amp > 0)
        {
            for (int i = 0; i < leafObjects.Count; i++)
            {
                var a = Mathf.Sin((angle + angleOffsets[i]) * freq) * dir[i];
                Vector3 eulerAngle = new Vector3(0, 0, a * amp);
                leafObjects[i].transform.localEulerAngles = eulerAngle;
                
                shadows.subShadowSprites[i].transform.localEulerAngles = eulerAngle;
                shadows.subNightShadowSprites[i].transform.localEulerAngles = eulerAngle;
            }
            angle += increment;
            if (angle > Mathf.PI * 2)
                angle = 0;
            amp -= increment;
            freq -= increment;
            yield return null;
        }

    }


}
