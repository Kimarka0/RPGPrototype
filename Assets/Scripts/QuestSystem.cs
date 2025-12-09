using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestSystem : MonoBehaviour
{
    public static QuestSystem instance;

    [Header("Settings")]
    [SerializeField] private List<Quest> allQuests = new List<Quest>();

    [SerializeField] private UnityEvent<QuestProgress> onQuestStarted;
    [SerializeField] private UnityEvent<QuestProgress> onQuestProgressUpdated;
    [SerializeField] private UnityEvent<QuestProgress> onQuestCompleted;
}
