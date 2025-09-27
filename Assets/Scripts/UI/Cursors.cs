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
                OpenMenu();
            }
        }
    }

    public void OpenMenu()
    {
        if (!menuActivated)
        {
            menuActivated = true;
            InventoryMenu.SetActive(true);
        }
    }

    public void CloseMenu()
    {
        menuActivated = false;
        InventoryMenu.SetActive(false);
    }

    private void ChangeCursor(Texture2D cursorType)
    {
        Cursor.SetCursor(cursorType, Vector2.zero, CursorMode.Auto);
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
}
