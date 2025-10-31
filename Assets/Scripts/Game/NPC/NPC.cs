using UnityEngine;

public class NPC : MonoBehaviour
{
    [SerializeField] float LifeSpan = 0f;
    [SerializeField] float MaxLifeSpan = 20f;
    void Update()
    {
        LifeSpan += Time.deltaTime;

            if (LifeSpan >= MaxLifeSpan)
            {
                LifeSpan = MaxLifeSpan;
                Destroy(gameObject);
                //There should be a code here to kill all current alive NPC
            }

    }
}
