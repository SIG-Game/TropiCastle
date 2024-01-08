using UnityEngine.UI;

public static class SelectableExtensions
{
    public static void SetFadeDuration(this Selectable selectable, float fadeDuration)
    {
        ColorBlock selectableColors = selectable.colors;
        selectableColors.fadeDuration = fadeDuration;
        selectable.colors = selectableColors;
    }
}
