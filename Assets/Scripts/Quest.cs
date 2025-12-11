using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Quests")]
public class Quest : ScriptableObject
{
    [Header("Information")]
    [SerializeField] private string questName;
    [SerializeField] private string description;
    [Header("Settings")]
    [SerializeField] private string questID;
    [SerializeField] private List<Quest> requiredQuests = new List<Quest>();
    [Header("Objectives")]
    [SerializeField] private List<QuestObjective> objectives = new List<QuestObjective>();

    public string QuestName => questName;
    public string Description => description;
    public string QuestID => questID;
    public List<Quest> RequiredQuests => requiredQuests;
    public List<QuestObjective> Objectives => objectives;

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(questID) && !string.IsNullOrWhiteSpace(questName))
        {
            questID = questName + Guid.NewGuid().ToString();
        }
#endif
    }
    public bool IsAvailable(List<string> completedQuestsIDs)
    {
        foreach(var reqQuest in requiredQuests)
        {
            if(!completedQuestsIDs.Contains(reqQuest.questID))
            return false;
        }
        return true;
    }
}
[System.Serializable]
    public class QuestObjective
    {
        [Header("Basic Information")]
        [SerializeField] private string objectiveID;
        [SerializeField] private string description;
        [SerializeField] private ObjectiveType type;
        [SerializeField] private int requiredAmount;
        [SerializeField] private int currentAmount;

        [Header("Enemy objectives")]
        [SerializeField] private string requiredEnemyID;

        [Header("Dialogue objectives")]
        [SerializeField] private string requiredDialogueID;
        [SerializeField] private string requiredDialogueNodeID;
        [Header("Visual Information")]
        [SerializeField] private Sprite objectiveIcon;

        public string ObjectiveID => objectiveID;
        public string Description => description;
        public ObjectiveType Type => type;
        public int RequiredAmount => requiredAmount;
        public int CurrentAmount => currentAmount;
        public string RequiredDialogueID => requiredDialogueID;
        public string RequiredDialogueNodeID => requiredDialogueNodeID;
        public string RequiredEnemyID => requiredEnemyID;
        public Sprite ObjectiveIcon => objectiveIcon;

        public QuestObjective(
            string objectiveID,
            string description,
            ObjectiveType type,
            int requiredAmount,
            int currentAmount = 0,
            string requiredDialogueID = null,
            string requiredDialogueNodeID = null,
            string requiredEnemyID = null,
            Sprite objectiveIcon = null)
    {
        this.objectiveID = objectiveID;
        this.description = description;
        this.type = type;
        this.requiredAmount = requiredAmount;
        this.currentAmount = currentAmount;
        this.requiredDialogueID = requiredDialogueID;
        this.requiredDialogueNodeID = requiredDialogueNodeID;
        this.requiredEnemyID = requiredEnemyID;
        this.objectiveIcon = objectiveIcon;
    }
    public QuestObjective() {}
        public bool IsCompleted => currentAmount >= requiredAmount;

        public void UpdateProgress( int amount = 1)
        {
            currentAmount = Mathf.Clamp(currentAmount + amount, 0, requiredAmount);
        }
    }

    public enum ObjectiveType { CollectItem, DefeatEnemy, ReachLocation, TalkToNpc, Custom}

    [System.Serializable]
    public class QuestProgress
    {
    [SerializeField] private Quest quest;
    [SerializeField] private QuestState state = QuestState.NotStarted;
    [SerializeField] private List<QuestObjective> objectives = new List<QuestObjective>();
    [SerializeField] private DateTime startTime;
    [SerializeField] private DateTime? completionTime;

    public Quest Quest => quest;
    public QuestState State => state;
    public List<QuestObjective> Objectives => objectives;
    public DateTime StartTime => startTime;
    public DateTime? CompletionTime => completionTime;

    public QuestProgress(Quest quest)
    {
        this.quest = quest;
        this.state = QuestState.InProgress;
        this.startTime = DateTime.Now;
        objectives = new List<QuestObjective>();

        foreach(var objective in quest.Objectives)
         {
            objectives.Add(new QuestObjective(
                objectiveID: objective.ObjectiveID,
                description: objective.Description,
                type: objective.Type,
                requiredAmount: objective.RequiredAmount,
                currentAmount: objective.CurrentAmount,
                requiredDialogueID: objective.RequiredDialogueID,
                requiredDialogueNodeID: objective.RequiredDialogueNodeID,
                requiredEnemyID: objective.RequiredEnemyID,
                objectiveIcon: objective.ObjectiveIcon


            ));
        }
    }

    public bool IsCompleted
    {
        get
        {
            if(state == QuestState.Completed) return true;

            foreach(var objective in objectives)
            {
                if(!objective.IsCompleted) return false;
            }
            return true;
        }
    }

    public bool UpdateObjective(string objectiveID, int amount = 1)
    {
        var objective = objectives.Find(o => o.ObjectiveID == objectiveID);
        if(objective != null && !objective.IsCompleted)
        {
            objective.UpdateProgress(amount);

            if (IsCompleted)
            {
                state = QuestState.Completed;
                completionTime = DateTime.Now;
            }
            return true;
        }
        return false;
    }

    public bool UpdateObjectByType(ObjectiveType type, int amount = -1)
    {
        bool updated = false;
        foreach(var objective in objectives)
        {
            if(objective.Type == type && !objective.IsCompleted)
            {
                objective.UpdateProgress(amount);
                updated = true;
            }
        }

        if (IsCompleted)
        {
            state = QuestState.Completed;
            completionTime = DateTime.Now;
        }
        return updated;
    }
    
    
    }

    public enum QuestState
{
    NotStarted,
    InProgress,
    Completed,
    Failed
}
