using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    private Animator animator;
    private string destinationSceneName;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TransitionToScene(string sceneName)
    {
        destinationSceneName = sceneName;
        animator.SetTrigger("Start");
    }

    public void ReloadScene()
    {
        TransitionToScene(SceneManager.GetActiveScene().name);
    }

    public void OnTransitionCompletedAnimationEvent()
    {
        Time.timeScale = 1f; // TODO: This statement might not always apply, so it could be moved
        SceneManager.LoadScene(destinationSceneName);
    }
}
