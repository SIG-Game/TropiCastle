using UnityEngine;

public class MenuProperties : MonoBehaviour
{
    [SerializeField] private CanvasGroup menuCanvasGroup;
    [SerializeField] private GameObject defaultSelectedGameObject;

    public CanvasGroup MenuCanvasGroup => menuCanvasGroup;
    public GameObject DefaultSelectedGameObject => defaultSelectedGameObject;

    public bool MenuIsVisible => menuCanvasGroup.alpha == 1f;
}
