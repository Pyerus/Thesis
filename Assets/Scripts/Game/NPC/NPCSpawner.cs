using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCSpawner : MonoBehaviour
{
    public GameObject spawnPoint;

    public GameObject npcPrefab;

    float spawnInterval = 1f;

    public int spawnLimit = 10;

    int spawnCount = 0;
    [SerializeField] Button OpenStore;
    [SerializeField] TMP_Text buttonText;
    bool storeOpen = false;

    private Vector3 spawnLocation;


    private void Start()
    {
        spawnLocation = spawnPoint.transform.position;

        if (OpenStore != null)
            OpenStore.onClick.AddListener(OnOpenStoreClicked);
    }

    private void OnOpenStoreClicked()
    {
        storeOpen = !storeOpen; // Toggle

        if (storeOpen)
        {
            buttonText.text = "Close";
            StartCoroutine(SpawnNPC(spawnInterval, npcPrefab));
        }
        else
        {
            buttonText.text = "Open";
            StopAllCoroutines(); 
            NPC.KillNPCs();
        }
    }

    public IEnumerator SpawnNPC(float interval, GameObject npc)
    {
        spawnCount = GameObject.FindGameObjectsWithTag("NPC").Length;
        while (spawnCount < spawnLimit)
        {
            Instantiate(npc, spawnLocation, Quaternion.identity);
            yield return new WaitForSeconds(interval);
        }
    }
}
