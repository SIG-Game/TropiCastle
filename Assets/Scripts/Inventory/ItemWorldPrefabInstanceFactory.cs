using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ItemWorldPrefabInstanceFactory : MonoBehaviour
{
    [SerializeField] private GameObject itemWorldPrefab;
    [SerializeField] private Transform itemWorldParent;
    [SerializeField] private Vector2 itemWorldPrefabColliderExtents;

    private Dictionary<string, IItemInteraction> itemNameToInteraction;
    private CampfireItemInteraction campfireItemInteraction;

    private const int maxDropSpawnAttempts = 20;

    public static ItemWorldPrefabInstanceFactory Instance;

    private void Awake()
    {
        Instance = this;

        campfireItemInteraction = new CampfireItemInteraction();

        itemNameToInteraction = new Dictionary<string, IItemInteraction>
        {
            { "Campfire", campfireItemInteraction }
        };
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    [ContextMenu("Set Item World Prefab Collider Extents")]
    private void SetItemWorldPrefabColliderExtents()
    {
        // BoxCollider2D bounds property (which has extents property) is not set
        // until prefab is instantiated, so that property cannot be used here
        itemWorldPrefabColliderExtents = itemWorldPrefab.GetComponent<BoxCollider2D>().size / 2f;

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }

    public void SpawnItemWorld(Vector3 position, ItemWithAmount itemToSpawn)
    {
        GameObject spawnedGameObject = Instantiate(itemWorldPrefab, position, Quaternion.identity, itemWorldParent);
        ItemWorld spawnedItemWorld = spawnedGameObject.GetComponent<ItemWorld>();

        spawnedItemWorld.item = itemToSpawn;
        spawnedItemWorld.ItemInteraction = itemNameToInteraction.GetValueOrDefault(itemToSpawn.itemData.name);
    }

    public void DropItem(Vector3 dropPosition, ItemWithAmount itemToDrop)
    {
        if (itemToDrop.itemData.name == "Empty")
            return;

        Vector2 spawnPositionGenerator()
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            randomOffset.Normalize();
            randomOffset *= 0.7f;
            return dropPosition + randomOffset;
        }

        Vector2 itemColliderExtents = GetItemColliderExtents(itemToDrop.itemData);

        if (SpawnColliderHelper.TryGetSpawnPositionOutsideColliders(spawnPositionGenerator, itemColliderExtents,
            maxDropSpawnAttempts, out Vector2 spawnPosition))
        {
            SpawnItemWorld(spawnPosition, itemToDrop);
        }

        // Dropped items currently don't move
        //itemWorld.GetComponent<Rigidbody2D>().AddForce(randomOffset * 0.5f, ForceMode2D.Impulse);
    }

    public static Vector2 GetItemColliderExtents(ItemScriptableObject itemData)
    {
        Vector2 itemColliderExtents;

        if (itemData.hasCustomColliderSize)
        {
            itemColliderExtents = itemData.customColliderSize / 2f;
        }
        else
        {
            itemColliderExtents = Instance.itemWorldPrefabColliderExtents;
        }

        return itemColliderExtents;
    }

    public static Vector2 GetItemColliderSize(ItemScriptableObject itemData)
    {
        Vector2 itemColliderSize;

        if (itemData.hasCustomColliderSize)
        {
            itemColliderSize = itemData.customColliderSize;
        }
        else
        {
            itemColliderSize = Instance.itemWorldPrefabColliderExtents * 2f;
        }

        return itemColliderSize;
    }
}
