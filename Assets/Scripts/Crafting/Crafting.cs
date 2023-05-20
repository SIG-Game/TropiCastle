using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private bool verboseLogging;

    public void CraftItem(CraftingRecipeScriptableObject craftingRecipe)
    {
        List<ItemWithAmount> itemList = inventory.GetItemList();

        Dictionary<int, int> itemIndexToRemoveAmount = new Dictionary<int, int>();

        foreach (ItemWithAmount ingredient in craftingRecipe.ingredients)
        {
            if (!TryFindIngredient(itemList, itemIndexToRemoveAmount, ingredient))
            {
                if (verboseLogging)
                {
                    Debug.Log($"{ingredient.amount} {ingredient.itemData.name} not found");
                }

                return;
            }
        }

        foreach (var itemIndexAndRemoveAmount in itemIndexToRemoveAmount)
        {
            ItemWithAmount itemToRemove = itemList[itemIndexAndRemoveAmount.Key];

            if (verboseLogging)
            {
                Debug.Log($"Using {itemIndexAndRemoveAmount.Value} " +
                    $"{itemToRemove.itemData.name} from index " +
                    itemIndexAndRemoveAmount.Key);
            }

            inventory.RemoveItemAtIndex(itemIndexAndRemoveAmount.Key);

            int amountLeft = itemToRemove.amount - itemIndexAndRemoveAmount.Value;

            if (amountLeft != 0)
            {
                ItemWithAmount itemToAddBack = new ItemWithAmount(itemToRemove.itemData,
                    amountLeft);

                itemToAddBack.SetItemInstanceProperties();

                inventory.AddItemAtIndexWithFallbackToFirstEmptyIndex(itemToAddBack,
                    itemIndexAndRemoveAmount.Key);
            }
        }

        inventory.AddItem(craftingRecipe.resultItem);
    }

    public bool TryFindIngredient(List<ItemWithAmount> itemList,
        Dictionary<int, int> itemIndexToExcludeAmount, ItemWithAmount ingredient)
    {
        int amountExcluded = 0;

        for (int i = 0; i < itemList.Count; ++i)
        {
            ItemWithAmount currentItem = itemList[i];

            if (currentItem.itemData.name == ingredient.itemData.name)
            {
                int currentItemAmount = currentItem.amount;

                if (itemIndexToExcludeAmount.ContainsKey(i))
                {
                    currentItemAmount -= itemIndexToExcludeAmount[i];
                }

                if (currentItemAmount + amountExcluded >= ingredient.amount)
                {
                    AddToDictionaryOrIncreaseValue(itemIndexToExcludeAmount,
                        i, ingredient.amount - amountExcluded);

                    return true;
                }
                else
                {
                    AddToDictionaryOrIncreaseValue(itemIndexToExcludeAmount,
                        i, currentItemAmount);

                    amountExcluded += currentItemAmount;
                }
            }
        }

        return false;
    }

    private void AddToDictionaryOrIncreaseValue(Dictionary<int, int> dictionary,
        int key, int value)
    {
        if (dictionary.ContainsKey(key))
        {
            dictionary[key] += value;
        }
        else
        {
            dictionary.Add(key, value);
        }
    }
}
