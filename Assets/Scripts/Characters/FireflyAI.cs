using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class FireflyAI : MonoBehaviour
{
    public float roamingArea;
    
    public CircleCollider2D captureCollider;
    
    CharacterFlight flight;
    public Animator animator;
    bool isSleeping;
    public Transform home;
    DrawZasYDisplacement displacmentZ;

    public Light2D lightToFlicker;
    bool isFlickering;

    [SerializeField]
    public FlyingState currentState;

    public enum FlyingState
    {
        isFlying,
        isLanding,
        isAtDestination
    }

    private void Start()
    {

        DayNightCycle.instance.FullHourEventCallBack.AddListener(SetSleepOrWake);
       
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        flight = GetComponent<CharacterFlight>();

        displacmentZ = home.GetComponent<DrawZasYDisplacement>();
       if(DayNightCycle.instance.hours < 20 || DayNightCycle.instance.hours > 22)
        {
            flight.SetDestination(home.position, displacmentZ.displacedPosition);
            currentState = FlyingState.isLanding;
            lightToFlicker.gameObject.SetActive(false);
            isSleeping = true;
        }
        else
        {
            isSleeping = false;
            flight.SetRandomDestination(roamingArea);
            currentState = FlyingState.isFlying;
        }
    }

    private void Update()
    {

        switch (currentState)
        {
            case FlyingState.isFlying:

                if (!isFlickering)
                    StartCoroutine("FlickerLight");

                flight.Move();
                
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.01f)
                {
                    if (!isSleeping)
                    {
                        flight.SetRandomDestination(roamingArea);
                    }
                    else
                    {
                        flight.SetDestination(home.position, displacmentZ.displacedPosition);
                        currentState = FlyingState.isLanding;
                    }

                }
                captureCollider.offset = flight.characterSprite.localPosition;
                break;


            case FlyingState.isLanding:

                if (!isFlickering)
                    StartCoroutine("FlickerLight");

                flight.Move();
                if (Vector2.Distance(transform.position, flight.currentDestination) <= 0.001f)
                {
                    
                    
                    currentState = FlyingState.isAtDestination;
                }
                captureCollider.offset = flight.characterSprite.localPosition;
                break;


            case FlyingState.isAtDestination:

                lightToFlicker.gameObject.SetActive(false);
                
                break;
        }
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

    

    public void SetSleepOrWake(int time)
    {
        if (time == 22)
        {

            flight.SetDestination(home.position, displacmentZ.displacedPosition);
            currentState = FlyingState.isLanding;

            isSleeping = true;

        }
        else if (time == 20)
        {
            isSleeping = false;
            flight.SetRandomDestination(roamingArea);
            currentState = FlyingState.isFlying;
            lightToFlicker.gameObject.SetActive(true);
        }
    }

}
