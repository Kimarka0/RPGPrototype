using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class DialogueNodeManager : MonoBehaviour
{
    public static DialogueNodeManager instance;
    
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI speakerNameText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject choiceButtonPrefab;
    [SerializeField] private Transform choiceContainer;
    
    [Header("Animation")]
    [SerializeField] private Animator animator; // ДОБАВЛЕНО
    [SerializeField] private string openParameter = "isOpen"; // ДОБАВЛЕНО
    
    [Header("Events")]
    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueEnded;
    
    private DialogueData currentDialogue;
    private DialogueNode currentNode;
    
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
    
    public void StartDialogue(DialogueData dialogue)
    {
        if(dialogue == null)
        {
            Debug.LogError("DialogueData is null!");
            return;
        }
        
        currentDialogue = dialogue;
        
        // Включаем панель и анимацию
        if(dialoguePanel != null)
            dialoguePanel.SetActive(true);
        
        if(animator != null)
            animator.SetBool(openParameter, true); // АНИМАЦИЯ ОТКРЫТИЯ
        
        OnDialogueStarted?.Invoke();
        ShowNode(dialogue.StartNodeID);
    }
    
    void ShowNode(string nodeID)
    {
        if(string.IsNullOrEmpty(nodeID))
        {
            EndDialogue();
            return;
        }
        
        currentNode = currentDialogue.GetNode(nodeID);
        
        if(currentNode == null) 
        {
            Debug.LogWarning($"Node {nodeID} not found!");
            EndDialogue();
            return;
        }
        
        // Показываем текст
        if(speakerNameText != null)
            speakerNameText.text = currentNode.SpeakerName;
        
        if(dialogueText != null)
            dialogueText.text = currentNode.DialogueText;
        
        // Выполняем действия ноды
        ExecuteActions(currentNode.Actions);
        
        // Очищаем старые кнопки
        if(choiceContainer != null)
        {
            foreach(Transform child in choiceContainer)
                Destroy(child.gameObject);
        }
        
        // Если есть выборы - создаём кнопки
        if(currentNode.HasChoices)
        {
            foreach(var choice in currentNode.Choices)
            {
                CreateChoiceButton(choice);
            }
        }
        // Иначе кнопка "Продолжить"
        else
        {
            CreateContinueButton();
        }
    }
    
    void CreateChoiceButton(DialogueChoice choice)
    {
        if(choiceButtonPrefab == null || choiceContainer == null) return;
        
        GameObject btn = Instantiate(choiceButtonPrefab, choiceContainer);
        TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        if(btnText != null)
            btnText.text = choice.ChoiceText;
        
        Button button = btn.GetComponent<Button>();
        if(button != null)
            button.onClick.AddListener(() => OnChoiceSelected(choice));
    }
    
    void CreateContinueButton()
    {
        if(choiceButtonPrefab == null || choiceContainer == null) return;
        
        GameObject btn = Instantiate(choiceButtonPrefab, choiceContainer);
        TextMeshProUGUI btnText = btn.GetComponentInChildren<TextMeshProUGUI>();
        if(btnText != null)
            btnText.text = "Продолжить";
        
        Button button = btn.GetComponent<Button>();
        if(button != null)
            button.onClick.AddListener(() => ShowNode(currentNode.NextNodeID));
    }
    
    void OnChoiceSelected(DialogueChoice choice)
    {
        ExecuteActions(choice.Actions);
        ShowNode(choice.NextNodeID);
    }
    
    void ExecuteActions(List<DialogueAction> actions)
    {
        if(actions == null) return;
        
        foreach(var action in actions)
        {
            switch(action.ActionType)
            {
                case DialogueActionType.StartQuest:
                    if(QuestSystem.instance != null)
                    {
                        QuestSystem.instance.StartQuest(action.QuestID);
                        Debug.Log($"Квест начат: {action.QuestID}");
                    }
                    break;
                    
                case DialogueActionType.CompleteQuest:
                    if(QuestSystem.instance != null)
                    {
                        QuestSystem.instance.CompleteQuest(action.QuestID);
                        Debug.Log($"Квест завершён: {action.QuestID}");
                    }
                    break;
                    
                case DialogueActionType.UpdateObjective:
                    if(QuestSystem.instance != null)
                    {
                        QuestSystem.instance.UpdateObjective(action.QuestID, action.Parameter, action.Amount);
                        Debug.Log($"Цель обновлена: {action.Parameter}");
                    }
                    break;
                    
                case DialogueActionType.EndDialogue:
                    EndDialogue();
                    break;
            }
        }
    }
    
    public void EndDialogue()
    {
        // Анимация закрытия
        if(animator != null)
            animator.SetBool(openParameter, false); // АНИМАЦИЯ ЗАКРЫТИЯ
        
        // Можно отключить панель с задержкой после анимации
        // Или оставить как есть - аниматор сам скроет
        
        currentDialogue = null;
        currentNode = null;
        
        OnDialogueEnded?.Invoke();
        Debug.Log("Диалог завершён");
    }
    
    public bool IsDialogueActive()
    {
        return currentDialogue != null;
    }
}



