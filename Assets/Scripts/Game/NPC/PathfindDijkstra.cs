using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindDijkstra : MonoBehaviour
{
    [Header("NPC")]
    public Transform seeker;
    public Transform target;
    public float moveSpeed = 10f;
    public float searchRadius = 3f;

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
    private bool goingToCheckout = false;



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
    // MAIN LOOP  --------------------------------------------------------
    /////////////////////////////////////////////////////////////////////

    IEnumerator BehaviourLoop()
    {
        while (true)
        {
            // 1. Get where to go next
            GameObject randomShelf = shelvesManager.GetRandomShelfGOInRadius(seeker.position, searchRadius);
            if (HandleImpulseBuying(randomShelf) && !goingToCheckout)
            {
                waypoint = randomShelf.transform.Find("Waypoint").gameObject; // get random shelf's waypoint
                Debug.Log($"NPC is buying {currentItem.product?.name ?? "something"}!");
            }
            else
                waypoint = GetNextWaypoint(); // get next waypoint on the list

            if (waypoint == null)
            {
                Debug.Log("NPC has no waypoint. Ending.");
                yield break;
            }

            target.position = waypoint.transform.position;

            // 2. Compute path
            FindPath(seeker.position, target.position);

            // 3. Move along the computed path
            yield return StartCoroutine(MoveAlongPath(moveSpeed));

            // 4. Execute arrival behavior (buy item / checkout)
            yield return StartCoroutine(HandleArrival(0.5f));
        }
    }


    /////////////////////////////////////////////////////////////////////
    // WAYPOINT SELECTION  -----------------------------------------------
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
            if (checkout != null)
                return checkout;

            Debug.LogWarning("Checkout not found.");
        }

        // Otherwise go exit
        GameObject exit = GameObject.FindGameObjectWithTag("Exit");
        return exit;
    }


    /////////////////////////////////////////////////////////////////////
    // PATHFINDING  ------------------------------------------------------
    /////////////////////////////////////////////////////////////////////

    void FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = grid.NodeFromWorldPoint(startPos);
        Node targetNode = grid.NodeFromWorldPoint(targetPos);

        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        while (openSet.Count > 0)
        {
            Node current = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].gCost < current.gCost)
                    current = openSet[i];

            openSet.Remove(current);
            closedSet.Add(current);

            if (current == targetNode)
            {
                RetracePath(startNode, targetNode);
                return;
            }

            foreach (Node n in grid.GetNeighbors(current))
            {
                if (!n.walkable || closedSet.Contains(n))
                    continue;

                int newCost = current.gCost + GetDistance(current, n) + n.addedWeight;

                if (newCost < n.gCost || !openSet.Contains(n))
                {
                    n.gCost = newCost;
                    n.parent = current;

                    if (!openSet.Contains(n))
                        openSet.Add(n);
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
        if (dx > dy) return 14 * dy + 10 * (dx - dy);
        return 14 * dx + 10 * (dy - dx);
    }


    /////////////////////////////////////////////////////////////////////
    // MOVEMENT -----------------------------------------------------------
    /////////////////////////////////////////////////////////////////////

    IEnumerator MoveAlongPath(float speed)
    {
        if (path == null || path.Count == 0)
            yield break;

        foreach (Node node in path)
        {
            Vector3 targetPos = node.worldPosition;

            while (Vector3.Distance(seeker.position, targetPos) > 0.05f)
            {
                seeker.position = Vector3.MoveTowards(
                    seeker.position,
                    targetPos,
                    speed * Time.deltaTime
                );

                yield return null;
            }
        }
    }


    /////////////////////////////////////////////////////////////////////
    // ARRIVAL BEHAVIOR ---------------------------------------------------
    /////////////////////////////////////////////////////////////////////

    IEnumerator HandleArrival(float interval)
    {
        // Wait before doing anything
        yield return new WaitForSeconds(interval);

        // At a shelf waypoint
        if (waypoint.CompareTag("Waypoint"))
        {
            ShelfInventory shelfInv = currentShelf.GetComponent<ShelfInventory>();
            ProductEntry entry = currentItem;

            npcBehaviour.AddToCart(shelfInv, entry.product, entry.quantity);
        }
        // Checkout
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
        
        // Get shelf ideal item evaluator
        ShelfInventory shelfInventory = shelf.GetComponent<ShelfInventory>();
        ItemValue itemValue = shelfInventory.GetComponent<ItemValue>();
        if (itemValue == null) return false;

        // Check if the shelf contains ANY ideal item
        ItemObject ideal = itemValue.GetBestIdealItem();
        if (ideal == null) return false;
        
        // Boost the multiplier for this specific ideal item
        impulsiveBuying.AddImpulseFactor(ideal, 0.5f);
        bool bought = impulsiveBuying.TryImpulseBuy(ideal);

        if (bought)
        {
            currentShelf = shelf;
            currentItem = new ProductEntry(ideal, Random.Range(0, 5)); // randomly assign number of items to buy

            Debug.Log($"NPC wants {ideal?.name ?? "something"}!");
        }

        return bought;
    }

}
