using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableGravityItem : MonoBehaviour
{

    [Range(0, 2)]
    public float itemDrag;
    [Range(0, 1)]
    public float itemBounceFriction;

    GravityItemMovement itemMovement;

    float velocity;
    Vector2 mainDirection;
    public bool canInteractWithOtherInteractables;

    private void Start()
    {
        itemMovement = GetComponent<GravityItemMovement>();

    }

    private void Update()
    {
        if (velocity > 0)
        {

            if (!itemMovement.CanReachNextPosition(mainDirection))
            {

                Vector3Int diff = itemMovement.nextTilePosition - itemMovement.currentGridLocation.lastTilePosition;
                foreach (var tile in itemMovement.surroundingTiles.allCurrentDirections)
                {

                    if (tile.Key == diff && !tile.Value.isValid)
                    {

                        if (tile.Value.difference == 0)
                        {
                            velocity += 0.1f;
                            break;
                        }
                           
                        // This is where we bounce off of walls, or maybe fall off the cliff, depending on tile.Value.difference  0 = cliff fall, 1 = cliff up

                        mainDirection = new Vector2(mainDirection.y, mainDirection.x);
                        if (tile.Key.x != 0)
                            mainDirection *= -1;

                        velocity *= itemBounceFriction;

                        break;

                    }
                }

            }
            itemMovement.Move(mainDirection, velocity);
            velocity -= itemDrag * Time.deltaTime;
        }

        
    }


    void AddMovement(Vector2 newDirection, float _velocity)
    {
        mainDirection = newDirection.normalized;
        velocity = _velocity;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider is CompositeCollider2D)
            return;
        if (collision.gameObject.TryGetComponent(out InteractableGravityItem interacltableItem) && !canInteractWithOtherInteractables)
            return;

        if (collision.gameObject.TryGetComponent(out GravityItemMovement gravityItem))
        {
            Vector2 direction = transform.position - collision.transform.position;
            AddMovement(direction, gravityItem.currentVelocity != 0 ? gravityItem.currentVelocity * 1.1f:velocity * itemBounceFriction);
        }
        else if (collision.gameObject.CompareTag("Robot") && collision.collider is PolygonCollider2D)
        {
            Vector2 direction = transform.position - collision.transform.position;
            AddMovement(direction, 0.5f);
        }


    }

}
