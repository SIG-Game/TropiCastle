public class Tooltip
{
    public string Text { get; private set; }
    public int Priority { get; private set; }

    public Tooltip(string text, int priority)
    {
        Text = text;
        Priority = priority;
    }
}
