using UnityEngine;

public class Waypoint : MonoBehaviour
{
    private WorldGrid grid;
    private Vector3 position;
    
    public bool isOccupied;

    private void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<WorldGrid>();
        position = transform.position;

        SetToUnoccupied();
    }

    public void SetToOccupied()
    {
        isOccupied = true;
    }

    public void SetToUnoccupied()
    {
        isOccupied = false;

        grid.UpdateCostsAround(position, 2, 5);
    }
}
