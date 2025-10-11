using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDropping : MonoBehaviour
{
    float spriteDisplacementY = GlobalSettings.SpriteDisplacementY;

    public SpriteRenderer droppingItemSprite;
    public SpriteRenderer droppingItemShadow;
    float speed = 0.5f;
    float finalSpeed;
    float theta;
    public float amplitude = 0.005f;
    public float frequency = 0.9f;
    Transform dropTransform;
    Transform shadowTransform;
    Vector3 rot;
    float rotDir;
    public void SetDropping(Vector3 position, float maxHeight, Sprite droppingSprite)
    {
        dropTransform = droppingItemSprite.transform;
        shadowTransform = droppingItemShadow.transform;
        transform.position = position;
        droppingItemSprite.sprite = droppingSprite;
        droppingItemShadow.sprite = droppingSprite;
        var h = Random.Range(maxHeight * 0.33f, maxHeight * 0.9f);
        rot = new Vector3(0, 0, Random.Range(0, 360));
        dropTransform.localPosition = new Vector3(0, spriteDisplacementY * h, h);
        finalSpeed = speed + Random.Range(-.1f, 0.11f);
        rotDir = Mathf.Sign(Random.Range(-1, 1));
        //Invoke("ResetDrop", 2.0f);
    }

    private void FixedUpdate()
    {
        if(dropTransform.localPosition.y > 0.001f)
        {
            var wind = WindManager.instance.GetWindDirectionFromPosition(transform.position);
            float windSpeed = WindManager.instance.GetWindMagnitude(transform.position);
            float disp = dropTransform.localPosition.z;
            disp -= finalSpeed * Time.fixedDeltaTime;
            
            rot.z += windSpeed * rotDir;
            dropTransform.eulerAngles = rot;
            shadowTransform.eulerAngles = rot;

            Vector3 pos = new Vector3(0, spriteDisplacementY * disp, disp);
            dropTransform.localPosition = pos;
            var np = (Vector3)wind.normalized * windSpeed * 0.008f * Time.deltaTime;
            np.x += amplitude * Mathf.Sin(theta * frequency);
            theta += Time.fixedDeltaTime;
            
            transform.position += np;
        }
        else
        {
            ResetDrop();
        }
    }

    void ResetDrop()
    {
        gameObject.SetActive(false);
    }

    
}
