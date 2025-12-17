using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Node Dialogue")]
public class DialogueData : ScriptableObject
{
    [SerializeField] private string dialogueID;
    [SerializeField] private string dialogueName;
    [SerializeField] private string startNodeID = "start";
    [SerializeField] private List<DialogueNode> nodes = new List<DialogueNode>();

    public string DialogueID => dialogueID;
    public string DialogueName => dialogueName;
    public string StartNodeID => startNodeID;
    public List<DialogueNode> Nodes => nodes;

    public DialogueNode GetNode(string id)
    {
        return nodes.Find(n => n.NodeID == id);
    }
}

