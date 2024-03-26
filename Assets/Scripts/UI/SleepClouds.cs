using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SleepClouds : MonoBehaviour
{
    [Serializable]
    public struct CloudType
    {
        public List<Sprite> cloudSprites;
    }
    
    public List<CloudType> cloudTypes = new List<CloudType>();
    public int currentType;
    public float speed;
    public float size;
    public Image cloudImage;
    public RectTransform cloudtransform;
    public int currentIndex;
    int currentAnimFrame;

    public void ResetCloud(bool start = true)
    {
        speed = UnityEngine.Random.Range(1f, 4f);
        size = UnityEngine.Random.Range(.8f, 1.7f);
        currentType = UnityEngine.Random.Range(0, cloudTypes.Count);
        currentIndex = 0;
        if(start)
            currentIndex = UnityEngine.Random.Range(0, cloudTypes[currentType].cloudSprites.Count);
        currentAnimFrame = 0;
        cloudImage.sprite = cloudTypes[currentType].cloudSprites[currentIndex];
        cloudtransform.localScale = new Vector3(size, size, size);

        cloudtransform.anchoredPosition = new Vector2(UnityEngine.Random.Range(-350f, 50f), UnityEngine.Random.Range(-30f, 100f));
    }
    public void CloudStep(int frameCount)
    {
        

        float xPos = cloudtransform.anchoredPosition.x + (frameCount * speed);
        xPos = xPos >= 50 || xPos <= -350 ? xPos >= 50 ? -350 : 50 : xPos;
        
        cloudtransform.anchoredPosition = new Vector2(xPos, cloudtransform.anchoredPosition.y);
        currentIndex = (int)nfmod((currentIndex + frameCount), cloudTypes[currentType].cloudSprites.Count - 1);
        cloudImage.sprite = cloudTypes[currentType].cloudSprites[currentIndex];

        if (currentIndex == 0)
            ResetCloud(false);
        
        
    }
    
    float nfmod(float a, float b)
    {
        return a - b * Mathf.Floor(a / b);
}
}
