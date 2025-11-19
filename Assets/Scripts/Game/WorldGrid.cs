using UnityEngine;
using System.Collections.Generic;

public class WorldGrid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    int defaultCost = 10;
    int idealCost = 7;

    private void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Color c;

                if (!n.walkable)
                {
                    c = Color.red;
                }
                else if (n.addedWeight < idealCost)
                {
                    c = Color.green;
                }
                else if (n.addedWeight < defaultCost)
                {
                    c = Color.yellow;
                }
                else
                {
                    c = Color.white;
                }

                Gizmos.color = c;
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));
            }
        }
    }

    private void CreateGrid()
    {
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;
        grid = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

                grid[x, y] = new Node(walkable, worldPoint, x, y, defaultCost);
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x;
        float percentY = (worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    public List<Node> GetNodesInRadius(Node centerNode, float radius)
    {
        List<Node> nodes = new List<Node>();
        int rad = Mathf.CeilToInt(radius);

        for (int x = -rad; x <= rad; x++)
        {
            for (int y = -rad; y <= rad; y++)
            {
                int checkX = centerNode.gridX + x;
                int checkY = centerNode.gridY + y;

                // check bounds
                if (checkX >= 0 && checkX < gridSizeX &&
                    checkY >= 0 && checkY < gridSizeY)
                {
                    Node node = grid[checkX, checkY];

                    // ensure circular radius, not square
                    float dist = Vector2.Distance(
                        new Vector2(centerNode.gridX, centerNode.gridY),
                        new Vector2(checkX, checkY)
                    );

                    if (dist <= radius)
                    {
                        nodes.Add(node);
                    }
                }
            }
        }

        return nodes;
    }

    public void UpdateCostsAround(Vector3 worldPos, float radius, int newValue)
    {
        Node center = NodeFromWorldPoint(worldPos);

        List<Node> nodes = GetNodesInRadius(center, radius);

        foreach (Node n in nodes)
        {
            n.addedWeight = newValue;     // Set cost directly
        }
    }

    public Node GetWeightedRandomWalkableNode()
    {
        List<Node> candidates = new List<Node>();
        int totalWeight = 0;

        foreach (Node n in grid)
        {
            if (!n.walkable) continue;

            int weight = Mathf.Max(1, n.addedWeight);
            totalWeight += weight;
            candidates.Add(n);
        }

        int r = Random.Range(0, totalWeight);

        foreach (Node n in candidates)
        {
            int weight = Mathf.Max(1, n.addedWeight);
            if (r < weight)
                return n;
            r -= weight;
        }

        return null;
    }

    public Node GetRandomNearbyNode(Node centerNode, int radius)
    {
        List<Node> candidates = new List<Node>();

        // Search within radius
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                int checkX = centerNode.gridX + x;
                int checkY = centerNode.gridY + y;

                // Skip if out of bounds
                if (checkX < 0 || checkX >= gridSizeX || checkY < 0 || checkY >= gridSizeY)
                    continue;

                Node node = grid[checkX, checkY];

                // Must be walkable, not the same node, and within circle distance
                if (node.walkable && node != centerNode)
                {
                    // Optional: enforce circular radius instead of square
                    if (x * x + y * y <= radius * radius)
                        candidates.Add(node);
                }
            }
        }

        // If nothing found, fallback to current node
        if (candidates.Count == 0)
            return centerNode;

        // Return a random nearby node
        return candidates[Random.Range(0, candidates.Count)];
    }

}
