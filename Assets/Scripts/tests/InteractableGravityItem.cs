using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableGravityItem : MonoBehaviour
{

    [Range(0, 2)]
    public float itemDrag;
    [Range(0, 1)]
    public float itemBounceFriction;
    DrawZasYDisplacement drawZasY;
    GravityItemMovement itemMovement;
    
    float velocity;
    Vector2 mainDirection;
    public bool canInteractWithOtherInteractables;

    private void Start()
    {
        itemMovement = GetComponent<GravityItemMovement>();
        drawZasY = GetComponent<DrawZasYDisplacement>();
    }

    private void Update()
    {
        if (velocity > 0)
        {
            
            Vector3Int diff = itemMovement.nextTilePosition - itemMovement.currentGridLocation.lastTilePosition;
            foreach (var tile in itemMovement.surroundingTiles.allCurrentDirections)
            {
                if (tile.Key == diff && !tile.Value.isValid)
                {

                    // This is where we bounce off of walls, or maybe fall off the cliff, depending on tile.Value.difference  0 = cliff fall, 1 = cliff up

                    if (tile.Value.levelZ <= 0)
                    {
                        //velocity += 0.1f;
                        break;
                    }
                           
                    mainDirection = new Vector2(mainDirection.y, mainDirection.x);
                    if (tile.Key.x > 0)
                        mainDirection *= -1;

                    velocity *= itemBounceFriction;

                    break;

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
            if (gravityItem.displacedPosition.y >= drawZasY.displacedPosition.y)
                return;
            Vector2 direction = transform.position - collision.transform.position;
            AddMovement(direction, gravityItem.currentVelocity != 0 ? gravityItem.currentVelocity * itemBounceFriction : velocity * itemBounceFriction);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            var direction = transform.position - collision.transform.position;
            AddMovement(direction, velocity * itemBounceFriction);
        }
    }

}
