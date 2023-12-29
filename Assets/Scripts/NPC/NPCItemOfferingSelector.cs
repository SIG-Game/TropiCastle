using System.Collections.Generic;
using System.Linq;

public class NPCItemOfferingSelector
{
    private NPCItemOfferingScriptableObject itemOffering;
    private WeightedRandomSelector itemSelector;

    public NPCItemOfferingSelector(NPCItemOfferingScriptableObject itemOffering)
    {
        this.itemOffering = itemOffering;

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
