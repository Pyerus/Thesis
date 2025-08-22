using UnityEngine;

public class Cursors : MonoBehaviour
{
    public Texture2D cursor;
    public Texture2D cursorClicked;

    private CursorControls controls;

    private Camera mainCamera;

    private bool menuActivated;
    public GameObject InventoryMenu;
    void Awake()
    {
        ChangeCursor(cursor);
        Cursor.lockState = CursorLockMode.Confined;
        controls = new CursorControls();
        mainCamera = Camera.main;
    }
    void Start()
    {
        controls.Mouse.Click.started += _ => StartedClick();
        controls.Mouse.Click.performed += _ => EndedClick();
    }

    public void StartedClick() {
        ChangeCursor(cursorClicked);
    }

    public void EndedClick()
    {
        ChangeCursor(cursor);
        DetectObject();
    }

    public void DetectObject()
    {
        Ray ray = mainCamera.ScreenPointToRay(controls.Mouse.Position.ReadValue<Vector2>());
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Shelf"))
            {
                Time.timeScale = 0;
                menuActivated = !menuActivated; // toggle true/false
                InventoryMenu.SetActive(menuActivated);
            }
        }
    }

    private void ChangeCursor(Texture2D cursorType)
    {
        //Vector2 hotspot = new Vector2(cursorType.width / 2, cursorType.height / 2);
        Cursor.SetCursor(cursorType, Vector2.zero, CursorMode.Auto);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   

    private void OnEnable()
    {
        controls.Enable();
    }
    private void OnDisable()
    {
        controls.Disable();
    }
}
