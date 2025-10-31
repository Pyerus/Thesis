using System.Collections;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab;

    float spawnInterval = 1f;

    public int spawnLimit = 10;

    int spawnCount = 0;


    private void Start()
    {
        StartCoroutine(SpawnNPC(spawnInterval, npcPrefab));
    }

    public IEnumerator SpawnNPC(float interval, GameObject npc)
    {
        spawnCount = 0; 
        while (spawnCount < spawnLimit)
        {
            Instantiate(npc, new Vector3(0, 0.1f, -5), Quaternion.identity);
            spawnCount++;
            yield return new WaitForSeconds(interval);
        }
    }
}
