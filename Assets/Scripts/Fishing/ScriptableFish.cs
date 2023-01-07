using UnityEngine;

[CreateAssetMenu(menuName = "Fish", fileName = "New Fish")]
public class ScriptableFish : ScriptableObject
{
   public string species;
   
   public string description;
   public Sprite image;
   public int size;
   public int speed;
   
}
