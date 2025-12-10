using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem instance;

    [Header("Settings")]
    [SerializeField] private List<Quest> allQuests = new List<Quest>();

    [Header("Events")]
    public UnityEvent<QuestProgress> onQuestStarted; // УБРАЛ [SerializeField], сделал public
    public UnityEvent<QuestProgress> onQuestProgressUpdated; // УБРАЛ [SerializeField]
    public UnityEvent<QuestProgress> onQuestCompleted; // УБРАЛ [SerializeField]

    private readonly Dictionary<string, QuestProgress> activeQuests = new();
    private readonly List<string> completedQuestsIDs = new();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartQuest(string questID)
    {
        if(string.IsNullOrWhiteSpace(questID)) return;
        if(activeQuests.ContainsKey(questID)) return;

        Quest quest = allQuests.Find(q => q != null && q.QuestID == questID);
        
        if(quest == null)
        {
            Debug.LogError($"Quest not found: {questID}");
            return;
        }

        if(!quest.IsAvailable(completedQuestsIDs)) return;

        QuestProgress progress = new QuestProgress(quest);
        activeQuests.Add(questID, progress);
        onQuestStarted?.Invoke(progress);
        
        Debug.Log($"Квест начат: {quest.QuestName}");
    }

    public void UpdateObjective(string questID, string objectiveID, int amount = 1)
    {
        if (!activeQuests.TryGetValue(questID, out QuestProgress progress)) return;

        bool updated = progress.UpdateObjective(objectiveID, amount);
        if(!updated) return;

        onQuestProgressUpdated?.Invoke(progress);
        
        if(progress.IsCompleted) CompleteQuest(questID);
    }

    public void CompleteQuest(string questID)
    {
        if(!activeQuests.TryGetValue(questID, out QuestProgress progress)) return;

        if(!completedQuestsIDs.Contains(questID)) 
            completedQuestsIDs.Add(questID);
        
        activeQuests.Remove(questID);
        onQuestCompleted?.Invoke(progress);
        
        Debug.Log($"Квест завершён: {progress.Quest.QuestName}");
    }

    public bool IsQuestCompleted(string questID)
    {
        return completedQuestsIDs.Contains(questID);
    }

    public bool IsQuestActive(string questID)
    {
        return activeQuests.ContainsKey(questID);
    }

    public QuestProgress GetActiveQuest(string questID)
    {
        activeQuests.TryGetValue(questID, out QuestProgress progress);
        return progress;
    }

    public List<QuestProgress> GetAllActiveQuests()
    {
        return new List<QuestProgress>(activeQuests.Values);
    }
}
