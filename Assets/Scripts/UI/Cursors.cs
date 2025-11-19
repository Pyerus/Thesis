using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class Cursors : MonoBehaviour
{
    [Header("Cursor Textures")]
    public Texture2D cursor;
    public Texture2D cursorClicked;

    private CursorControls controls;
    private Camera mainCamera;

    [Header("Inventory Menu (Shelf UI)")]
    public GameObject InventoryMenu;     // The one that toggles active on shelf click
    public DisplayInventory inventorySlots;

    private ShelfInventory shelfInventory;
    private bool menuActivated;

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

        if (InventoryMenu != null)
            InventoryMenu.SetActive(false);
    }

    public void StartedClick()
    {
        ChangeCursor(cursorClicked);
    }

    public void EndedClick()
    {
        ChangeCursor(cursor);
        DetectObject();
    }

    private void DetectObject()
    {
        // Stop clicking shelves if UI button is under cursor or if the shelf menu is active
        if (IsPointerOverUIButton() || (InventoryMenu != null && InventoryMenu.activeInHierarchy))
            return;

        Ray ray = mainCamera.ScreenPointToRay(controls.Mouse.Position.ReadValue<Vector2>());
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Shelf"))
            {
                shelfInventory = hit.collider.GetComponent<ShelfInventory>();
                OpenMenu();
            }
        }
    }

    private bool IsPointerOverUIButton()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = controls.Mouse.Position.ReadValue<Vector2>();

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            var go = result.gameObject;

            if (go.GetComponent<Button>() != null ||
                go.GetComponent<Toggle>() != null ||
                go.GetComponent<Slider>() != null ||
                go.GetComponent<Dropdown>() != null ||
                go.GetComponent<Scrollbar>() != null ||
                go.GetComponent<InputField>() != null ||
                go.GetComponentInParent<Button>() != null ||
                go.GetComponentInParent<Toggle>() != null ||
                go.GetComponentInParent<Slider>() != null ||
                go.GetComponentInParent<Dropdown>() != null ||
                go.GetComponentInParent<Scrollbar>() != null ||
                go.GetComponentInParent<InputField>() != null)
            {
                return true;
            }
        }

        return false;
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

    public ShelfInventory GetShelfInventory()
    {
        return shelfInventory;
    }
}
