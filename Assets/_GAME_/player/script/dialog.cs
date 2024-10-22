using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class dialog
{
    [SerializeField] List<string> lines;
    [SerializeField] string npcName;

    public List<string> Lines
    {
        get { return lines; }
    }
    public string NpcName
    {
        get { return npcName; }
    }
}
