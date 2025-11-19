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

    private ListOrImpulse listOrImpulse = ListOrImpulse.List;



    void Start()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<WorldGrid>();
        shelvesManager = GameObject.FindFirstObjectByType<ShelvesManager>();

        npcBehaviour = GetComponent<NPCBehaviour>();
        shoppingList = GetComponent<ShoppingList>();
        impulsiveBuying = FindFirstObjectByType<ImpulsiveBuying>();

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
                listOrImpulse = ListOrImpulse.Impulse;
                waypoint = randomShelf.transform.Find("Waypoint").gameObject; // get random shelf's waypoint
                Debug.Log($"NPC is buying {currentItem.product?.name ?? "something"}!");
            }
            else
            {
                listOrImpulse = ListOrImpulse.List;
                waypoint = GetNextWaypoint(); // get next waypoint on the list
            }

            if (waypoint == null)
            {
                Debug.Log("NPC has no waypoint. Ending.");
                yield break;
            }

            target.position = waypoint.transform.position;

            // 2. Compute path
            FindPath(seeker.position, target.position);

            // 3. Move along the computed path
            yield return StartCoroutine(MoveAlongPathWithHop(moveSpeed, 0.5f, 1f));

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

    IEnumerator MoveAlongPathWithHop(float speed, float hopHeight, float hopFrequency)
    {
        if (path == null || path.Count == 0) yield break;

        foreach (Node node in path)
        {
            Vector3 startPos = seeker.position;
            Vector3 targetPos = node.worldPosition;

            float journeyLength = Vector3.Distance(startPos, targetPos);
            float traveled = 0f;

            while (traveled < journeyLength)
            {
                // Step distance
                float step = speed * Time.deltaTime;

                // Move horizontally toward the target
                seeker.position = Vector3.MoveTowards(seeker.position, targetPos, step);

                // Hopping effect
                float t = traveled / journeyLength;  // 0 → 1
                float yOffset = Mathf.Sin(t * Mathf.PI * hopFrequency) * hopHeight;
                seeker.position = new Vector3(seeker.position.x, startPos.y + yOffset, seeker.position.z);

                // Face the direction of movement
                Vector3 direction = (targetPos - seeker.position);
                direction.y = 0f; // Keep rotation horizontal
                if (direction.sqrMagnitude > 0.001f)
                {
                    seeker.rotation = Quaternion.Slerp(seeker.rotation, Quaternion.LookRotation(direction), 10f * Time.deltaTime);
                }

                traveled += step;
                yield return null;
            }

            // Snap to exact node position and reset vertical
            seeker.position = new Vector3(targetPos.x, startPos.y, targetPos.z);
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
            ProductEntry entry = currentItem;

            npcBehaviour.AddToCart(currentShelf, entry.product, entry.quantity, listOrImpulse);
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

    bool HandleImpulseBuying(GameObject _shelf)
    {
        if (_shelf == null) return false;

        ShelfInventory inventory = _shelf.GetComponent<ShelfInventory>();

        if (inventory == null) return false;

        // Now attempt impulse buy from the items in the shelf
        ItemObject itemBought = impulsiveBuying.TryImpulseBuy(_shelf);

        if (itemBought == null) return false;

        // If NPC decides to buy
        currentShelf = _shelf;
        currentItem = new ProductEntry(itemBought, Random.Range(0, 5));

        Debug.Log($"NPC impulse bought {itemBought?.name ?? "something"}!");

        return true;
    }

}
