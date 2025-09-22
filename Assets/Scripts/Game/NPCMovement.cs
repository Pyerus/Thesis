using UnityEngine;
using System.Collections.Generic;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 10f;

    [SerializeField] WorldGrid grid;

    [SerializeField] PathfindDijkstra pathfinder;

    [SerializeField] Transform seeker;

    [SerializeField] Transform target;

    private List<Node> path;



    private void Update()
    {
        if (pathfinder.path != null && pathfinder.path.Count > 0)
        {
            path = pathfinder.path;
            Node currentNode = grid.NodeFromWorldPoint(seeker.position);
            Node targetNode = grid.NodeFromWorldPoint(target.position);

            if (currentNode != targetNode)
            {
                Node pathNode = path[0];    //first node in the list

                Vector3 nodePosition = pathNode.worldPosition;
                float move = movementSpeed * Time.deltaTime;

                seeker.transform.position = Vector3.MoveTowards(seeker.position, nodePosition, move);
            }
            
        }

    }
}
