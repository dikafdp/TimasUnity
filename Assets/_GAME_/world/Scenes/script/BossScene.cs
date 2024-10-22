using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossScene : MonoBehaviour
{
    public int sceneBuildIndex;
    public string requiredItemID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {

            if (PlayerPrefs.GetInt(requiredItemID, 0) == 1)
            {

                if (gameController.Instance != null)
                {
                    gameController.Instance.PlayerPosition = other.transform.position;
                }

                SceneManager.LoadScene(sceneBuildIndex, LoadSceneMode.Single);
            }
            else
            {
                Debug.Log("Item yang diperlukan tidak ada di inventory!");
            }
        }
    }
}
