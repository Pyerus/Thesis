using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField] float movementSpeed = 10f;

    [SerializeField] Transform seeker;

    [SerializeField] Transform target;

    private WorldGrid grid;

    private PathfindDijkstra pathfinder;

    private List<Node> path;

    private GameObject[] waypoints;

    private Transform targetWaypoint;

    bool isCoroutineRunning = false;

    private void Start()
    {
        GameObject gridObj = GameObject.FindGameObjectWithTag("Grid");
        grid = gridObj.GetComponent<WorldGrid>();
        pathfinder = gameObject.GetComponent<PathfindDijkstra>();

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        targetWaypoint = GetRandomWaypoint();
        
        target.transform.position = targetWaypoint.position;
    }

    private void Update()
    {
        if (pathfinder.path != null && pathfinder.path.Count > 0)
        {
            path = pathfinder.path;
            Node currentNode = grid.NodeFromWorldPoint(seeker.position);
            Node targetNode = grid.NodeFromWorldPoint(target.position);

            if (currentNode.worldPosition != targetNode.worldPosition)
            {
                Node pathNode = path[0];    // first node in the list

                Vector3 nodePosition = pathNode.worldPosition;
                float move = movementSpeed * Time.deltaTime;

                seeker.transform.position = Vector3.MoveTowards(seeker.position, nodePosition, move);
            }

        }
        else if (!isCoroutineRunning)
        {
            StartCoroutine(NewTarget(1f));
        }
    }

    public Transform GetRandomWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned.");
            return null;
        }

        int randomIndex = Random.Range(0, waypoints.Length);

        Transform waypoint = waypoints[randomIndex].GetComponent<Transform>();

        Debug.Log("Picked waypoint: " + waypoint.name);

        return waypoint;
    }

    IEnumerator NewTarget(float interval)
    {
        isCoroutineRunning = true;

        yield return new WaitForSeconds(interval);

        targetWaypoint = GetRandomWaypoint();

        target.transform.position = targetWaypoint.position;

        isCoroutineRunning = false;
    }
}
