using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private Transform questListContent;
    [SerializeField] private GameObject questEntryPrefab;
    [SerializeField] private GameObject objectiveTextPrefab;

    private void Start()
    {
        SubscribeToQuestEvents();
        UpdateQuestUI();
    }

    private void SubscribeToQuestEvents()
    {
        if(QuestSystem.instance != null)
        {
            QuestSystem.instance.onQuestStarted.AddListener(OnQuestChanged);
            QuestSystem.instance.onQuestProgressUpdated.AddListener(OnQuestChanged);
            QuestSystem.instance.onQuestCompleted.AddListener(OnQuestChanged);
        }
        else
        {
            Debug.LogError("QuestSystem не найден в сцене!");
        }
    }

    private void OnQuestChanged(QuestProgress questProgress)
    {
        // Обновляем UI когда что-то меняется
        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        // Очищаем старые записи
        foreach (Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        // Получаем активные квесты из QuestSystem
        if(QuestSystem.instance == null) return;
        
        List<QuestProgress> activeQuests = QuestSystem.instance.GetAllActiveQuests();

        // Показываем каждый квест
        foreach (QuestProgress questProgress in activeQuests)
        {
            CreateQuestEntry(questProgress);
        }
    }

    private void CreateQuestEntry(QuestProgress questProgress)
    {
        GameObject entry = Instantiate(questEntryPrefab, questListContent);
        
        // Находим текст квеста
        TextMeshProUGUI questNameText = entry.transform.Find("QuestText")?.GetComponent<TextMeshProUGUI>();
        if(questNameText != null)
        {
            questNameText.text = questProgress.Quest.QuestName;
        }
        
        // Находим список целей
        Transform objectiveList = entry.transform.Find("ObjectiveList");
        if(objectiveList == null) return;

        // Создаём цели
        foreach (QuestObjective objective in questProgress.Objectives)
        {
            GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
            TextMeshProUGUI objText = objTextGO.GetComponent<TextMeshProUGUI>();

            
            if(objText != null)
            {
                objText.text = $"{objective.Description} {objective.CurrentAmount}/{objective.RequiredAmount}";
                
                // Зачеркиваем если выполнено
                if(objective.IsCompleted)
                {
                    objText.text = $"<s>{objText.text}</s>";
                    objText.color = Color.gray;
                }
            }
        }
        ForCanvas();
    }
    
    public void ForCanvas()
    {
        Canvas.ForceUpdateCanvases();
        LayoutRebuilder.ForceRebuildLayoutImmediate(questListContent as RectTransform);
    }
    private void OnDestroy()
    {
        // Отписываемся от событий
        if(QuestSystem.instance != null)
        {
            QuestSystem.instance.onQuestStarted.RemoveListener(OnQuestChanged);
            QuestSystem.instance.onQuestProgressUpdated.RemoveListener(OnQuestChanged);
            QuestSystem.instance.onQuestCompleted.RemoveListener(OnQuestChanged);
        }
    }
}
