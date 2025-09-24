using System.Collections;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] GameObject npcPrefab;

    float spawnInterval = 1f;

    int spawnLimit = 5;

    int spawnCount = 0;


    private void Start()
    {
        StartCoroutine(SpawnNPC(spawnInterval, npcPrefab));
    }

    IEnumerator SpawnNPC(float interval, GameObject npc)
    {
        yield return new WaitForSeconds(interval);

        GameObject newNPC = Instantiate(npc, new Vector3(0, 0.1f, -5), Quaternion.identity);

        spawnCount++;

        if (spawnCount <= spawnLimit)
        {
            StartCoroutine(SpawnNPC(interval, npc));
        }
    }
}
