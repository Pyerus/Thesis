using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;

public class Cursors : MonoBehaviour
{
    public Texture2D cursor;
    public Texture2D cursorClicked;

    private CursorControls controls;
    private Camera mainCamera;

    private bool menuActivated;
    public GameObject InventoryMenu;
    public DisplayInventory inventorySlots;

    private ShelfInventory shelfInventory;

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

    public void DetectObject()
    {

        if (IsPointerOverUIButton())
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
                return true; // mouse is over a clickable UI element
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
