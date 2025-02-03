using UnityEngine;

public class SmellsObject : MonoBehaviour
{
    public SpriteRenderer smellSprite;
    public Animator animator;
    WindManager wind;
    public float speed;
    //SmellGenerator smellGenerator;
    Material smellMaterial;
    float timer;
    float minSize = 0.3f;
    float maxSize;
    public GatherableItem gatherableItem;
    private void Start()
    {
        gatherableItem = GetComponent<GatherableItem>();
        smellSprite.flipY = Random.Range(0, 2) == 1 ? true : false;
        smellSprite.transform.localScale = new Vector3(minSize, minSize, minSize);
        smellMaterial = smellSprite.material;
        wind = WindManager.instance;
        float randomIdleStart = Random.Range(0, animator.GetCurrentAnimatorStateInfo(0).length);
        animator.Play(0, 0, randomIdleStart);
        float r = speed * .1f;
        speed += Random.Range(-r, r);
    }

    public void SetSmell(DrawZasYDisplacement displacement, SmellItemData smellItem)
    {
        gatherableItem.dataList.Add(smellItem);
        wind = WindManager.instance;
        var pos = displacement.displacedPosition;
        pos.z = displacement.transform.parent.transform.position.z;
        smellSprite.transform.localPosition = pos;
        smellSprite.transform.localScale = new Vector3(minSize, minSize, minSize);
        float ra = Random.Range(0.5f, 1.0f);
        smellMaterial = smellSprite.material;
        smellMaterial.SetColor("_EmissionColor", smellItem.smellEmissionColor);
        var c = smellItem.smellColor;
        c.a = ra;
        smellSprite.color = c;

        timer = 0;
        maxSize = Random.Range(1.5f, 3.3f);

    }

    void Update()
    {
        UpdateSmell();
    }

    public void UpdateSmell()
    {
        var windDirection = wind.GetWindDirectionFromPosition(transform.position);
        float windSpeed = wind.GetWindMagnitude(transform.position);

        Vector3 currentPosition = transform.position;
        currentPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + windDirection.normalized, speed * windSpeed * Time.deltaTime);
        currentPosition.z = 1;
        transform.position = currentPosition;
        timer += Time.deltaTime;
        SmellSize(7);
        if (timer > 5)
            FadeSmell(5);
    }

    void SmellSize(float time)
    {
        if (timer > time)
            return;

        if (timer < time)
        {
            float size = Mathf.Lerp(0.3f, maxSize, timer / time);
            smellSprite.transform.localScale = new Vector3(size, size, size);
        }

    }

    void FadeSmell(float time)
    {
        if (timer > 10)
        {
            gameObject.SetActive(false);
            return;
        }
        Color c = smellSprite.color;
        float startAlpha = c.a;
        if (timer < 10)
        {
            float a = Mathf.Lerp(startAlpha, 0.0f, timer / time);
            smellSprite.color = new Color(c.r, c.g, c.b, a);
        }


    }
}
