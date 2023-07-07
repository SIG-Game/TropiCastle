using System.Collections.Generic;
using UnityEngine;

public class Crafting : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private bool verboseLogging;

    public bool TryCraftItem(CraftingRecipeScriptableObject craftingRecipe)
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

                return false;
            }
        }

        Dictionary<int, ItemWithAmount> itemIndexToItemBeforeRemoval =
            new Dictionary<int, ItemWithAmount>();

        foreach (var itemIndexAndRemoveAmount in itemIndexToRemoveAmount)
        {
            int itemToRemoveIndex = itemIndexAndRemoveAmount.Key;
            int itemAmountToRemove = itemIndexAndRemoveAmount.Value;

            ItemWithAmount itemToRemove = itemList[itemToRemoveIndex];

            if (!itemIndexToItemBeforeRemoval.ContainsKey(itemToRemoveIndex))
            {
                itemIndexToItemBeforeRemoval.Add(itemToRemoveIndex,
                    new ItemWithAmount(itemToRemove));
            }

            if (verboseLogging)
            {
                Debug.Log($"Using {itemAmountToRemove} {itemToRemove.itemData.name} " +
                    $"from index {itemToRemoveIndex}");
            }

            int newItemAmount = itemToRemove.amount - itemAmountToRemove;
            inventory.SetItemAmountAtIndex(newItemAmount, itemToRemoveIndex);
        }

        if (inventory.CanAddItem(craftingRecipe.resultItem))
        {
            inventory.AddItem(craftingRecipe.resultItem);

            return true;
        }
        else
        {
            RevertItemRemoval(itemIndexToItemBeforeRemoval);

            return false;
        }
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

    private void RevertItemRemoval(
        Dictionary<int, ItemWithAmount> itemIndexToItemBeforeRemoval)
    {
        foreach (var itemIndexAndItemRemoved in itemIndexToItemBeforeRemoval)
        {
            int removedItemIndex = itemIndexAndItemRemoved.Key;
            ItemWithAmount removedItem = itemIndexAndItemRemoved.Value;

            inventory.SetItemAtIndex(removedItem, removedItemIndex);
        }
    }
}
