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
        menuProperties.MenuCanvasGroup.ShowAndMakeInteractable();

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
