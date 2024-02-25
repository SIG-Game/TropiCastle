using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MenuProperties : MonoBehaviour
{
    [SerializeField] private CanvasGroup menuCanvasGroup;
    [SerializeField] private GameObject defaultSelectedGameObject;

    public Selectable[] ChildSelectables { get; private set; }
    public List<float> ChildSelectableFadeDurations { get; private set; }

    public CanvasGroup MenuCanvasGroup => menuCanvasGroup;
    public GameObject DefaultSelectedGameObject => defaultSelectedGameObject;

    public bool MenuIsVisible => menuCanvasGroup.alpha == 1f;

    private void Awake()
    {
        ChildSelectables = MenuCanvasGroup
            .GetComponentsInChildren<Selectable>();

        ChildSelectableFadeDurations = ChildSelectables
            .Select(x => x.colors.fadeDuration).ToList();
    }
}
