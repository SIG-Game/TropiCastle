using UnityEngine;

[CreateAssetMenu(menuName = "Fish Item")]
public class FishItemScriptableObject : ItemScriptableObject
{
    public string description;
    public int speed;
    public float probabilityWeight;
    public Color fishUIColor;
}
