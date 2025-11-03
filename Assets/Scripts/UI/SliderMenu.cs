using UnityEngine;

public class SliderMenu : MonoBehaviour
{
    public GameObject PanelMenu;

    void Start()
    {
        if (PanelMenu != null)
        {
            Animator animator = PanelMenu.GetComponent<Animator>();
            if (animator != null)
            {
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;
                animator.SetBool("show", false); 
            }
        }
    }

    public void ShowHideMenu()
    {
        if (PanelMenu != null)
        {
            Animator animator = PanelMenu.GetComponent<Animator>();
            if (animator != null)
            {
                animator.updateMode = AnimatorUpdateMode.UnscaledTime;

                bool isOpen = animator.GetBool("show");
                animator.SetBool("show", !isOpen);
            }
            else
            {
                Debug.LogError("Animator component not found on PanelMenu.");
            }
        }
        else
        {
            Debug.LogError("PanelMenu is not assigned.");
        }
    }
}
