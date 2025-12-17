using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueNode
{
    [SerializeField] private string nodeID;
    [SerializeField] private string speakerName;
    [SerializeField] private string dialogueText;
    [SerializeField] private List<DialogueChoice> choices = new List<DialogueChoice>();
    [SerializeField] private List<DialogueAction> actions = new List<DialogueAction>();
    [SerializeField] private string nextNodeID;

    public string NodeID => nodeID;
    public string SpeakerName => speakerName;
    public string DialogueText => dialogueText;
    public List<DialogueChoice> Choices => choices;
    public List<DialogueAction> Actions => actions;
    public string NextNodeID => nextNodeID;
    public bool HasChoices => choices != null && choices.Count > 0;
}

[System.Serializable]
public class DialogueChoice
{
    [SerializeField] private string choiceText;
    [SerializeField] private string nextNodeID;
    [SerializeField] private List<DialogueAction> actions = new List<DialogueAction>();

    public string ChoiceText => choiceText;
    public string NextNodeID => nextNodeID;
    public List<DialogueAction> Actions => actions;
}

[System.Serializable]
public class DialogueAction
{
    [SerializeField] private DialogueActionType actionType;
    [SerializeField] private string questID;
    [SerializeField] private string itemID;
    [SerializeField] private int amount = 1;
    [SerializeField] private string parameter;

    public DialogueActionType ActionType => actionType;
    public string QuestID => questID;
    public string ItemID => itemID;
    public int Amount => amount;
    public string Parameter => parameter;
}

public enum DialogueActionType
{
    None,
    StartQuest,
    CompleteQuest,
    UpdateObjective,
    GiveItem,
    TakeItem,
    AddMoney,
    TriggerEvent,
    EndDialogue
}