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

    // hopping settings
    [SerializeField] float hopHeight = 0.2f;
    [SerializeField] float hopSpeed = 6f;
    private float baseY; // starting Y position
    private bool isHopping = false;

    private void Start()
    {
        GameObject gridObj = GameObject.FindGameObjectWithTag("Grid");
        grid = gridObj.GetComponent<WorldGrid>();
        pathfinder = gameObject.GetComponent<PathfindDijkstra>();

        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");

        targetWaypoint = GetRandomWaypoint();
        target.transform.position = targetWaypoint.position;

        baseY = seeker.position.y; // record starting Y
    }

    private void Update()
    {
        if (pathfinder.path != null && pathfinder.path.Count > 0)
        {
            path = pathfinder.path;
            Node currentNode = grid.NodeFromWorldPoint(seeker.position);
            Node targetNode = grid.NodeFromWorldPoint(target.position);

            float distanceToTarget = Vector3.Distance(seeker.position, target.position);

            
            if (distanceToTarget > 1.0f) 
            {
                isHopping = true;
            }
            else 
            {
                isHopping = false;
                seeker.position = new Vector3(seeker.position.x, baseY, seeker.position.z); // reset Y
            }

            if (currentNode.worldPosition != targetNode.worldPosition)
            {
                Node pathNode = path[0];    // first node in the list
                Vector3 nodePosition = pathNode.worldPosition;
                float move = movementSpeed * Time.deltaTime;

                // Calculate direction to the next node
                Vector3 direction = (nodePosition - seeker.position).normalized;

                // Rotate towards the direction of movement
                if (direction != Vector3.zero) // avoid zero direction
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    seeker.rotation = Quaternion.Slerp(seeker.rotation, targetRotation, 10f * Time.deltaTime);
                }

                // Move towards the next node
                seeker.position = Vector3.MoveTowards(seeker.position, nodePosition, move);
            }
        }
        else if (!isCoroutineRunning)
        {
            StartCoroutine(NewTarget(1f));
        }

        // Apply hopping if active
        if (isHopping)
        {
            float hopOffset = Mathf.Sin(Time.time * hopSpeed) * hopHeight;
            seeker.position = new Vector3(seeker.position.x, baseY + hopOffset, seeker.position.z);
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
