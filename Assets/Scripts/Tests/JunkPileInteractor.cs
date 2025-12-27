using Klaxon.UndertakingSystem;
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
    public CompleteTaskObject undertaking;

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
        if (replaceObjectOnDrop == null)
            return;
        replaceObjectOnDrop.ShowObjects(true);
        replaceObjectOnDrop.CheckForObjects();
    }
    public void StartParticles()
    {
        if (poofSystem != null)
            poofSystem.Play();
        DisableGameObject();
    }

    public void DisableGameObject()
    {
        if (grassHider != null)
            grassHider.SetActive(false);
        if (obstacleCollider != null)
            obstacleCollider.SetActive(false);
        if (junk != null)
            junk.SetActive(false);
        if (interactCollider != null)
            interactCollider.enabled = false;
        hasInteracted = true;
        SetPathfindingTiles();
        if (undertaking.undertaking != null)
            undertaking.undertaking.TryCompleteTask(undertaking.task);
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
