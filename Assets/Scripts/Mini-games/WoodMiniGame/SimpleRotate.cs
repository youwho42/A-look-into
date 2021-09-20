using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SimpleRotate : MonoBehaviour
{
    public float speed;
    int direction;
    public bool rotateOnUpdate;

  
    void Update()
    {
        if(rotateOnUpdate)
            transform.Rotate(0, 0, speed * direction * Time.deltaTime);
    }
    
    public void RandomizeDirection()
    {
        direction = Random.Range(0, 2) * 2 - 1;
    }

    public void RandomizeRotation()
    {
        int rand = Random.Range(0, 360);
        transform.Rotate(0, 0, rand);
    }
    public void AnimateRandomizeRotation(float time)
    {
        
        StartCoroutine(RotateCo(time));
    }

    IEnumerator RotateCo(float time)
    {
        Debug.Log("Reached coroutine");
        
        int rand = Random.Range(0, 360);
        float elapsedTime = 0;
        float waitTime = time;
        Vector3 currentRotation = transform.eulerAngles;
        while (elapsedTime < waitTime)
        {
            currentRotation.z = Mathf.Lerp(currentRotation.z, rand, (elapsedTime / waitTime));

            transform.eulerAngles = currentRotation;
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        
        yield return null;
    }
}
