using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class nextScene : MonoBehaviour
{
    public int sceneBuildIndex;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Save player position before changing scene
            if (gameController.Instance != null)
            {
                gameController.Instance.PlayerPosition = other.transform.position;
            }

            SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
        }
    }
}