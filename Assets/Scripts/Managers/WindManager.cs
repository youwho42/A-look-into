using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.Mathematics;
using UnityEngine;

public class WindManager : MonoBehaviour
{
    public static WindManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    Vector2 windDirection;
    float windStrength;
    public Vector2 Wind { get { return windDirection.normalized * windStrength; } }
    public Vector2 WindDirectionNormalized { get { return windDirection.normalized; } }
    public float WindStrength { get { return windStrength; } }

    float directionAngle;
    float directionOffsetX;
    float directionOffsetY;
    float directionIncrement = 0.01f;
    int directionOffsetDirectionX = 1;
    int directionOffsetDirectionY = 1;


    float magnitudeAngle;
    float magnitudeOffsetX;
    float magnitudeOffsetY;
    public float maxMagnitude;
    public float magnitudeIncrement = 0.1f;
    int magnitudeOffsetDirectionX = 1;
    int magnitudeOffsetDirectionY = 1;

    private void Start()
    {
        directionOffsetX = UnityEngine.Random.Range(1, 1000);
        directionOffsetY = UnityEngine.Random.Range(1, 1000);
        magnitudeOffsetX = UnityEngine.Random.Range(1, 1000);
        magnitudeOffsetY = UnityEngine.Random.Range(1, 1000);
        GameEventManager.onTimeTickEvent.AddListener(SetWind);
        SetWind(0);
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetWind);
    }
    void SetWind(int tick)
    {
        GetWindDirection();
        GetWindMagnitude();
    }
    void GetWindMagnitude()
    {
        
        float x = MapNumber.Remap(Mathf.Cos(magnitudeAngle), -1, 1, 0, 1);
        float y = MapNumber.Remap(Mathf.Sin(magnitudeAngle), -1, 1, 0, 1);
        windStrength = maxMagnitude * Mathf.PerlinNoise(x + magnitudeOffsetX, y + magnitudeOffsetY);
        
        magnitudeAngle += magnitudeIncrement;
        
        if (magnitudeAngle >= 2 * Mathf.PI)
        {
            magnitudeAngle = 0;
            magnitudeOffsetX += magnitudeIncrement * magnitudeOffsetDirectionX;
            magnitudeOffsetY += magnitudeIncrement * magnitudeOffsetDirectionY;
            if (magnitudeOffsetX > 1000 || magnitudeOffsetX < 1)
                magnitudeOffsetDirectionX *= -1;
            if (magnitudeOffsetY > 1000 || magnitudeOffsetY < 1)
                magnitudeOffsetDirectionY *= -1;
        }
            
        
    }

    void GetWindDirection()
    {
        float x = MapNumber.Remap(Mathf.Cos(directionAngle), -1, 1, 0, 1);
        float y = MapNumber.Remap(Mathf.Sin(directionAngle), -1, 1, 0, 1);
        float a = MapNumber.Remap(Mathf.PerlinNoise(x + directionOffsetX, y + directionOffsetY), 0, 1, -1, 1) * (2 * Mathf.PI);
        
        windDirection = new Vector2(Mathf.Sin(a), Mathf.Cos(a));
        directionAngle += directionIncrement;
        if(directionAngle >= 2 * Mathf.PI)
        {
            directionAngle = 0;
            directionOffsetX += directionIncrement * directionOffsetDirectionX;
            directionOffsetY += directionIncrement * directionOffsetDirectionY;
            if (directionOffsetX > 1000 || directionOffsetX < 10)
                directionOffsetDirectionX *= -1;
            if (directionOffsetY > 1000 || directionOffsetY < 10)
                directionOffsetDirectionY *= -1;
        }
        
    }
}
