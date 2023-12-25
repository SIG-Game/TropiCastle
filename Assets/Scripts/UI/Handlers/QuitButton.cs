using UnityEngine;

public class QuitButton : MonoBehaviour
{
    public void QuitButton_OnClick()
    {
        Application.Quit();

#if UNITY_EDITOR
        Debug.Log("Application.Quit() called");
#endif
    }
}
