using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGravity : MonoBehaviour
{

    public Transform itemObject;
    const float gravity = 20f;
    CurrentGridLocation currentGridLocation;

    public float positionZ;
    public readonly float spriteDisplacementY = 0.27808595f;
    public Vector3 displacedPosition;
    public bool isGrounded;


    [Range(0, 1)]
    public float bounceFriction;
    [Range(0, 10)]
    public float bounciness;
    
    float bounceFactor = 1;
    int dif;

    public int lastLevel;
    bool displacing;
    Vector3 currentPosition;


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.25f);

        currentGridLocation = GetComponent<CurrentGridLocation>();
        lastLevel = currentGridLocation.currentLevel;
        isGrounded = true;
    }

    private void Update()
    {
        if (currentGridLocation == null)
            return;

        if (currentGridLocation.currentLevel < lastLevel && !displacing)
        {
            Displace();
        }
        int levelDiff = currentGridLocation.currentLevel - lastLevel;
        if (levelDiff == 1 && !displacing)
        {
            displacing = true;
            lastLevel = currentGridLocation.currentLevel;
            Invoke("ResetDisplacing", 0.4f);
        }
            

        isGrounded = itemObject.localPosition.y <= 0;
        if (isGrounded && itemObject.localPosition.z > 0)
            itemObject.localPosition = Vector3.zero;
            

        if (!isGrounded)
        {
            
            positionZ -= gravity * Time.deltaTime;
            displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
            itemObject.Translate(displacedPosition * Time.deltaTime);

            if (itemObject.localPosition.y <= 0)
            {
                positionZ = 0;
                displacedPosition = Vector3.zero;
                itemObject.localPosition = Vector3.zero;
                if(bounceFactor >= .1f)
                    Bounce((bounciness + Mathf.Abs(dif)) * bounceFactor);
            }
        }
        
    }

    void Displace()
    {
        displacing = true;
        bounceFactor = 1;
        dif = currentGridLocation.currentLevel - lastLevel;
        float displacement = dif * spriteDisplacementY;
        currentPosition = transform.position;
        transform.position = new Vector3(currentPosition.x, currentPosition.y + displacement, currentPosition.z);
        itemObject.localPosition = new Vector3(itemObject.localPosition.x, itemObject.localPosition.y - displacement, lastLevel - 1);

        Invoke("ResetDisplacing", 0.3f);

        lastLevel = currentGridLocation.currentLevel;
    }

    void ResetDisplacing()
    {
        displacing = false;
    }

    public void Bounce(float bounceAmount)
    {
        positionZ += bounceAmount;
        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        itemObject.transform.Translate(displacedPosition * Time.deltaTime);
        bounceFactor *= bounceFriction;
    }

}
