using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    // Serializable
    [SerializeField] float movementSpeed = 10f;
    [SerializeField] Transform seeker;
    [SerializeField] Transform target;

    // Background processes
    private WorldGrid grid;
    private PathfindDijkstra pathfinder;
    private List<Node> path;
    bool isCoroutineRunning = false;

    // NPC waypoint destinations
    private ShoppingList shoppingList; // the script
    private List<ProductEntry> shopList; // the actual list
    private GameObject[] waypoints;
    private GameObject[] shelves;
    private Transform targetWaypoint;
    private int currentIndex = 0;
    private bool goingToCheckout = false;

    // NPC behavior
    private NPCBehaviour npcBehaviour;

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

        shoppingList = gameObject.GetComponent<ShoppingList>();
        shopList = shoppingList.generatedList;
        waypoints = shoppingList.GetWaypoints();
        shelves = shoppingList.GetShelves();

        targetWaypoint = GetNewWaypoint();
        target.transform.position = targetWaypoint.position;

        baseY = seeker.position.y; // record starting Y


        npcBehaviour = gameObject.GetComponent<NPCBehaviour>();
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
            // Find new target
            StartCoroutine(NewTarget(1f));
        }

        // Apply hopping if active
        if (isHopping)
        {
            float hopOffset = Mathf.Sin(Time.time * hopSpeed) * hopHeight;
            seeker.position = new Vector3(seeker.position.x, baseY + hopOffset, seeker.position.z);
        }
    }

    

    

    private Transform GetNewWaypoint()
    {
        // If there are no waypoints or all are invalid
        if (waypoints == null || waypoints.Length == 0)
        {
            Debug.LogWarning("No waypoints available for NPC.");
            return null;
        }

        // If we've reached the end of the shopping list, go to checkout
        if (currentIndex >= waypoints.Length)
        {
            if (!goingToCheckout)
            {
                goingToCheckout = true;
                GameObject checkout = GameObject.FindGameObjectWithTag("Checkout");

                if (checkout != null)
                {
                    Debug.Log("All items collected � heading to checkout!");
                    return checkout.transform;
                }
                else
                {
                    Debug.LogWarning("Checkout not found in scene.");
                    return null;
                }
            }
            else
            {
                // Proceed to exit after checking out
                GameObject exit = GameObject.FindGameObjectWithTag("Exit");
                Debug.Log("Heading to exit.");
                return exit.transform;
            }
        }

        // Otherwise, continue through the waypoints list
        GameObject waypoint = waypoints[currentIndex];
        currentIndex++;

        if (waypoint != null)
        {
            Debug.Log($"Heading to waypoint {currentIndex}/{waypoints.Length}: {waypoint.name}");
            return waypoint.transform;
        }
        else
        {
            Debug.LogWarning($"Waypoint {currentIndex} is null, skipping.");
            return GetNewWaypoint(); // recursively skip nulls
        }
    }


    private void AddItemToCart()
    {
        if (targetWaypoint != null && targetWaypoint.CompareTag("Waypoint"))
        {
            ShelfInventory _shelfInventory = shelves[currentIndex-1].gameObject.GetComponent<ShelfInventory>();
            ProductEntry _productEntry = shopList[currentIndex-1];

            npcBehaviour.AddToCart(_shelfInventory, _productEntry.product, _productEntry.quantity);
        }
        else if (targetWaypoint != null && targetWaypoint.CompareTag("Checkout"))
        {
            npcBehaviour.CheckOut();
        }
    }



    IEnumerator NewTarget(float interval)
    {
        isCoroutineRunning = true;
        AddItemToCart();

        yield return new WaitForSeconds(interval);

        targetWaypoint = GetNewWaypoint();

        if (targetWaypoint != null)
        {
            target.transform.position = targetWaypoint.position;
        }
        else
        {
            Debug.Log("No more waypoints � NPC is done shopping.");
        }

        isCoroutineRunning = false;
    }
}
