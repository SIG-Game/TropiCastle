using UnityEngine;

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
        foreach (var childSelectable in menuProperties.ChildSelectables)
        {
            childSelectable.SetFadeDuration(0f);
        }

        menuProperties.MenuCanvasGroup.ShowAndMakeInteractable();

        for (int i = 0; i < menuProperties.ChildSelectables.Length; ++i)
        {
            menuProperties.ChildSelectables[i].SetFadeDuration(
                menuProperties.ChildSelectableFadeDurations[i]);
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
