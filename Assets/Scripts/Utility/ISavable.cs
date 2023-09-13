public interface ISavable
{
    public SavableState GetSavableState();

    public void SetPropertiesFromSavableState(SavableState savableState);

    public string GetSaveGuid() => string.Empty;
}
