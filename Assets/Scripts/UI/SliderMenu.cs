using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SliderMenu : MonoBehaviour
{
    public GameObject PanelMenu;

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
