using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCSpawner : MonoBehaviour
{
    public Algo algorithm;
    public GameObject spawnPoint;
    public GameObject[] npcPrefabs;
    float spawnInterval = 1f;
    public int spawnLimit = 10;
    int spawnCount = 0;
    [SerializeField] Button OpenStore;
    [SerializeField] TMP_Text buttonText;
    bool storeOpen = false;

    private Vector3 spawnLocation;

    public int npcNo = 0;

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
            // Logic to OPEN the store
            buttonText.text = "Close";
            StartCoroutine(SpawnNPC(npcPrefabs));
        }
        else
        {
            // Logic to CLOSE the store (now uses the public function)
            CloseStore();
        }
    }

    // This can be called from other scripts (like GameTimer)
    public void CloseStore()
    {
        storeOpen = false;
        buttonText.text = "Open";
        StopAllCoroutines();
    }

    public IEnumerator SpawnNPC(GameObject[] npcPrefabs)
    {
        while (storeOpen)
        {
            float interval = Random.Range(1f, 5f);
            
            spawnCount = GameObject.FindGameObjectsWithTag("NPC").Length;

            if (spawnCount < spawnLimit)
            {
                npcNo++;
                GameObject npc = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
                Instantiate(npc, spawnLocation, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(interval);
        }
    }


    /////////////////////////////////////////////////////////////////////
    // BUG FIX ----------------------------------------------------------
    /////////////////////////////////////////////////////////////////////

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}

public enum Algo
{
    Dijkstra,
    Modified
}