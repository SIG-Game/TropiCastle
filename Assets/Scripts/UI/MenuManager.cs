using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private MenuProperties currentMenuProperties;

    [Inject] private EventSystemDefaultGameObjectSelector eventSystemDefaultGameObjectSelector;

    public bool MenuOpen => currentMenuProperties != null;

    private void Awake()
    {
        this.InjectDependencies();
    }

    public void ShowMenu(MenuProperties menuProperties)
    {
        Selectable[] childSelectables =
            menuProperties.MenuCanvasGroup.GetComponentsInChildren<Selectable>();

        List<float> childSelectableFadeDurations = new();

        foreach (var childSelectable in childSelectables)
        {
            childSelectableFadeDurations.Add(childSelectable.colors.fadeDuration);

            childSelectable.SetFadeDuration(0f);
        }

        menuProperties.MenuCanvasGroup.ShowAndMakeInteractable();

        for (int i = 0; i < childSelectables.Length; ++i)
        {
            childSelectables[i].SetFadeDuration(childSelectableFadeDurations[i]);
        }

        eventSystemDefaultGameObjectSelector.SetDefaultSelectedGameObject(
            menuProperties.DefaultSelectedGameObject);

        currentMenuProperties = menuProperties;
    }

    public void HideCurrentMenu()
    {
        if (!MenuOpen)
        {
            Debug.LogWarning($"{nameof(HideCurrentMenu)} called with no menu open");
            return;
        }

        currentMenuProperties.MenuCanvasGroup.HideAndMakeNonInteractive();

        eventSystemDefaultGameObjectSelector.SetDefaultSelectedGameObject(null);
        eventSystemDefaultGameObjectSelector.SelectNull();

        currentMenuProperties = null;
    }
}
