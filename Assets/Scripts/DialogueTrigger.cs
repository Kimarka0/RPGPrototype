using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue System")]
    [SerializeField] private bool useNodeSystem = true;
    [SerializeField] private DialogueData dialogueData;
    [SerializeField] private string dialogueFileName;
    
    [Header("Quest Requirements")]
    [SerializeField] private bool checkQuestRequirements;
    [SerializeField] private string requiredQuestID;
    [SerializeField] private QuestState requiredQuestState = QuestState.Completed;
    [SerializeField] private Sprite npcPortrait;

    [Header("UI")]
    [SerializeField] private GameObject interactionUI;

    [Header("Settings")]
    [SerializeField] private bool canRetrigger = true;
    [SerializeField] private float retriggerCooldown = 3f;

    private PlayerControls playerControls;
    private bool isPlayerInTrigger;
    private bool isDialogueActive;
    private bool isOnCooldown;

    private void Start()
    {
        if(interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        
        playerControls = new PlayerControls();
        playerControls.Controls.Interact.performed += OnInteractPerformed;
        playerControls.Enable();
        
        SubscribeToEvents();
    }

    private void SubscribeToEvents()
    {
        if(DialogueManager.instance != null)
        {
            DialogueManager.instance.OnDialogueEnded.AddListener(OnDialogueEnded);
        }
        
        if(DialogueNodeManager.instance != null)
        {
            DialogueNodeManager.instance.OnDialogueEnded.AddListener(OnDialogueEnded);
        }
    }

    private void Update()
    {
        // Проверяем статус диалога
        if(useNodeSystem && DialogueNodeManager.instance != null)
        {
            isDialogueActive = DialogueNodeManager.instance.IsDialogueActive();
        }
        else if(!useNodeSystem && DialogueManager.instance != null)
        {
            isDialogueActive = DialogueManager.instance.IsDialogueActive();
        }
        
        // Управляем UI
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(interactionUI == null) return;

        bool shouldShow = isPlayerInTrigger && !isDialogueActive && !isOnCooldown;
        
        if(shouldShow && !interactionUI.activeSelf)
        {
            interactionUI.SetActive(true);
            Debug.Log($"[{gameObject.name}] UI включен");
        }
        else if(!shouldShow && interactionUI.activeSelf)
        {
            interactionUI.SetActive(false);
            Debug.Log($"[{gameObject.name}] UI выключен");
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if(!isPlayerInTrigger || isDialogueActive || isOnCooldown) return;
        
        if(checkQuestRequirements && !CheckQuestRequirements()) return;
        
        StartDialogue();
    }

    private bool CheckQuestRequirements()
    {
        if(QuestSystem.instance == null) return false;
        
        switch (requiredQuestState)
        {
            case QuestState.Completed:
                return QuestSystem.instance.IsQuestCompleted(requiredQuestID);
            case QuestState.InProgress:
                return QuestSystem.instance.IsQuestActive(requiredQuestID);
            case QuestState.NotStarted:
                return !QuestSystem.instance.IsQuestActive(requiredQuestID) && 
                       !QuestSystem.instance.IsQuestCompleted(requiredQuestID);
            default:
                return true;
        }
    }

    private void StartDialogue()
    {
        // Нодовая система
        if(useNodeSystem && dialogueData != null)
        {
            if(DialogueNodeManager.instance == null)
            {
                Debug.LogError("DialogueNodeManager не найден!");
                return;
            }
            
            DialogueNodeManager.instance.StartDialogue(dialogueData);
            isDialogueActive = true;
            
            if(interactionUI != null)
                interactionUI.SetActive(false);
        }
        // Простая система
        else if(!useNodeSystem && !string.IsNullOrEmpty(dialogueFileName))
        {
            if(DialogueManager.instance == null)
            {
                Debug.LogError("DialogueManager не найден!");
                return;
            }
            
            if(DialogueManager.instance.IsDialogueActive()) return;

            DialogueManager.instance.SetSpeakerPortrait(npcPortrait);
            DialogueManager.instance.StartDialogueFromFile(dialogueFileName);
            isDialogueActive = true;
            
            if(interactionUI != null)
                interactionUI.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Диалог не настроен! Назначь DialogueData или DialogueFileName");
        }
    }

    private void OnDialogueEnded()
    {
        isDialogueActive = false;
        
        if(canRetrigger && isPlayerInTrigger)
        {
            StartCoroutine(CooldownCoroutine());
        }
    }

    private IEnumerator CooldownCoroutine()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(retriggerCooldown);
        isOnCooldown = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            Debug.Log($"[{gameObject.name}] Игрок вошёл");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            Debug.Log($"[{gameObject.name}] Игрок вышел");
            
            if(interactionUI != null)
                interactionUI.SetActive(false);
        }
    }

    private void OnEnable()
    {
        playerControls?.Enable();
    }
    
    private void OnDisable()
    {
        playerControls?.Disable();
        
        if(DialogueManager.instance != null)
        {
            DialogueManager.instance.OnDialogueEnded.RemoveListener(OnDialogueEnded);
        }
        
        if(DialogueNodeManager.instance != null)
        {
            DialogueNodeManager.instance.OnDialogueEnded.RemoveListener(OnDialogueEnded);
        }
        
        if(interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
}

