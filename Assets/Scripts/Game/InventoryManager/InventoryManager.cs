using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;
    public Cursors click;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.CompareTag(""))
        {
            InventoryMenu.SetActive(true);
        }
    }
}
