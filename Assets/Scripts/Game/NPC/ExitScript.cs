using UnityEngine;

public class ExitScript : MonoBehaviour
{
    public GameObject npc;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exit"))
            Destroy(npc);
    }
}
