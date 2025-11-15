using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NPCSpawner : MonoBehaviour
{
    public TextMeshProUGUI npcCounter;

    public GameObject spawnPoint;

    public GameObject[] npcPrefabs;

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

    private void Update()
    {
        npcCounter.text = "NPC: " + spawnCount;
    }

    private void OnOpenStoreClicked()
    {
        storeOpen = !storeOpen; // Toggle

        if (storeOpen)
        {
            buttonText.text = "Close";
            StartCoroutine(SpawnNPC(spawnInterval, npcPrefabs));
        }
        else
        {
            buttonText.text = "Open";
            StopAllCoroutines(); 
            NPC.KillNPCs();
        }
    }

    public IEnumerator SpawnNPC(float interval, GameObject[] npcPrefabs)
    {
        while (storeOpen)
        {
            spawnCount = GameObject.FindGameObjectsWithTag("NPC").Length;

            if (spawnCount < spawnLimit)
            {
                GameObject npc = npcPrefabs[Random.Range(0, npcPrefabs.Length)];
                Instantiate(npc, spawnLocation, Quaternion.identity);
            }
            
            yield return new WaitForSeconds(interval);
        }
    }
}
