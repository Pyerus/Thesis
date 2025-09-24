using UnityEngine;

public class MoveTarget : MonoBehaviour
{
    [SerializeField] Transform target;
    private GameObject[] waypoints;

    private void Start()
    {
        waypoints = GameObject.FindGameObjectsWithTag("Waypoint");
        
        Transform targetWaypoint = GetRandomWaypoint();
        if (targetWaypoint != null)
        {
            Debug.Log("Picked waypoint: " + targetWaypoint.name);
        }

        target.transform.position = targetWaypoint.position;
    }

    private void Update()
    {
        //target.transform.position = targetWaypoint.position;
    }

    public Transform GetRandomWaypoint()
    {
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints assigned.");
            return null;
        }

        int randomIndex = Random.Range(0, waypoints.Length);
        return waypoints[randomIndex].GetComponent<Transform>();
    }
}
