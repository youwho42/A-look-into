using QuantumTek.QuantumInventory;
using UnityEngine;

public class JunkPileInteractor : MonoBehaviour
{
    public EquipmentTier junkPileTier;
    public QI_ItemDatabase junkPileDatabase;
    public MiniGameDificulty gameDificulty;
    public ParticleSystem poofSystem;
    public GameObject grassHider;
    public GameObject obstacleCollider;
    public GameObject junk;
    public Collider2D interactCollider;
    [HideInInspector]
    public bool hasInteracted;
    public ReplaceObjectOnItemDrop replaceObjectOnDrop;
    private void Start()
    {
        GameEventManager.onGameLoadedEvent.AddListener(CheckForObjectsToHide);
        GameEventManager.onGameStartLoadEvent.AddListener(CheckForObjectsToHide);
    }
    private void OnDisable()
    {
        GameEventManager.onGameLoadedEvent.RemoveListener(CheckForObjectsToHide);
        GameEventManager.onGameStartLoadEvent.RemoveListener(CheckForObjectsToHide);
    }
    void CheckForObjectsToHide() 
    { 
        replaceObjectOnDrop.CheckForObjects();
    }
    public void StartParticles()
    {
        poofSystem.Play();
        DisableGameObject();
    }

    public void DisableGameObject()
    {
        grassHider.SetActive(false);
        obstacleCollider.SetActive(false);
        junk.SetActive(false);
        interactCollider.enabled = false;
        hasInteracted = true;
        SetPathfindingTiles();

    }

    private void SetPathfindingTiles()
    {
        var tile = GridManager.instance.GetTilePosition(transform.position);
        var worldPos = GridManager.instance.GetTileWorldPosition(tile) + Vector3Int.forward;

        Collider2D obstacleCheck = Physics2D.OverlapCircle(worldPos, 0.1f, LayerMask.GetMask("Obstacle"), worldPos.z, worldPos.z);
        if (obstacleCheck == null)
            PathRequestManager.instance.pathfinding.isometricGrid.nodeLookup[tile].walkable = true;
        
    }
}
