using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

public class ItemWorldPrefabInstanceFactory : MonoBehaviour
{
    [SerializeField] private GameObject itemWorldPrefab;
    [SerializeField] private ItemInteractableDependencies itemInteractableDependencies;
    [SerializeField] private Transform itemWorldParent;
    [SerializeField] private Vector2 itemWorldPrefabColliderExtents;

    private const int maxDropSpawnAttempts = 20;

    public static ItemWorldPrefabInstanceFactory Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

#if UNITY_EDITOR
    [ContextMenu("Set Item World Prefab Collider Extents")]
    private void SetItemWorldPrefabColliderExtents()
    {
        // BoxCollider2D bounds property (which has extents property) is not set
        // until prefab is instantiated, so that property cannot be used here
        itemWorldPrefabColliderExtents = itemWorldPrefab.GetComponent<BoxCollider2D>().size / 2f;

        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
    }
#endif

    public ItemWorld SpawnItemWorld(Vector3 position, ItemStack itemToSpawn)
    {
        GameObject spawnedGameObject = Instantiate(itemWorldPrefab, position, Quaternion.identity, itemWorldParent);
        ItemWorld spawnedItemWorld = spawnedGameObject.GetComponent<ItemWorld>();

        spawnedItemWorld.SetUpItemWorld(itemToSpawn, itemInteractableDependencies);

        return spawnedItemWorld;
    }

    public void DropItem(Vector3 dropPosition, ItemStack itemToDrop)
    {
        if (itemToDrop.itemDefinition.IsEmpty())
        {
            return;
        }

        Vector2 spawnPositionGenerator()
        {
            Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            randomOffset.Normalize();
            randomOffset *= 0.7f;
            return dropPosition + randomOffset;
        }

        Vector2 itemColliderExtents = GetItemColliderExtents(itemToDrop.itemDefinition);

        if (SpawnColliderHelper.TryGetSpawnPositionOutsideColliders(spawnPositionGenerator, itemColliderExtents,
            maxDropSpawnAttempts, out Vector2 spawnPosition))
        {
            _ = SpawnItemWorld(spawnPosition, itemToDrop);
        }

        // Dropped items currently don't move
        //itemWorld.GetComponent<Rigidbody2D>().AddForce(randomOffset * 0.5f, ForceMode2D.Impulse);
    }

    public static Vector2 GetItemColliderExtents(ItemScriptableObject itemDefinition)
    {
        Vector2 itemColliderExtents;

        if (itemDefinition.HasCustomColliderSize)
        {
            itemColliderExtents = itemDefinition.CustomColliderSize / 2f;
        }
        else
        {
            itemColliderExtents = Instance.itemWorldPrefabColliderExtents;
        }

        return itemColliderExtents;
    }

    public static Vector2 GetItemColliderSize(ItemScriptableObject itemDefinition)
    {
        Vector2 itemColliderSize;

        if (itemDefinition.HasCustomColliderSize)
        {
            itemColliderSize = itemDefinition.CustomColliderSize;
        }
        else
        {
            itemColliderSize = Instance.itemWorldPrefabColliderExtents * 2f;
        }

        return itemColliderSize;
    }
}
