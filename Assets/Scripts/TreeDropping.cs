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
    public void SetDropping(Vector3 position, float maxHeight, Sprite droppingSprite)
    {
        transform.position = position;
        droppingItemSprite.sprite = droppingSprite;
        droppingItemShadow.sprite = droppingSprite;
        var h = Random.Range(maxHeight * 0.33f, maxHeight * 0.9f);
        droppingItemSprite.transform.localPosition = new Vector3(0, spriteDisplacementY * h, h);
        finalSpeed = speed + Random.Range(-.1f, 0.11f);
        //Invoke("ResetDrop", 2.0f);
    }

    private void Update()
    {
        if(droppingItemSprite.transform.localPosition.y > 0.001f)
        {
            var wind = WindManager.instance.GetWindDirectionFromPosition(transform.position);
            float windSpeed = WindManager.instance.GetWindMagnitude(transform.position);
            float disp = droppingItemSprite.transform.localPosition.z;
            disp -= finalSpeed * Time.deltaTime;
            

            Vector3 pos = new Vector3(0, spriteDisplacementY * disp, disp);
            droppingItemSprite.transform.localPosition = pos;
            var np = (Vector3)wind.normalized * windSpeed * 0.008f * Time.deltaTime;
            np.x += amplitude * Mathf.Sin(theta * frequency);
            theta += Time.deltaTime;
            
            transform.position += np;
        }
        else
        {
            ResetDrop();
        }
    }

    void ResetDrop()
    {
        TreeDroppingManager.instance.treeDroppingPool.Release(this);
    }

    
}
