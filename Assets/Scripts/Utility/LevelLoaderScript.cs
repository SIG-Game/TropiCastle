using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoaderScript : MonoBehaviour
{
   public Animator transition;
   public float transitionTime = 1f;

    // Update is called once per frame
    public void makeTransition(string sceneName)
    {
        StartCoroutine(LoadLevel(sceneName));
    }

    IEnumerator LoadLevel(string sceneName)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSecondsRealtime(transitionTime);
        Time.timeScale = 1f; // TODO: This statement might not always apply, so it could be moved
        SceneManager.LoadScene(sceneName);
    }

}
