using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Transform itemWorld;

    public Sprite testSprite;
    public Sprite appleSprite;
}
