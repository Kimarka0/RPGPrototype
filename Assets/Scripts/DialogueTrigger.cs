using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private Dialogue dialogue;
    private bool isTriggered = false;
    
    private void TriggerDialogue()
    {
        if(isTriggered == false)
        {
            DialogueManager.instance.StartDialogue(dialogue);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerDialogue();
            isTriggered = true;
        }
    }
}
