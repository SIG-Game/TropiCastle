using UnityEngine;

public class WeaponAssets : MonoBehaviour
{
    public static WeaponAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public Sprite stickSprite;
    public Sprite spearSprite;
}
