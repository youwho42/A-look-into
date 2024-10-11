
using UnityEngine;
using NoiseTest;

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

    
    public float maxMagnitude;
    
    int currentSeed;
    float currentZ;

    OpenSimplexNoise openSimplexNoise;

    private void Start()
    {
        currentSeed = Random.Range(0, 10000);
        openSimplexNoise = new OpenSimplexNoise(currentSeed);

       
        GameEventManager.onTimeTickEvent.AddListener(SetWind);
        SetWind(0);
        
    }
    private void OnDisable()
    {
        GameEventManager.onTimeTickEvent.RemoveListener(SetWind);
    }
    void SetWind(int tick)
    {
        currentZ += 0.01f;
        
        
    }
    public float GetWindMagnitude(Vector3 position)
    {
        float offset = 2500;
        float scale = 10.0f;

        float a = (float)openSimplexNoise.Evaluate(position.x / scale + offset, position.y / scale + offset, currentZ + (position.z / scale + offset));
        a = NumberFunctions.RemapNumber(a, -1.0f, 1.0f, 0.0f, 1.0f);
        return Mathf.Abs(maxMagnitude * a);

        
    }

    

    public Vector2 GetWindDirectionFromPosition(Vector3 position)
    {
        float offset = 5000;
        float scale = 10.0f;

        float a = (float)openSimplexNoise.Evaluate(position.x / scale + offset, position.y / scale + offset, (currentZ + position.z) / (scale + offset)) * (2 * Mathf.PI);

        var dir = new Vector2(Mathf.Sin(a), Mathf.Cos(a));
        return dir;
    }

    
}
