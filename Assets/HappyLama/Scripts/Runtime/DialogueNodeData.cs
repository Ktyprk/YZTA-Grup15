using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueNodeData
{
    public string NodeGUID;
    public string DialogueText;
    public Vector2 Position;
    public List<string> Choices = new List<string>();
    public List<int> ReputationChanges = new List<int>(); 
    public bool EntryPoint;
    public bool IsEndNode;
    public bool IsPlayerNode;
    public bool CloseDialogAfterThis = false;

}