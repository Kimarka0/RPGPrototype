using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private Transform questListContent;
    [SerializeField] private GameObject questEntryPrefab;
    [SerializeField] private GameObject objectiveTextPrefab;
    [SerializeField] private int testQuestAmount;
    [SerializeField] private Quest testQuest;
    private List<QuestProgress> testQuests = new();

    private void Start()
    {
        for(int i = 0; i < testQuestAmount; i++)
        {
            testQuests.Add(new QuestProgress(testQuest));
        }
        UpdateQuestUI();
    }

    private void UpdateQuestUI()
    {
        foreach(Transform child in questListContent)
        {
            Destroy(child.gameObject);
        }

        foreach(var quest in testQuests)
        {
            GameObject entry = Instantiate(questEntryPrefab, questListContent);
            TextMeshProUGUI questNameText = entry.transform.Find("QuestText").GetComponent<TextMeshProUGUI>();
            Transform objectiveList = entry.transform.Find("ObjectiveList");

            questNameText.text = quest.quest.name;

            foreach(var objective in quest.objectives)
            {
                GameObject objTextGO = Instantiate(objectiveTextPrefab, objectiveList);
                TextMeshProUGUI objText = objTextGO.GetComponent<TextMeshProUGUI>();
                objText.text = $"{objective.description} {objective.currentAmount}/{objective.requiredAmount}";
            }
        }
    }
}
