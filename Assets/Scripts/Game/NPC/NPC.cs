using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] float LifeSpan = 0f;
    [SerializeField] float MaxLifeSpan = 298f;
    public static bool StoreClosed = false;

    void Update()
    {
        LifeSpan += Time.deltaTime;

        if (LifeSpan >= MaxLifeSpan)
        {
            LifeSpan = MaxLifeSpan;
            StoreClosed = true; // Close store globally
        }
    }

    //public static void KillNPCs()
    //{
    //    NPCMovement[] npcs = FindObjectsOfType<NPCMovement>();

    //    foreach (NPCMovement npc in npcs)
    //    {
    //        npc.isClosed = true;     // Force them to go to checkout/exit
    //    }
    //}
}
