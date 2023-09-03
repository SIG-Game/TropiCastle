public interface IElementWithTooltip
{
    public abstract string GetTooltipText();

    public string GetAlternateTooltipText() => string.Empty;
}
