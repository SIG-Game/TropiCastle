public interface ISavableNongeneric
{
    public SavableState GetSavableState();

    public void SetPropertiesFromSavableState(SavableState savableState);

    public string GetSaveGuid();
}
