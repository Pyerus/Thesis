using UnityEngine;
using UnityEngine.InputSystem;

public class CamZoom : MonoBehaviour
{
    [SerializeField] InputAction scroll;

    float scrollValue;

    private void Awake()
    {
        scroll.performed += x => scrollValue = x.ReadValue<float>();
    }

    private void Update()
    {
        if (scrollValue != 0) {
            Zoom();
        }
    }



    void Zoom()
    {
        float zPos = scrollValue;
        transform.Translate(0, 0, zPos);
    }
    



    private void OnEnable()
    {
        scroll.Enable();
    }

    private void OnDisable()
    {
        scroll.Disable();
    }
}
