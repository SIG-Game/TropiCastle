using UnityEngine;

public class ItemAssets : MonoBehaviour
{
    public static ItemAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Transform itemWorld;

    public Sprite transparentSprite;
    public Sprite testSprite;
    public Sprite appleSprite;
    public Sprite stickSprite;
    public Sprite spearSprite;
    public Sprite rockSprite;
    public Sprite vineSprite;
    public Sprite rawCrabMeatSprite;
    public Sprite cookedCrabMeatSprite;
    public Sprite campfireSprite;
    public Sprite fishingRodSprite;
    public Sprite fishSprite;
}
