using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class dialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] Text npcName;
    [SerializeField] int letterPerSecond;
    [SerializeField] Button skipButton;

    public event Action OnShowDialog;
    public event Action OnHideDialog;

    public static dialogManager Instance { get; private set; }

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

    dialog dialog;
    int currentLine = 0;
    bool isTyping;

    private void Start()
    {
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipDialog);
        }
    }

    public IEnumerator showDialog(dialog dialog)
    {
        yield return new WaitForEndOfFrame();
        OnShowDialog?.Invoke();

        this.dialog = dialog;

        if (dialogBox != null)
        {
            dialogBox.SetActive(true);
        }
        else
        {
            Debug.LogError("dialogBox is not assigned.");
        }
        if (npcName != null)
        {
            npcName.text = dialog.NpcName;
        }
        else
        {
            Debug.LogError("npcName is not assigned.");
        }

        DisplayNextLine();
    }

    private void DisplayNextLine()
    {
        if (currentLine < dialog.Lines.Count)
        {
            StartCoroutine(typeDialog(dialog.Lines[currentLine]));
        }
        else
        {
            EndDialog();
        }
    }

    private void EndDialog()
    {
        if (dialogBox != null)
        {
            dialogBox.SetActive(false);
        }

        currentLine = 0; // Reset current line after ending the dialog
        OnHideDialog?.Invoke();
    }

    public void handleUpdate()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetMouseButtonDown(0)) && !isTyping)
        {
            ++currentLine;
            DisplayNextLine();
        }
    }

    public IEnumerator typeDialog(string line)
    {
        if (dialogText == null)
        {
            Debug.LogError("dialogText is not assigned.");
            yield break;
        }

        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);

            if (!isTyping) // Check if skip button was pressed
            {
                dialogText.text = line;
                break;
            }
        }
        isTyping = false;
    }

    public void SkipDialog()
    {
        if (isTyping)
        {
            StopAllCoroutines(); // Stop typing coroutine
            dialogText.text = dialog.Lines[currentLine]; // Display the full current line
            isTyping = false;
        }

        currentLine = dialog.Lines.Count; // Set current line to end
        EndDialog(); // End the dialog immediately
    }
}
