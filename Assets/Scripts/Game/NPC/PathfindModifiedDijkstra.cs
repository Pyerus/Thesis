using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PathfindModifiedDijkstra : MonoBehaviour
{
    [Header("Pathfinder")]
    public Transform seeker, target;
    public List<Node> path;
    private WorldGrid grid;

    [Header("Weighted Probability")]
    public int newRandomTarget;
    public int newNeighbor;
    public int continuePath;

    [Header("Decision")]
    [SerializeField] Decision decision;
    bool isCoroutine1Running = false;
    bool isCoroutine2Running = false;

    public bool checkedOut = false;

    public ImpulsiveBuying impulsiveBuying;
    public ShelvesManager shelvesManager;
    public float searchRadius = 3;



    private void Start()
    {
        GameObject gridObj = GameObject.FindGameObjectWithTag("Grid");
        grid = gridObj.GetComponent<WorldGrid>();
        impulsiveBuying = new ImpulsiveBuying();
        shelvesManager = GameObject.FindFirstObjectByType<ShelvesManager>();
    }

    private void Update()
    {
        if (!isCoroutine1Running)
            StartCoroutine(RunAlgorithm(0.1f));
    }

    // -----------------------------
    // MAIN MODIFIED DIJKSTRA
    // -----------------------------
    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node currentNode = openSet[0];

            // Get the unvisited node with smallest distance 
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].gCost < currentNode.gCost)
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // ----------------------------------------------------
            //  If current node is the destination
            // ----------------------------------------------------
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);

                //if (!isCoroutine2Running)
                //{
                //    StartCoroutine(MakeDecision(0.7f));
                //}

                //if (decision == Decision.NewDestination)
                //{
                //    // pick a new random destination
                //    targetNode = grid.GetWeightedRandomWalkableNode();

                //    // Detect shelf
                //    ShelfInventory shelf = target.GetComponent<ShelfInventory>();
                //    if (shelf != null)
                //    {
                //        HandleImpulseBuying(shelf);
                //    }

                //    continue;
                //}
                //else if (decision == Decision.CheckDifferentNeighbor)
                //{
                //    // go to a nearby neighbor node
                //    targetNode = grid.GetRandomNearbyNode(currentNode, radius: 5);

                //    // Detect shelf
                //    ShelfInventory shelf = target.GetComponent<ShelfInventory>();
                //    if (shelf != null)
                //    {
                //        HandleImpulseBuying(shelf);
                //    }

                //    continue;
                //}
                //else if (decision == Decision.Continue)
                //{
                //    // finish pathfinding normally
                //    RetracePath(startNode, targetNode);

                //    // Detect random nearby shelf
                //    GameObject shelf = shelvesManager.GetRandomShelfGOInRadius(targetNode.worldPosition, searchRadius);
                //    if (shelf != null)
                //    {
                //        Debug.Log("ImpulseBuying: method called.");
                //        HandleImpulseBuying(shelf.GetComponent<ShelfInventory>());
                //    }

                //    return;
                //}
            }

            // ----------------------------------------------------
            //  Standard Dijkstra neighbor checking
            // ----------------------------------------------------
            foreach (Node neighbour in grid.GetNeighbors(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour))
                    continue;

                int newCost = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.addedWeight;

                if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.parent = currentNode;

                    if (!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();

        this.path = path;
    }

    int GetDistance(Node nodeA, Node nodeB)
    {
        int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
        int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

        if (distX > distY)
        {
            return 14 * distY + 10 * (distX - distY);
        }
        else
        {
            return 14 * distX + 10 * (distY - distX);
        }
    }

    IEnumerator MakeDecision(float interval)
    {
        isCoroutine2Running = true;

        int total = newRandomTarget + newNeighbor + continuePath;
        int n = Random.Range(0, total);

        if (checkedOut)
        {
            decision = Decision.Continue;
        }
        else if (n < newRandomTarget)
        {
            decision = Decision.NewDestination;
        }
        else if (n < newRandomTarget + newNeighbor)
        {
            decision = Decision.CheckDifferentNeighbor;
        }
        else
        {
            decision = Decision.Continue;
        }

        yield return new WaitForSeconds(interval);
        isCoroutine2Running = false;
    }

    enum Decision
    {
        NewDestination,
        CheckDifferentNeighbor,
        Continue
    }

    private void HandleImpulseBuying(GameObject shelf)
    {
        if (shelf == null) return;
        
        ShelfInventory inventory = shelf.GetComponent<ShelfInventory>();
        
        if (inventory == null) return;

        // Now attempt impulse buy from the items in the shelf
        ItemObject itemBought = impulsiveBuying.TryImpulseBuy(shelf);

        if (itemBought != null)
        {
            Debug.Log($"NPC impulse bought {itemBought?.name ?? "something"}!");
        }
        else
            Debug.Log($"NPC chose not to buy impulsively.");
    }

    IEnumerator RunAlgorithm(float interval)
    {
        isCoroutine1Running = true;

        FindPath(seeker.position, target.position);

        yield return new WaitForSeconds(interval);

        isCoroutine1Running = false;
    }
}
