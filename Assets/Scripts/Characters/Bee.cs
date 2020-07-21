using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : MonoBehaviour
{
    public float flyBaseSpeed;
    float currentSpeed;
    public float mainPositionZ;

    public Transform butterflySpritePosition;
    public Animator animator;

    public LayerMask landingLayer;
    public float detectionRadius;

    GetDestination getDestination;
    Vector3 destination;
    Vector3 spriteDestination;
    bool isFluttering = true;
    bool isLanding = false;
    bool isLanded = false;
    float timeToIdle;
    float timeToResetFlower = 3.0f;
    float timer;
    float direction;

    GameObject objectToLandOn;

    float spritePositionZ;
    readonly float spriteDisplacementY = 0.27808595f;

    private void Start()
    {
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);


        getDestination = GetComponent<GetDestination>();
        SetRandomDestination();
        currentSpeed = SetRandomSpeed();
    }

    void Update()
    {
        Collider2D[] hit = Physics2D.OverlapCircleAll(transform.position, detectionRadius, landingLayer);
        if (hit.Length > 0 && isFluttering)
        {
            GetNearestFlower(hit);
        }

        if (isLanding)
        {

            MoveSprites();
            float distance = Vector2.Distance(transform.position, destination);
            float distanceZasY = Vector2.Distance(butterflySpritePosition.localPosition, spriteDestination);
            if (distance <= 0.001f && distanceZasY <= 0.001f)
            {
                animator.SetBool("IsLanded", true);
                timeToIdle = Random.Range(0.5f, 10.0f);
                isLanded = true;
                isLanding = false;
            }
        }
        if (isLanded)
        {

            timer += Time.deltaTime;
            if (timer >= timeToIdle)
            {
                if (objectToLandOn != null)
                {
                    StartCoroutine(ResetOldFlower(objectToLandOn));
                }
                isLanded = false;
                animator.SetBool("IsLanded", false);
                isFluttering = true;
                timer = 0;
            }
        }

        if (isFluttering)
        {


            MoveSprites();

            float distance = Vector2.Distance(transform.position, destination);
            if (distance <= 0.1f)
            {
                SetRandomDestination();
                SetRandomPositionZasY();

            }
            float distanceZasY = Vector2.Distance(butterflySpritePosition.localPosition, spriteDestination);
            if (distanceZasY <= 0.1f)
            {
                SetRandomPositionZasY();
            }
        }
    }
    public void GetNearestFlower(Collider2D[] colliders)
    {
        // Find nearest item.
        Collider2D nearest = null;
        float distance = 0;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("OpenFlower"))
            {
                float tempDistance = Vector3.Distance(transform.position, colliders[i].transform.position);
                if (nearest == null || tempDistance < distance)
                {
                    nearest = colliders[i];
                    distance = tempDistance;
                }
            }

        }
        if (nearest != null)
        {
            objectToLandOn = nearest.gameObject;
            objectToLandOn.tag = "ClosedFlower";
            SetLandingDestination(objectToLandOn.transform.position, objectToLandOn.GetComponent<DrawZasYDisplacement>().positionZ);

        }

    }
    IEnumerator ResetOldFlower(GameObject flowerToReset)
    {
        GameObject tempObject = flowerToReset;
        yield return new WaitForSeconds(7f);
        if (tempObject != null)
        {
            tempObject.tag = "OpenFlower";
        }

    }
    void MoveSprites()
    {
        float step = currentSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(transform.position, destination, step);
        butterflySpritePosition.localPosition = Vector3.MoveTowards(butterflySpritePosition.localPosition, spriteDestination, step * 2);
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
        butterflySpritePosition.GetComponent<SpriteRenderer>().flipX = direction > 0;
    }

    void SetRandomPositionZasY()
    {
        spritePositionZ = SetRandomPositionZ();
        spriteDestination = SetPositionY();
        currentSpeed = SetRandomSpeed();
    }

    float SetRandomPositionZ()
    {
        return Random.Range(0.5f, 2.0f);
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
        isLanding = true;
        SetFacingDirection();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
