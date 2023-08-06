using System.IO;
using UnityEngine;

public class DeleteSaveDataButton : MonoBehaviour
{
    public void DeleteSaveDataButton_OnClick()
    {
        File.Delete(SaveController.GetSaveDataFilePath());
    }
}
