using UnityEngine;
using System.Collections.Generic;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 10f;

    [SerializeField] Transform seeker;

    [SerializeField] Transform target;

    private WorldGrid grid;

    private PathfindDijkstra pathfinder;

    private List<Node> path;



    private void Start()
    {
        GameObject gridObj = GameObject.FindGameObjectWithTag("Grid");
        grid = gridObj.GetComponent<WorldGrid>();
        pathfinder = gameObject.GetComponent<PathfindDijkstra>();
    }

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
