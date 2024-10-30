using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSculptureWall : MonoBehaviour
{

    public SphereSuperShapeActivator superShape;

    

    public void StartHide()
    {
        StartCoroutine(HideCo());

    }

    IEnumerator HideCo()
    {

        float timer = 0;
        float timeToHide = 5f;
        Vector3 startPos = transform.position;
        while (timer < timeToHide)
        {

            float size = Mathf.Lerp(1, 0, timer / timeToHide);
            transform.localScale = new Vector3(size, size, size);

            Vector3 pos = Vector3.Lerp(startPos, superShape.transform.position, timer / timeToHide);
            transform.position = pos;

            timer += Time.deltaTime;
            yield return null;
        }
        gameObject.SetActive(false);
        
        
    }



}
