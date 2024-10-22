using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int maxHealth = 100;
    public int currentHealth;

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
    public void ResetGame()
    {
        // Reset item data
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        // Reset player health or other game states as needed
        currentHealth = maxHealth;
    }

    private void Start()
    {
        currentHealth = maxHealth;  
    }
    private void Update()
    {
        
    }

}
