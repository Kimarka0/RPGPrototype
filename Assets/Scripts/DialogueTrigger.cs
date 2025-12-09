using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private string dialogueFileName;
    [SerializeField] private bool checkQuestRequirements;
    [SerializeField] private string requiredQuestID;
    [SerializeField] private QuestState requiredQuestState = QuestState.Completed;
    [SerializeField] private Sprite npcPortrait;

    [Header("UI")]
    [SerializeField] private GameObject interactionUI;
    [SerializeField] private float uiShowDelay = 1f;

    [Header("Settings")]
    [SerializeField] private bool canRetrigger = true;
    [SerializeField] private float retriggerCooldown = 3f;

    private PlayerControls playerControls;
    private Coroutine uiDelayCoroutine;

    private bool isPlayerInTrigger;
    private bool canInteract;
    private bool isDialogueActive;
    private bool isOnColldown;

    public bool IsPlayerInTrigger => isPlayerInTrigger;
    public bool CanInteract => canInteract;
    public bool IsDialogueActive => isDialogueActive;
    public bool IsOnCooldown => IsOnCooldown;
    public Sprite NpcPortrait => npcPortrait;

    private void Start()
    {
        InitializeUI();
        InitializeInput();
        SubscribeDialogueEvents();
    }

    private void InitializeUI()
    {
        if(interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
    private void InitializeInput()
    {
        playerControls = new PlayerControls();
        playerControls.Controls.Interact.performed += OnInteractPerformed;
        playerControls.Enable();
    }
    private void SubscribeDialogueEvents()
    {
        if(DialogueManager.instance != null)
        {
            DialogueManager.instance.OnDialogueEnded.AddListener(OnDialogueEnded);
            DialogueManager.instance.OnDialogueEnded.AddListener(OnDialogueEnded);
        }
    }

    private void Update()
    {
        UpdateInteractionUI();
    }

    private void UpdateInteractionUI()
    {
        if(interactionUI != null)
        {
            bool shouldShowUI = isPlayerInTrigger && !isDialogueActive && canInteract && !isOnColldown;

            if(shouldShowUI && interactionUI.activeSelf)
            {
                ShowInteractionUI();
            }
            else if(!shouldShowUI && interactionUI.activeSelf)
            {
                HideInteractionUI();
            }
        }
    }

    private bool CheckQuestRequirements()
    {
        if(!checkQuestRequirements) return true;
        if(QuestSystem.instance == null) return false;
        switch (requiredQuestState)
        {
            case QuestState.Completed:
                return QuestSystem.instance.IsQuestCompleted(requiredQuestID);
            case QuestState.InProgress:
                return QuestSystem.instance.IsQuestActive(requiredQuestID);
            case QuestState.NotStarted:
                return !QuestSystem.instance.IsQuestActive(requiredQuestID) && !QuestSystem.instance.IsQuestCompleted(requiredQuestID);
        default:
            return true;
        } 
    }

    private void ShowInteractionUI()
    {
        if(uiDelayCoroutine != null)
            {
                StopCoroutine(uiDelayCoroutine);
            }
            uiDelayCoroutine = StartCoroutine(ShowUIWithDelay());
    }

    private void HideInteractionUI()
    {
        if(interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }
    private IEnumerator ShowUIWithDelay()
    {
        yield return new WaitForSeconds(uiShowDelay);

        if(isPlayerInTrigger && !isDialogueActive && canInteract && !isOnColldown)
        {
           ShowInteractionUI();
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext context)
    {
        if(isPlayerInTrigger && !isDialogueActive && canInteract && !isOnColldown)
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (DialogueManager.instance.IsDialogueActive())
        {
            return;
        }

        DialogueManager.instance.SetSpeakerPortrait(npcPortrait);
        DialogueManager.instance.StartDialogueFromFile(dialogueFileName);
        isDialogueActive = true;
        canInteract = false;

        HideInteractionUI();
    }
    private void OnDialogueEnded()
    {
        isDialogueActive = false;

        if(canRetrigger && isPlayerInTrigger)
        {
            StartCoroutine(StartCooldown());
        }
        else
        {
            canInteract = true;
        }
    }

    private IEnumerator StartCooldown()
    {
        isOnColldown = true;
        yield return new WaitForSeconds(retriggerCooldown);

        isOnColldown = false;
        canInteract = true;
        HideInteractionUI();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            if(!isDialogueActive && !isOnColldown)
            {
                canInteract = true;
            }
            ShowInteractionUI();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;

            HideInteractionUI();

            if (isOnColldown)
            {
                StopCoroutine(StartCooldown());
                isOnColldown = false;
                canInteract = true;
            }
        }
    }

    public void TriggerDialogueManually()
    {
        if(!isDialogueActive && canInteract)
        {
            StartDialogue();
        }
    }

    public void SetCanInteract(bool value)
    {
        canInteract = value;
    }
    private void OnEnable()
    {
        playerControls?.Enable();
    }
    
    private void OnDisable()
   {
         playerControls?.Disable();
        
        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.OnDialogueEnded.RemoveListener(OnDialogueEnded);
        }
        HideInteractionUI();
        
        if (uiDelayCoroutine != null)
        {
             StopCoroutine(uiDelayCoroutine);
        }
     }
 }
