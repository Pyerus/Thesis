using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CamDragRotate : MonoBehaviour
{
    [SerializeField] InputAction pressed, axis;

    bool rotateAllowed = false;
    Vector2 rotation;
    float speed = 0.5f;

    private void Awake()
    {
        pressed.Enable();
        axis.Enable();

        pressed.performed += _ => { StartCoroutine(Rotate()); };
        pressed.canceled += _ => { rotateAllowed = false; };

        axis.performed += context => { rotation = context.ReadValue<Vector2>(); };
    }

    IEnumerator Rotate()
    {
        rotateAllowed = true;
        while (rotateAllowed)
        {
            rotation *= speed;
            transform.Rotate(Vector3.up, rotation.x, Space.World);
            yield return null;
        }
    }
}
