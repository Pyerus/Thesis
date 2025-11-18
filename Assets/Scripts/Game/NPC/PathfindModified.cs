using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindModified : MonoBehaviour
{
    [Header("NPC")]
    public Transform seeker;
    public Transform target;
    public float moveSpeed = 10f;
    public float searchRadius = 3f;

    [Header("Probabilities")]
    public int newRandomTarget;
    public int newNeighbor;
    public int continuePath;

    [SerializeField] Decision decision;

    private WorldGrid grid;
    private List<Node> path;
    private NPCBehaviour npcBehaviour;
    private ShoppingList shoppingList;
    private ShelvesManager shelvesManager;
    private ImpulsiveBuying impulsiveBuying;
    private List<ProductEntry> itemList;
    private int listIndex = 0;

    private GameObject currentShelf;
    private GameObject waypoint;
    private ProductEntry currentItem;
    private Vector3? nextExtraWaypoint = null;

    private bool goingToCheckout = false;
    private bool checkedOut = false;

    void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<WorldGrid>();
        shelvesManager = GameObject.FindFirstObjectByType<ShelvesManager>();
        npcBehaviour = GetComponent<NPCBehaviour>();
        shoppingList = GetComponent<ShoppingList>();
        impulsiveBuying = new ImpulsiveBuying();
        itemList = shoppingList.generatedList;

        StartCoroutine(BehaviourLoop());
    }

    /////////////////////////////////////////////////////////////////////
    // MAIN LOOP --------------------------------------------------------
    /////////////////////////////////////////////////////////////////////

    IEnumerator BehaviourLoop()
    {
        while (true)
        {
            // 1. Check if we have an extra waypoint from a decision
            if (nextExtraWaypoint.HasValue)
            {
                target.position = nextExtraWaypoint.Value;
                nextExtraWaypoint = null;
            }
            else
            {
                // Normal waypoint logic
                GameObject randomShelf = shelvesManager.GetRandomShelfGOInRadius(seeker.position, searchRadius);

                if (HandleImpulseBuying(randomShelf) && !goingToCheckout)
                {
                    waypoint = randomShelf.transform.Find("Waypoint").gameObject;
                }
                else
                {
                    waypoint = GetNextWaypoint();
                }

                if (waypoint == null)
                {
                    Debug.Log("NPC has no waypoint. Ending.");
                    yield break;
                }

                target.position = waypoint.transform.position;
            }

            // 2. Compute path
            FindPath(seeker.position, target.position);

            // 3. Move along path
            yield return StartCoroutine(MoveAlongPath(moveSpeed));

            // 4. Handle arrival
            yield return StartCoroutine(HandleArrival(0.5f));
        }
    }

    /////////////////////////////////////////////////////////////////////
    // WAYPOINT SELECTION -----------------------------------------------
    /////////////////////////////////////////////////////////////////////

    GameObject GetNextWaypoint()
    {
        // Store is closed OR list finished → go checkout/exit
        if (listIndex >= itemList.Count || NPC.StoreClosed)
            return GetCheckoutOrExitWaypoint();

        // Otherwise go to next shelf
        currentItem = itemList[listIndex];
        ItemObject item = currentItem.product;
        currentShelf = shelvesManager.SearchShelvesWithProduct(item);
        listIndex++;

        if (currentShelf == null)
        {
            Debug.LogWarning("No shelf found for product: " + item.name);
            return GetCheckoutOrExitWaypoint();
        }

        return currentShelf.transform.Find("Waypoint").gameObject;
    }

    GameObject GetCheckoutOrExitWaypoint()
    {
        // Go to checkout only once if cart has items
        if (!goingToCheckout && shoppingList.cart.Count > 0)
        {
            goingToCheckout = true;
            GameObject checkout = GameObject.FindGameObjectWithTag("Checkout");
            if (checkout != null) return checkout;

            Debug.LogWarning("Checkout not found.");
        }

        // Otherwise go exit
        GameObject exit = GameObject.FindGameObjectWithTag("Exit");
        return exit;
    }

    /////////////////////////////////////////////////////////////////////
    // MODIFIED PATHFINDING ----------------------------------------------
    /////////////////////////////////////////////////////////////////////

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

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
            // If current node is the destination
            // ----------------------------------------------------
            if (currentNode == targetNode)
            {
                RetracePath(startNode, targetNode);
                MakeDecision();

                switch (decision)
                {
                    case Decision.NewDestination:
                        Node newTarget = grid.GetWeightedRandomWalkableNode();
                        nextExtraWaypoint = newTarget.worldPosition;
                        break;
                    case Decision.CheckDifferentNeighbor:
                        Node neighborTarget = grid.GetRandomNearbyNode(currentNode, 5);
                        nextExtraWaypoint = neighborTarget.worldPosition;
                        break;
                    case Decision.Continue:
                        nextExtraWaypoint = null;
                        break;
                }

                return;
            }

            // ----------------------------------------------------
            // Standard Dijkstra neighbor scanning
            // ----------------------------------------------------
            foreach (Node neighbour in grid.GetNeighbors(currentNode))
            {
                if (!neighbour.walkable || closedSet.Contains(neighbour)) continue;

                int newCost = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.addedWeight;

                if (newCost < neighbour.gCost || !openSet.Contains(neighbour))
                {
                    neighbour.gCost = newCost;
                    neighbour.parent = currentNode;
                    if (!openSet.Contains(neighbour)) openSet.Add(neighbour);
                }
            }
        }
    }

    void RetracePath(Node start, Node end)
    {
        List<Node> r = new List<Node>();
        Node c = end;

        while (c != start)
        {
            r.Add(c);
            c = c.parent;
        }

        r.Reverse();
        path = r;
    }

    int GetDistance(Node a, Node b)
    {
        int dx = Mathf.Abs(a.gridX - b.gridX);
        int dy = Mathf.Abs(a.gridY - b.gridY);

        return (dx > dy) ? 14 * dy + 10 * (dx - dy) : 14 * dx + 10 * (dy - dx);
    }

    /////////////////////////////////////////////////////////////////////
    // DECISION MAKING ----------------------------------------------------
    /////////////////////////////////////////////////////////////////////

    void MakeDecision()
    {
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
    }

    enum Decision
    {
        NewDestination,
        CheckDifferentNeighbor,
        Continue
    }

    /////////////////////////////////////////////////////////////////////
    // MOVEMENT -----------------------------------------------------------
    /////////////////////////////////////////////////////////////////////

    IEnumerator MoveAlongPath(float speed)
    {
        if (path == null || path.Count == 0) yield break;

        foreach (Node node in path)
        {
            Vector3 targetPos = node.worldPosition;

            while (Vector3.Distance(seeker.position, targetPos) > 0.05f)
            {
                seeker.position = Vector3.MoveTowards(seeker.position, targetPos, speed * Time.deltaTime);
                yield return null;
            }
        }
    }

    /////////////////////////////////////////////////////////////////////
    // ARRIVAL BEHAVIOR ---------------------------------------------------
    /////////////////////////////////////////////////////////////////////

    IEnumerator HandleArrival(float interval)
    {
        yield return new WaitForSeconds(interval);

        if (waypoint.CompareTag("Waypoint"))
        {
            ShelfInventory shelfInv = currentShelf.GetComponent<ShelfInventory>();
            ProductEntry entry = currentItem;
            npcBehaviour.AddToCart(shelfInv, entry.product, entry.quantity);
        }
        else if (waypoint.CompareTag("Checkout"))
        {
            npcBehaviour.CheckOut();
        }
    }

    /////////////////////////////////////////////////////////////////////
    // IMPULSE BUYING -----------------------------------------------------
    /////////////////////////////////////////////////////////////////////

    bool HandleImpulseBuying(GameObject shelf)
    {
        if (shelf == null) return false;

        ShelfInventory shelfInventory = shelf.GetComponent<ShelfInventory>();
        ItemValue itemValue = shelfInventory.GetComponent<ItemValue>();
        if (itemValue == null) return false;

        ItemObject ideal = itemValue.GetBestIdealItem();
        if (ideal == null) return false;

        impulsiveBuying.AddImpulseFactor(ideal, 0.5f);

        bool bought = impulsiveBuying.TryImpulseBuy(ideal);

        if (bought)
        {
            currentShelf = shelf;
            currentItem = new ProductEntry(ideal, Random.Range(0, 5));
            Debug.Log($"NPC wants {ideal?.name ?? "something"}!");
        }

        return bought;
    }
}
