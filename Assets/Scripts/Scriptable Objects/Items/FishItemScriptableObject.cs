using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Item/Fish Item")]
public class FishItemScriptableObject : ItemScriptableObject
{
    public string description;
    public int speed;
    public float probabilityWeight;
}
