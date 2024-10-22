using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Free, Dialog }
public class gameController : MonoBehaviour
{
    public static gameController Instance { get; private set; }
    public Vector3 PlayerPosition;
    public Vector3 DefaultPlayerPosition;

    GameState state;

    [SerializeField] PlayerController playerController;

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
        }
    }
    public void dialogShow()
    {
        if (dialogManager.Instance != null)
        {
            dialogManager.Instance.OnShowDialog += () =>
            {
                state = GameState.Dialog;
            };
            dialogManager.Instance.OnHideDialog += () =>
            {
                if (state == GameState.Dialog)
                    state = GameState.Free;
            };
        }
    }
    public void dialogState()
    {
        if (state == GameState.Free)
        {

            if (playerController != null)
            {
                playerController.handleUpdate();
            }
        }
        else if (state == GameState.Dialog)
        {
            if (dialogManager.Instance != null)
            {
                dialogManager.Instance.handleUpdate();
            }
        }
    }

    public void Start()
    {
        DefaultPlayerPosition = new Vector3(0, 0, 0);
        PlayerPosition = DefaultPlayerPosition;

        dialogShow();

        if (playerController != null)
        {
            PlayerPosition = playerController.transform.position;
        }
        else
        {
            Debug.LogError("PlayerController is not assigned.");
        }

    }

    public void Update()
    {
        dialogState();
    }
    public void ResetGame()
    {
        // Reset the player position to the default
        PlayerPosition = DefaultPlayerPosition;

        // Reset any other game state here
        if (playerController != null)
        {
            playerController.ResetPosition();
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
        if (playerController != null)
        {
            playerController.transform.position = PlayerPosition;
        }
        else
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController != null)
            {
                playerController.transform.position = PlayerPosition;
            }
            else
            {
                Debug.LogError("PlayerController not found in the new scene.");
            }
        }
    }

}
