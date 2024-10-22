using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (AudioManager.Instance == null) return;

        if (scene.buildIndex == 0)
        {
            AudioManager.Instance.PlayMainMenuBGM();
        }
        else if (scene.buildIndex >= 1 && scene.buildIndex <= 4)
        {
            AudioManager.Instance.PlayGameBGM();
        }
        else if (scene.buildIndex >= 5 && scene.buildIndex <= 8)
        {
            AudioManager.Instance.PlayBossBGM();
        }
    }

}
