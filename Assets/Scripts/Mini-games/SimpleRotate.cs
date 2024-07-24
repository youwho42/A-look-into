using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SimpleRotate : MonoBehaviour
{
    public float speed;
    int direction;
    public bool rotateOnUpdate;
    Vector3 startRotation;

    private void Start()
    {
        startRotation = transform.eulerAngles;
    }

    void Update()
    {
        if (rotateOnUpdate)
            transform.Rotate(0, 0, speed * direction * Time.deltaTime);
        
            
    }
    
    public void RandomizeDirection()
    {
        direction = Random.Range(0, 2) * 2 - 1;
    }
    
    public void SetRotationDirection(int directionToSet)
    {
        direction = directionToSet;
    }

    public void ResetStartRotation()
    {
        transform.eulerAngles = startRotation;
    }
    public void SetRotation(int rotation)
    {
        transform.Rotate(0, 0, rotation);
    }
    public void SetEulerRotation(int rotation)
    {
        transform.eulerAngles = new Vector3(0, 0, rotation);
        
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
    public void StartToFromRotation(int start, int end, float timeToRotate)
    {
        StartCoroutine(RotateToFromCo(start, end, timeToRotate));
    }
   
    public void StopToFromRotation()
    {
        StopAllCoroutines();
    }

    IEnumerator RotateToFromCo(int start, int end, float timeToRotate)
    {
        float timer = 0.0f;
        float timeToTake = timeToRotate;
        while (timer < timeToTake)
        {
            float newRotation = Mathf.Lerp(start, end, timer / timeToTake);
            transform.eulerAngles = new Vector3(0, 0, newRotation);
            timer += Time.deltaTime;
            yield return null;
        }
        int newStart = end;
        end = start;
        
        StartCoroutine(RotateToFromCo(newStart, end, timeToRotate));
    }
}
