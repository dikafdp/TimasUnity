using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mainMenu : MonoBehaviour
{
    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMainMenuBGM();
        }
    }

    public void PlayGame()
    {
        if (gameController.Instance != null)
        {
            gameController.Instance.ResetGame();
        }
        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.ResetInventory(); // Reset inventory when starting a new game
        }
        SceneManager.LoadSceneAsync(2);
    }
    public void StoryScene()
    {
        SceneManager.LoadSceneAsync(9);
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
