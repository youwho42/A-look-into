using UnityEngine;
using UnityEngine.Localization.Settings;

[CreateAssetMenu(menuName = "Quantum Tek/Quantum Inventory/Equipment Item/SpadeItem", fileName = "New Spade Item")]
public class EquipmentSpade : EquipmentData
{

    public MiniGameType miniGameType;
    
    public LayerMask gathererLayer;
    public float detectionRadius;

    public override void UseEquippedItem()
    {
        base.UseEquippedItem();

        var player = PlayerInformation.instance;
        if (LevelManager.instance.inPauseMenu || player.playerInput.isInUI)
            return;

        //set the junkpiles to their own layer, check against that

        Collider2D[] hit = Physics2D.OverlapCircleAll(player.player.position, detectionRadius, gathererLayer, player.player.position.z, player.player.position.z);
        if (hit.Length > 0)
            GetNearestItem(hit, player.player.position);
        else
            player.playerAnimator.SetBool("UseEquipement", false);
    }

    public void GetNearestItem(Collider2D[] colliders, Vector3 position)
    {
        // Find nearest item.

        Collider2D nearest = null;
        float distance = 0;
        PlayerInformation playerInfo = PlayerInformation.instance;
        for (int i = 0; i < colliders.Length; i++)
        {
            float tempDistance = Vector2.Distance(position, colliders[i].transform.position);

            if (nearest == null || tempDistance < distance)
            {
                nearest = colliders[i];
                distance = tempDistance;
            }
        }


        // Found an object.
        if (nearest != null)
        {

            // activate the minigame
            bool none = true;
            if (nearest.gameObject.TryGetComponent(out JunkPileInteractor junkPile))
            { 
                if(junkPile.junkPileTier == equipmentTier)
                {
                    none = false;
                    if (InteractCostReward())
                    {
                        MiniGameManager.instance.StartMiniGame(miniGameType, junkPile);
                    }
                }
            }
            if (none)
            {
                playerInfo.playerAnimator.SetBool("UseEquipement", false);
                Notifications.instance.SetNewNotification(LocalizationSettings.StringDatabase.GetLocalizedString($"Variable-Texts", "Wrong equipment"), null, 0, NotificationsType.Warning);
            }


        }
    }
}
