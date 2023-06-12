using UnityEngine;

[CreateAssetMenu(menuName = "Fish", fileName = "New Fish")]
public class FishScriptableObject : ScriptableObject
{
    public string species;
    public string description;
    public int speed;
    public float probabilityWeight;
    public Color fishUIColor;
}
