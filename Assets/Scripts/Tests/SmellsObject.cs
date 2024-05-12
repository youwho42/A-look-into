using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class SmellsObject : MonoBehaviour
{
    public SpriteRenderer smellSprite;
    public Animator animator;
    WindManager wind;
    public float speed;
    SmellGenerator smellGenerator;
    Material smellMaterial;

    private void Start()
    {
        smellSprite.flipY = Random.Range(0, 2) == 1 ? true : false;
        smellMaterial = smellSprite.material;
        wind = WindManager.instance;
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        float r = speed * .1f;
        speed += Random.Range(-r, r);
    }

    public void SetSmell(SmellGenerator generator, DrawZasYDisplacement displacement, Color color, Color emissionColor)
    {
        smellGenerator = generator;
        smellSprite.transform.localPosition = displacement.displacedPosition;
        float ra = Random.Range(0.5f, 1.0f);
        smellMaterial = smellSprite.material;
        smellMaterial.SetColor("_EmissionColor", emissionColor);
        color.a = ra;
        smellSprite.color = color;
        
        StartCoroutine(SmellSizeCo());
        Invoke("Fade", 5.0f);
    }
    
    void Fade()
    {
        StartCoroutine(FadeSmellCo());
    }

    void Update()
    {
        var windDirection = wind.GetWindDirectionFromPosition(transform.position);
        float windSpeed = wind.GetWindMagnitude(transform.position);
        float angle = Mathf.Atan2(windDirection.y, windDirection.x) * Mathf.Rad2Deg;
        smellSprite.transform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 currentPosition = transform.position;
        currentPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + windDirection.normalized, speed * windSpeed * Time.deltaTime);
        currentPosition.z = 1;
        transform.position = currentPosition;
    }
    IEnumerator SmellSizeCo()
    {
        float timer = 0;
        float time = 7;
        float max = Random.Range(1.5f, 3.3f);
        while (timer < time)
        {
            timer += Time.deltaTime;
            float size = Mathf.Lerp(0.3f, max, timer / time);
            smellSprite.transform.localScale = new Vector3(size, size, size);
            yield return null;
        }

    }

    IEnumerator FadeSmellCo()
    {
        float timer = 0;
        float time = 5;
        Color c = smellSprite.color;
        float startAlpha = c.a;
        while (timer < time)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(startAlpha, 0.0f, timer / time);
            smellSprite.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        smellGenerator.StopEmit(this);
        yield return null;
    }
}
