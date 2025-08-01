using QuantumTek.QuantumInventory;
using System.Collections;
using UnityEngine;

public class GroundBubble : MonoBehaviour
{
    public Transform groundBubble_X;
    public Transform groundBubbleObject;
    public Transform groundBubbleShadow;
    public Transform floatingBubble;
    public Transform bubbleSprite;
    SpriteRenderer xSprite;
    bool isGrounded;

    public QI_ItemData dynamiblobItem;
    ParticlesToPlayer particles;
    DrawZasYDisplacement dynamiblobSpawnDisplacement;
    bool hasBeenActivated;


    private void Start()
    {
        xSprite = groundBubble_X.GetComponent<SpriteRenderer>();
        hasBeenActivated = false;
        particles = GetComponent<ParticlesToPlayer>();
        dynamiblobSpawnDisplacement = GetComponent<DrawZasYDisplacement>();

        floatingBubble.gameObject.SetActive(false);
        groundBubbleObject.gameObject.SetActive(false);
        groundBubbleShadow.gameObject.SetActive(false);
        groundBubble_X.gameObject.SetActive(false);
    }

    public void SetBubble(float remainTime)
    {
        
        floatingBubble.gameObject.SetActive(false);
        groundBubbleObject.gameObject.SetActive(true);
        groundBubbleShadow.gameObject.SetActive(true);
        groundBubble_X.gameObject.SetActive(true);
        groundBubble_X.localScale = Vector3.zero;
        groundBubbleObject.localScale = Vector3.zero;
        StartCoroutine(GrowBubbleCo());
        Invoke("EndBubble", remainTime);
    }

    IEnumerator GrowBubbleCo()
    {
        float timer = 0;
        float growtime = 2.5f;

        while (timer < growtime)
        {
            timer += Time.deltaTime;
            float size = Mathf.Lerp(0, 1, timer / growtime);
            groundBubble_X.localScale = new Vector3(size, size, size);
            groundBubbleObject.localScale = new Vector3(size, size, size);
            groundBubbleShadow.localScale = new Vector3(size, size, size);
            yield return null;
        }

        isGrounded = true;
        yield return null;
    }

    void EndBubble()
    {
        groundBubbleObject.gameObject.SetActive(false);
        groundBubbleShadow.gameObject.SetActive(false);
        isGrounded = false;
        StartCoroutine(BubbleDriftCo());
        StartCoroutine(FadeX());
    }
    IEnumerator FadeX()
    {
        float timer = 0;
        float fadeTime = 1.5f;

        while (timer < fadeTime)
        {
            timer += Time.deltaTime;
            float a = Mathf.Lerp(1, 0, timer / fadeTime);
            xSprite.color = new Color(1, 1, 1, a);
            yield return null;
        }
        groundBubble_X.gameObject.SetActive(false);
        xSprite.color = Color.white;
        yield return null;
    }
    IEnumerator BubbleDriftCo()
    {
        floatingBubble.localPosition = Vector3.zero;
        floatingBubble.gameObject.SetActive(true);
        float size = Random.Range(0.5f, 0.8f);
        floatingBubble.localScale = new Vector3(size, size, size);
        float timer = 0;

        float driftTime = Random.Range(5.0f, 12.0f);
        float z = 0;
        while (timer < driftTime)
        {
            timer += Time.deltaTime;
            //Move Bubble Object
            var dir =  WindManager.instance.GetWindDirectionFromPosition(floatingBubble.position);
            var mag =  WindManager.instance.GetWindMagnitude(floatingBubble.position);
            floatingBubble.Translate(dir * 0.06f * mag * Time.deltaTime);

            //Move Bubble Sprite
            float disp = GlobalSettings.SpriteDisplacementY;
            z += 0.008f;
            Vector3 pos = new Vector3(0, disp * z, z);
            bubbleSprite.localPosition = pos;

            yield return null;
        }
        floatingBubble.gameObject.SetActive(false);
        bubbleSprite.localPosition = Vector3.zero;
        yield return null;
    }

    IEnumerator SpawnDynamiblobCo()
    {
        hasBeenActivated = true;
        particles.SpawnParticles(1, dynamiblobSpawnDisplacement);
        groundBubbleObject.gameObject.SetActive(false);
        groundBubbleShadow.gameObject.SetActive(false);
        StartCoroutine(FadeX());
        CancelInvoke();
        yield return new WaitForSeconds(1.0f);
        PlayerInformation.instance.playerInventory.AddItem(dynamiblobItem, 1, false);
        Notifications.instance.SetNewNotification("", dynamiblobItem, 1, NotificationsType.Inventory);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.CompareTag("Player") || !isGrounded || hasBeenActivated) 
            return;
        AudioManager.instance.PlaySound("BubblePop");
        StartCoroutine(SpawnDynamiblobCo());
    }
}
