using UnityEngine;

public class NPCData : MonoBehaviour
{
    // new string[] { "Algorithm", "NPC", "Item", "Quantity", "Listed or Impulsive", "Item Value", "Base Value", "Position Multiplier", "Promotion Multiplier", "Visibility", "Layout", "Number of Posters", "Promotional Score" }
    public Algo algorithm; //
    public int npc; //
    public ItemObject item; //
    public int quantity; //
    public ListOrImpulse listOrImpulse; //
    public float itemValue; //
    public float baseValue; //
    public float positionMultiplier; //
    public float promotionMultiplier; //
    public float visibilityScore; 
    public float layoutScore; 
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
    public void NewItem(ProductEntry entry, ListOrImpulse _listOrImpulse, float _itemValue, float _baseValue, float _positionMultiplier)
    {
        item = entry.product;
        quantity = entry.quantity;
        listOrImpulse = _listOrImpulse;
        itemValue = _itemValue;
        baseValue = _baseValue;
        positionMultiplier = _positionMultiplier;
        promotionMultiplier = impulsiveBuying.posterMultiplier;
        visibilityScore = _itemValue * impulsiveBuying.posterMultiplier;
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
            npc,
            item.name,
            quantity,
            listOrImpulse,
            itemValue,
            baseValue,
            positionMultiplier,
            promotionMultiplier,
            visibilityScore,
            layoutScore,
            numOfPosters,
            promoScore
            );
    }
}
