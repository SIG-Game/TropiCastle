public interface ISavable<TState>
{
    public TState GetSerializableState();

    public void SetPropertiesFromSerializableState(TState serializableState);
}
