using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI goldText;
    public static UIController Instance { get; private set; }

    public GameObject victoryPanel;
    public GameObject DefeatPanel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        DeactivatePanels();

        if (goldText == null)
        {
            goldText = GameObject.Find("GoldText").GetComponent<TextMeshProUGUI>();
        }
        UpdateGoldUI(PlayerController.Instance.gold);
    }
    public void UpdateGoldUI(int goldAmount)
    {
        if (goldText != null)
        {
            goldText.text = "Koin : " + goldAmount;
        }
    }
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
        if (Instance != null)
        {
            ReassignPanels();
            DeactivatePanels();
        }
    }

    public void ShowVictoryPanel()
    {
        if (victoryPanel != null)
        {
            GameManager.Instance.ResetGame();
            victoryPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void Resume()
    {
        victoryPanel.SetActive(false);
        Time.timeScale = 1;
    }

    public void ShowDefeatPanel()
    {
        if (DefeatPanel != null)
        {
            DefeatPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void MainMenu()
    {
        if (gameController.Instance != null)
        {
            gameController.Instance.ResetGame();
        }
        SceneManager.LoadSceneAsync(0);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    private void DeactivatePanels()
    {
        if (victoryPanel != null)
        {
            victoryPanel.SetActive(false);
        }
        if (DefeatPanel != null)
        {
            DefeatPanel.SetActive(false);
        }
    }
    private void ReassignPanels()
    {
        // Add logic to reassign panel references if they are missing
        // For example, you can find the panels by name in the new scene
        if (victoryPanel == null)
        {
            victoryPanel = GameObject.Find("VictoryPanel");
        }
        if (DefeatPanel == null)
        {
            DefeatPanel = GameObject.Find("DefeatPanel");
        }
    }
}
