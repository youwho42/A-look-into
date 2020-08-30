using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Firefly : MonoBehaviour
{
    public float flyBaseSpeed;
    float currentSpeed;
    public float mainPositionZ;
    public int hourToDailySpawn;
    public Transform fireflySpritePosition;
    public Animator animator;
    public Light2D lightToFlicker;
    

    GetDestination getDestination;
    Vector3 destination;
    Vector3 spriteDestination;
    bool isFluttering = true;
    bool isFlickering;
    
    float timer;
    float direction;

    Collider2D captureCollider;
    GameObject objectToLandOn;

    float spritePositionZ;
    readonly float spriteDisplacementY = 0.27808595f;
    
    private void Start()
    {
        captureCollider = GetComponent<Collider2D>();
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);

        isFluttering = false;
        fireflySpritePosition.gameObject.SetActive(false);

        
        DayNightCycle.instance.FullHourEventCallBack.AddListener(FireflyLoop);

        getDestination = GetComponent<GetDestination>();
        if (getDestination.chickenPen == null)
        {
            getDestination.chickenPen = PlayerInformation.instance.player;
            SetRandomDestination();
        }
        SetRandomDestination();
        currentSpeed = SetRandomSpeed();
        
    }

    void Update()
    {
        

        if (isFluttering)
        {

            if (!isFlickering)
                StartCoroutine("FlickerLight");

            MoveSprites();


            float distance = Vector2.Distance(transform.position, destination);
            if (distance <= 0.1f)
            {
                SetRandomDestination();
                SetRandomPositionZasY();

            }
            float distanceZasY = Vector2.Distance(fireflySpritePosition.localPosition, spriteDestination);
            if (distanceZasY <= 0.1f)
            {
                SetRandomPositionZasY();
            }
        }

        if (getDestination.chickenPen.gameObject.CompareTag("Player"))
        {
            getDestination.chickenPen = GameObject.FindGameObjectWithTag("OpenFlower").transform;
        }
    }

    public void FireflyLoop(int time)
    {
        
        
        if (time == hourToDailySpawn || time == hourToDailySpawn + 2)
        {
            StartCoroutine("SetFireflyLoopCo");

        }
        
    }
    IEnumerator SetFireflyLoopCo()
    {
        
        yield return new WaitForSeconds(Random.Range(0.5f, 8.0f));
        isFluttering = !isFluttering;
        fireflySpritePosition.gameObject.SetActive(!fireflySpritePosition.gameObject.activeSelf);
    }
    
    IEnumerator FlickerLight()
    {
        isFlickering = true;
        float delay = Random.Range(0.5f, 3.0f);
        yield return new WaitForSeconds(delay);
        float elapsedTime = 0;
        float waitTime = Random.Range(0.1f, 2f);
        float intensity = 0.6f;
        float tempIntensity = intensity + Random.Range(-.3f, .3f);
        while (elapsedTime < waitTime)
        {
            
            lightToFlicker.intensity = Mathf.Lerp(intensity, tempIntensity, (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        lightToFlicker.intensity = tempIntensity;
       
        yield return null;
        isFlickering = false;
    }

    
    void MoveSprites()
    {
        float step = currentSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, destination, step);
        fireflySpritePosition.localPosition = Vector3.MoveTowards(fireflySpritePosition.localPosition, spriteDestination, step * 2);
        captureCollider.offset = fireflySpritePosition.localPosition;
    }

    void SetRandomDestination()
    {
        // Set destination
        destination = getDestination.SetDestination();
        destination.z = mainPositionZ;

        SetFacingDirection();

    }
    void SetFacingDirection()
    {
        // Set facing direction
        Vector2 dir = destination - transform.position;
        direction = Mathf.Sign(dir.x);
        Vector3 theScale = fireflySpritePosition.localScale;
        if (direction > 0)
        {
            
            theScale.x *= -1;
            fireflySpritePosition.localScale = theScale;

        }
    }

    void SetRandomPositionZasY()
    {
        spritePositionZ = SetRandomPositionZ();
        spriteDestination = SetPositionY();
        currentSpeed = SetRandomSpeed();
    }

    float SetRandomPositionZ()
    {
        return Random.Range(0.2f, 0.8f);
    }

    Vector3 SetPositionY()
    {
        return new Vector3(0, spriteDisplacementY * spritePositionZ, spritePositionZ);
    }

    float SetRandomSpeed()
    {
        return flyBaseSpeed + Random.Range(-0.03f, 0.06f);
    }

    void SetLandingDestination(Vector2 landingDestination, float displacement)
    {
        destination = landingDestination;
        destination.z = mainPositionZ;
        spriteDestination = new Vector3(0, spriteDisplacementY * displacement, displacement);
        isFluttering = false;
        
        SetFacingDirection();
    }

    
}
