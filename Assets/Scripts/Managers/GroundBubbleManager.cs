using UnityEngine;

public class GroundBubbleManager : MonoBehaviour
{
    public GroundBubble groundBubble;

    float bubbleTime;
    float bubbleAppearTime;
    public Vector2 minMaxBetweenBubbles;
    public Vector2 minMaxBubbleAppearance;

    private void Start()
    {
        groundBubble.gameObject.SetActive(true);
        ResetAppearTime();
    }

    void ResetAppearTime()
    {
        bubbleAppearTime = Random.Range(minMaxBubbleAppearance.x, minMaxBubbleAppearance.y);
        bubbleTime = Random.Range(minMaxBetweenBubbles.x, minMaxBetweenBubbles.y);
        Invoke("AttemptBubble", bubbleTime + bubbleAppearTime + 12.0f);
    }

    public void AttemptBubble()
    {
        groundBubble.transform.position = GetBubblePosition();
        groundBubble.gameObject.SetActive(true);
        groundBubble.SetBubble(bubbleAppearTime);
        ResetAppearTime();
    }

    private Vector3 GetBubblePosition()
    {
        var playerPos = PlayerInformation.instance.player.position;
        var pos = GridManager.instance.GetRandomTileWorldPosition(playerPos, 3);
        var hit = Physics2D.OverlapPoint(pos, LayerMask.GetMask("Obstacle"), pos.z, pos.z);
        if (!hit)
            return pos;
        for (int d = 1; d < 3; d++)
        {
            for (float i = 0; i < Mathf.PI * 2; i += Mathf.PI * 0.25f)
            {
                Vector2 dir = new Vector2(Mathf.Cos(i), Mathf.Sin(i));
                dir = dir.normalized;
                dir *= d * 0.1f;
                var posI = pos + (Vector3)dir;
                var h = Physics2D.OverlapPoint(posI, LayerMask.GetMask("Obstacle"), posI.z, posI.z);
                if (!h && GridManager.instance.GetTileValid(posI))
                    return posI;
            }
        }
        return GetBubblePosition();
    }
}
