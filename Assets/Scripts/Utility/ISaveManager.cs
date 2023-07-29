public interface ISaveManager<TState>
{
    public TState[] GetStates();

    public void CreateObjectsFromStates(TState[] states);
}
