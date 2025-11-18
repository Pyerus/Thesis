using UnityEngine;

public class NPCData : MonoBehaviour
{
    // new string[] { "Algorithm", "NPC", "Item", "Quantity", "Listed or Impulsive", "Item Value", "Position Multiplier", "Promotion Multiplier", "Visibility", "Layout", "Number of Posters", "Promotional Score" }
    public string algorithm; //
    public int npc; //
    public ItemObject item; //
    public int quantity; //
    public ListOrImpulse listOrImpulse; //
    public float itemValue; //
    public float positionMultiplier; //
    public float promotionMultiplier; //
    public float visibilityScore; //
    public float layoutScore; //
    public int numOfPosters;
    public float promoScore;

    private CSVWriter writer;
    private ShelvesManager shelvesManager;
    private ImpulsiveBuying impulsiveBuying;

    private void Start()
    {
        writer = FindFirstObjectByType<CSVWriter>();
        
        // Information available right after instantiating.
        algorithm = FindFirstObjectByType<NPCSpawner>().algorithm;
        npc = FindFirstObjectByType<NPCSpawner>().npcNo;

        impulsiveBuying = FindFirstObjectByType<ImpulsiveBuying>();
        shelvesManager = FindFirstObjectByType<ShelvesManager>();
    }

    // Called everytime the NPC takes an item.
    public void NewItem(ProductEntry entry, ListOrImpulse _listOrImpulse, float _itemValue, float _positionMultiplier)
    {
        item = entry.product;
        quantity = entry.quantity;
        listOrImpulse = _listOrImpulse;
        itemValue = _itemValue;
        positionMultiplier = _positionMultiplier;
        promotionMultiplier = 1f + (0.05f * impulsiveBuying.noOfPosters);
        visibilityScore = _itemValue * _positionMultiplier * (1 - impulsiveBuying.nullChance);
        layoutScore = shelvesManager.CalculateLayoutScore();
        numOfPosters = impulsiveBuying.noOfPosters;
        promoScore = shelvesManager.CalculatePromotionScore();

        RecordData();
    }

    // This method is called for every item taken.
    public void RecordData()
    {
        writer.AppendRow(
            algorithm,
            npc.ToString(),
            item.name,
            quantity.ToString(),
            listOrImpulse.ToString(),
            itemValue.ToString(),
            positionMultiplier.ToString(),
            promotionMultiplier.ToString(),
            visibilityScore.ToString(),
            layoutScore.ToString(),
            numOfPosters.ToString(),
            promoScore.ToString()
            );
    }
}
