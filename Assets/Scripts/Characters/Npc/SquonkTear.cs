using System.Collections;
using UnityEngine;

public class SquonkTear : MonoBehaviour
{
    public Transform tearSprite;
    SquonkTearManager tearManager;
    float positionZ;
    public const float mainGravity = .5f;
    public float gravity = .5f;
    float spriteDisplacementY;
    Vector3 displacedPosition;
    bool fullsize;
    DrawZasYDisplacement displacement;
    Vector3 worldPosition;

    private void Start()
    {
        spriteDisplacementY = GlobalSettings.SpriteDisplacementY;
    }
    private void OnEnable()
    {
        StartCoroutine(GrowTearCo());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    public void SetTear(SquonkTearManager manager, DrawZasYDisplacement displacement)
    {
        transform.position = displacement.transform.position;
        fullsize = false;
        this.displacement = displacement;
        positionZ = displacement.positionZ;
        tearManager = manager;
        tearSprite.localPosition = displacement.GetDisplacedPosition();
        tearSprite.localScale = Vector3.zero;
        
    }

    public void FixedUpdate()
    {
        if (!fullsize)
            return;
       
        positionZ -= gravity * Time.fixedDeltaTime;

        displacedPosition = new Vector3(0, spriteDisplacementY * positionZ, positionZ);
        transform.position = worldPosition;
        tearSprite.localPosition = displacedPosition;
        gravity += 0.03f;
        if (tearSprite.localPosition.z <= 0)
        {
            gravity = mainGravity;
            //tearManager.OnReturnedToPool(this);
            tearManager.LeaveTrail(transform.position);
            gameObject.SetActive(false);
        }
            
    }

    IEnumerator GrowTearCo()
    {
        float timer = 0;
        float waitTime = Random.Range(0.3f, 1.1f);
        while(timer < waitTime)
        {
            timer += Time.deltaTime;
            float s = Mathf.Lerp(0.0f, 1.0f, timer / waitTime);
            Vector3 size = new Vector3(s, s, s);
            if(displacement != null)
                tearSprite.localPosition = displacement.GetDisplacedPosition();
            tearSprite.localScale = size;
            yield return null;
        }
        
        fullsize = true;
        worldPosition = transform.position;
        yield return null;
    }

    
}
