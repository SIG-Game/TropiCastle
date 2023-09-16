public interface ISavablePrefabInstance
{
    public SavablePrefabInstanceState GetSavablePrefabInstanceState();

    public void SetPropertiesFromSavablePrefabInstanceState(
        SavablePrefabInstanceState savableState);
}
