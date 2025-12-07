using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class DialogueTrigger : MonoBehaviour
{
    [Header("Dialogue")]
    [SerializeField] private string dialogueFileName;

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

    private void Start()
    {
        if(interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        if(DialogueManager.instance != null)
        {
            DialogueManager.instance.OnDialogueEnded.AddListener(OnDialogueEnded);
        }

        playerControls = new PlayerControls();

        playerControls.Controls.Interact.performed += OnInteractPerformed;
        playerControls.Enable();

    }

    private void Update()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(interactionUI != null)
        {
            bool shouldShowUI = isPlayerInTrigger && !isDialogueActive && canInteract && !isOnColldown;

            if(shouldShowUI && interactionUI.activeSelf)
            {
                if(uiDelayCoroutine != null)
                {
                    StopCoroutine(uiDelayCoroutine);
                }
                uiDelayCoroutine = StartCoroutine(ShowUIWithDelay());
            }
            else if(!shouldShowUI && interactionUI.activeSelf)
            {
                interactionUI.SetActive(false);
            }
        }
    }
    private IEnumerator ShowUIWithDelay()
    {
        yield return new WaitForSeconds(uiShowDelay);

        if(isPlayerInTrigger && !isDialogueActive && canInteract && !isOnColldown)
        {
            if(interactionUI != null)
            {
                interactionUI.SetActive(true);
            }
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

        DialogueManager.instance.StartDialogueFromFile(dialogueFileName);
        isDialogueActive = true;
        canInteract = false;

        if(interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
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
        if(interactionUI != null)
        {
            interactionUI.SetActive(true);
        }
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
            if(interactionUI != null)
            {
                interactionUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;

            if(interactionUI != null)
            {
                interactionUI.SetActive(false);
            }

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

    public bool IsDialogueActive()
    {
        return isDialogueActive;
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
     if (interactionUI != null)
      {
            interactionUI.SetActive(false);
        }
        
        if (uiDelayCoroutine != null)
         {
             StopCoroutine(uiDelayCoroutine);
         }
     }
 }
