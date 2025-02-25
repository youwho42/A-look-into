using UnityEngine;

public class FlumpOozeDrop : MonoBehaviour
{
    Vector2 direction;
    float speed = 0.3f;
    float height;
    public Transform dropObject;
    public SpriteRenderer dropSprite;
    float positionZ;
    float gravity = 20;
    Vector3 displacedPosition;
    float currentZ;
    bool launched;
    public Color colorA;
    public Color colorB;
    Color currentColor;
    public FlumpOoze flumpOoze;
    Transform currentParent;

    public SoundSet spawnSoundSet;
    AudioSource source;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        launched = false;
        direction = Random.insideUnitCircle.normalized;
        height = Random.Range(3, 8);
        speed = Random.Range(0.05f, 0.4f);
    }

    public void SetOozeDrop(Transform parent, Vector3 position, DrawZasYDisplacement displacement)
    {
        transform.position = position;
        currentZ = transform.position.z;
        currentParent = parent;
        dropObject.localPosition = displacement.displacedPosition;
        positionZ = displacement.positionZ;
        currentColor = Color.Lerp(colorA, colorB, Random.value);
        dropSprite.color = currentColor;
    }

    private void Update()
    {
        if(!launched)
        {
            launched = true;
            Launch(height);
        }
        Move(direction, speed);
        ApplyGravity();
    }

    public void Launch(float launchAmount)
    {
        positionZ = launchAmount;
        displacedPosition = new Vector3(0, GlobalSettings.SpriteDisplacementY * positionZ, positionZ);
        ApplyGravity();
        if (Random.value < 0.5f)
            PlaySound();
    }

    protected void ApplyGravity()
    {
        positionZ -= gravity * Time.fixedDeltaTime;

        displacedPosition = new Vector3(0, GlobalSettings.SpriteDisplacementY * positionZ, positionZ);
        
        dropObject.Translate(displacedPosition * Time.fixedDeltaTime);

        if (dropObject.localPosition.y <= 0)
        {
            positionZ = 0;
            dropObject.localPosition = Vector3.zero;
            gameObject.SetActive(false);
            if(Random.value < 0.5f)
            {
                
                var go = Instantiate(flumpOoze, transform.position, Quaternion.identity, currentParent);
                go.SetOoze(currentParent, currentColor, false);
            }
        }
    }

    public void Move(Vector2 dir, float velocity)
    {
        Vector3 currentPosition = transform.position;
        currentPosition = Vector2.MoveTowards(transform.position, (Vector2)transform.position + dir, Time.deltaTime * velocity);
        currentPosition.z = currentZ;
        transform.position = currentPosition;
    }

    void PlaySound()
    {
        int t = Random.Range(0, spawnSoundSet.clips.Length);
        spawnSoundSet.SetSource(source, t);
        spawnSoundSet.Play();
    }
}
