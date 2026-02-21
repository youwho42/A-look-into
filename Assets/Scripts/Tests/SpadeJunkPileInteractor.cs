using Klaxon.UndertakingSystem;
using QuantumTek.QuantumInventory;
using UnityEngine;

public class SpadeJunkPileInteractor : SpadeInteractable
{
    
    public ParticleSystem poofSystem;
    public GameObject grassHider;
    public GameObject obstacleCollider;
    public GameObject junk;
    
    public CompleteTaskObject undertaking;

    
    public override void EndSpadeInteraction()
    {
        base.EndSpadeInteraction();
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
