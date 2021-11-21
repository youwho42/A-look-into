using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QPathFinder;

public class PathfindingFollowGameObject : MonoBehaviour
{
    CharacterWalk walk;
    public GameObject objectToFollow;
    public List<Vector3> nodesToDestination = new List<Vector3>();
    public Vector3 currentDestination;
    public int currentDestinationIndex = 0;
    public Animator animator;
    bool following;

    private void Start()
    {
        walk = GetComponent<CharacterWalk>();


    }



    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && !following)
        {
            
            SetDestination(objectToFollow.transform.position);
            following = true;
            
        }

        if (nodesToDestination.Count > 0)
        {
            float stoppingDistance = currentDestinationIndex == nodesToDestination.Count - 1 ? 0.1f : 0.01f;
            float dist = Vector2.Distance(transform.position, currentDestination);
            if (dist >= stoppingDistance)
            {
                walk.Move();
                animator.SetBool("isWalking", true);
            }
            else
            {
                currentDestinationIndex++;
                if (currentDestinationIndex < nodesToDestination.Count)
                    SetCurrentDestination(nodesToDestination[currentDestinationIndex]);
                else
                    animator.SetBool("isWalking", false);
            }
        }
    }

    public void SetDestination(Vector3 position)
    {

        GetPathToDestination(position);

    }

    void GetPathToDestination(Vector3 position)
    {

        PathFinder.instance.FindShortestPathOfPoints(transform.position, position, PathFinder.instance.graphData.lineType,
            Execution.Asynchronously,
            SearchMode.Simple,
            delegate (List<Vector3> points)
            {

                SetMainDestination(points);
            }
            );

    }

    public void SetCurrentDestination(Vector3 position)
    {
        
        currentDestination = position;
        walk.SetDestination(position, Vector3.zero);
    }

    void SetMainDestination(List<Vector3> nodes)
    {
        nodesToDestination.Clear();
        currentDestinationIndex = 0;
        nodesToDestination = nodes;
        SetCurrentDestination(nodesToDestination[currentDestinationIndex]);
    }
}
