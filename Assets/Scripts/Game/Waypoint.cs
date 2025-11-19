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

        grid.UpdateCostsAround(position, 3, 8);

        SetToUnoccupied();
        SetDefaultVisibility();
    }

    public void SetToOccupied()
    {
        isOccupied = true;
    }

    public void SetToUnoccupied()
    {
        isOccupied = false;
    }

    public void SetDefaultVisibility()
    {
        grid.UpdateCostsAround(position, 2, 7);
    }

    public void IncreaseVisibility()
    {
        grid.UpdateCostsAround(position, 2, 5);
    }
}
