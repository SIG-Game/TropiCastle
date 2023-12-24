using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NPCItemOfferingSelector : MonoBehaviour
{
    [SerializeField] private NPCItemOfferingScriptableObject itemOffering;

    private WeightedRandomSelector itemSelector;

    private void Awake()
    {
        List<float> itemWeights = itemOffering.PotentialItemsToGive
            .Select(x => x.ProbabilityWeight).ToList();

        itemSelector = new WeightedRandomSelector(itemWeights);
    }

    public ItemStackStruct SelectItemToGive()
    {
        int itemToGiveIndex = itemSelector.SelectIndex();
        return itemOffering.PotentialItemsToGive[itemToGiveIndex].Item;
    }
}
