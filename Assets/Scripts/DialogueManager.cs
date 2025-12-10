using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance {get; private set;}
    [SerializeField] private TextMeshProUGUI nameText;
    [Header("UI")]
    [SerializeField] TextMeshProUGUI dialogueText;
    [SerializeField] private Animator animator;
    [SerializeField] private Image speakerPortrait;

    [Header("Settings")]
    [SerializeField] private string dialoguesFolder = "Dialogues";
    [SerializeField] private float typeSpeed = 0.03f;

    public UnityEvent OnDialogueStarted;
    public UnityEvent OnDialogueEnded;
    private Queue<string> sentences = new Queue<string>();
    private bool isDialogueActive = false;
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
    public Dialogue LoadDialogue(string fileName)
    {
        string exePath = Path.Combine(Application.dataPath, "..", dialoguesFolder, fileName + ".json");

        string streamingPath = Path.Combine(Application.streamingAssetsPath, fileName + ".json");

        string filePath = "";

        if (File.Exists(exePath))
        {
            filePath = exePath;
        }
        else if (File.Exists(streamingPath))
        {
            filePath = streamingPath;
        }
        else
        {
            Debug.LogError($"Файл диалога не найден: {fileName}.json");
            Debug.Log($"Искали в: {exePath}");
            Debug.Log($"Искали в: {streamingPath}");
            return null;
        }

        try
        {
            string jsonText = File.ReadAllText(filePath);

            Dialogue dialogue = JsonUtility.FromJson<Dialogue>(jsonText);
            Debug.Log($"Загружен диалог: {dialogue.name}");
            return dialogue;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка загрузки диалога: {e.Message}");
            return null;
        }

    }

    public void StartDialogueFromFile(string fileName)
    {
        Dialogue dialogue = LoadDialogue(fileName);
        if(dialogue != null)
        {
            StartDialogue(dialogue);
        }
    }
    public void StartDialogue(Dialogue dialogue)
    {
        animator.SetBool("isOpen", true);
        nameText.text = dialogue.name;
        sentences.Clear();

        foreach(string sentence in dialogue.sentences)
        {
            if(!string.IsNullOrWhiteSpace(sentence))
            sentences.Enqueue(sentence);

        }

        isDialogueActive = true;
        OnDialogueStarted?.Invoke();
        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if(sentences.Count == 0)
        {
            EndDialogue();
            return;
        }
        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }
    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    private void EndDialogue()
    {
        animator.SetBool("isOpen", false);
        isDialogueActive = false;
        OnDialogueEnded?.Invoke();
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }

    public void SetSpeakerPortrait(Sprite portrait)
    {
        if(speakerPortrait == null) return;
        if(portrait == null) return;
        speakerPortrait.enabled = true;
        speakerPortrait.sprite = portrait;
    }
    public void SetTypeSpeed(float speed)                        
    {
        typeSpeed = Mathf.Max(0.001f, speed);
    }
}
