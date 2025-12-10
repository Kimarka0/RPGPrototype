// ============================================================================
// MINIMAL DIALOGUE SYSTEM - САМЫЙ ПРОСТОЙ ВАРИАНТ
// ============================================================================
// БЕЗ квестов, БЕЗ лямбд, БЕЗ сложностей
// Только: диалог → выборы → конец

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// ============================================================================
// ШАГ 1: ПРОСТАЯ СТРУКТУРА ДИАЛОГА
// ============================================================================
/// <summary>
/// Одна фраза в диалоге.
/// Это самая простая версия - только текст и следующий узел.
/// </summary>
[System.Serializable]
public class SimpleDialogueNode
{
    [SerializeField] public string nodeID;           // "0", "1", "2" и т.д.
    [SerializeField] public string speakerName;      // "NPC", "Player", "Narration"
    [SerializeField] public string dialogueText;     // Что говорится
    [SerializeField] public string nextNodeID = "";  // К какому узлу переходить дальше
    [SerializeField] public List<SimpleDialogueChoice> choices = new List<SimpleDialogueChoice>(); // Выборы
}

/// <summary>
/// Выбор ответа игрока.
/// </summary>
[System.Serializable]
public class SimpleDialogueChoice
{
    [SerializeField] public string choiceText;       // Текст кнопки ("Да", "Нет" и т.д.)
    [SerializeField] public string nextNodeID;       // Куда переходить при выборе
}

/// <summary>
/// Полный диалог - набор узлов.
/// </summary>
[System.Serializable]
public class SimpleDialogueData
{
    [SerializeField] public string dialogueID;       // "npc_greeting"
    [SerializeField] public string dialogueName;     // "Приветствие торговца"
    [SerializeField] public string startNodeID;      // С какого узла начинать ("0")
    [SerializeField] public List<SimpleDialogueNode> nodes;  // Все узлы
}

// ============================================================================
// ШАГ 2: МЕНЕДЖЕР ДИАЛОГОВ (БЕЗ СИНГЛТОНА, БЕЗ ЛЯМБД)
// ============================================================================
/// <summary>
/// Самый простой менеджер диалогов.
/// Показывает текст и кнопки, больше ничего.
/// </summary>
public class SimpleDialogueManager : MonoBehaviour
{
    // Ссылки на UI элементы
    [SerializeField] private TextMeshProUGUI nameText;        // "NPC"
    [SerializeField] private TextMeshProUGUI dialogueText;    // Основной текст
    [SerializeField] private Transform choicesContainer;      // Где создавать кнопки
    [SerializeField] private Button choiceButtonPrefab;       // Шаблон кнопки
    [SerializeField] private GameObject dialoguePanel;        // Панель с диалогом

    // Текущее состояние
    private SimpleDialogueData currentDialogue;    // Какой диалог сейчас идёт
    private SimpleDialogueNode currentNode;        // Какой узел сейчас показывается
    private bool isDialogueActive = false;         // Идёт ли диалог

    // ========================================================================
    // ЗАГРУЗКА ДИАЛОГА
    // ========================================================================
    /// <summary>
    /// Загрузить диалог из JSON-файла.
    /// ПРИМЕР: LoadDialogue("npc_greeting");
    /// 
    /// Файл должен быть в: Assets/Resources/Dialogues/npc_greeting.json
    /// </summary>
    public void LoadDialogue(string fileName)
    {
        // Ищем файл в Resources/Dialogues/
        TextAsset jsonFile = Resources.Load<TextAsset>($"Dialogues/{fileName}");

        // Проверка: файл найден?
        if (jsonFile == null)
        {
            Debug.LogError($"Файл не найден: Dialogues/{fileName}");
            return;
        }

        // Парсим JSON в объект
        currentDialogue = JsonUtility.FromJson<SimpleDialogueData>(jsonFile.text);

        // Проверка: диалог загружен правильно?
        if (currentDialogue == null || currentDialogue.nodes == null)
        {
            Debug.LogError($"Диалог повреждён: {fileName}");
            return;
        }

        Debug.Log($"Диалог загружен: {currentDialogue.dialogueName}");

        // Показываем панель
        dialoguePanel.SetActive(true);
        isDialogueActive = true;

        // Показываем первый узел
        ShowNode(currentDialogue.startNodeID);
    }

    // ========================================================================
    // ПОКАЗ УЗЛА
    // ========================================================================
    /// <summary>
    /// Показать узел с заданным ID.
    /// 
    /// ЦЕЛЬ: найти этот узел в currentDialogue и вывести его содержимое.
    /// </summary>
    private void ShowNode(string nodeID)
    {
        // Ищем узел в списке
        currentNode = currentDialogue.nodes.Find(n => n.nodeID == nodeID);

        // Проверка: узел найден?
        if (currentNode == null)
        {
            Debug.LogError($"Узел не найден: {nodeID}");
            EndDialogue();
            return;
        }

        // ====== ПОКАЗЫВАЕМ ТЕКСТ ======
        nameText.text = currentNode.speakerName;
        dialogueText.text = currentNode.dialogueText;

        // ====== ПОКАЗЫВАЕМ КНОПКИ ВЫБОРОВ ======
        // Если есть выборы - создаём кнопки
        if (currentNode.choices != null && currentNode.choices.Count > 0)
        {
            ShowChoices(currentNode.choices);
        }
        // Если нет выборов - автоматически переходим на следующий узел
        else if (!string.IsNullOrEmpty(currentNode.nextNodeID))
        {
            // Используем Invoke чтобы сделать задержку перед переходом
            Invoke("GoToNextNode", 2f); // Ждём 2 секунды
        }
        // Если нет выборов и нет следующего узла - конец
        else
        {
            Invoke("EndDialogue", 2f);
        }
    }

    /// <summary>
    /// Перейти на следующий узел (вспомогательный метод для Invoke).
    /// </summary>
    private void GoToNextNode()
    {
        if (isDialogueActive)
        {
            ShowNode(currentNode.nextNodeID);
        }
    }

    // ========================================================================
    // ПОКАЗ КНОПОК ВЫБОРА
    // ========================================================================
    /// <summary>
    /// Создать кнопки для каждого выбора.
    /// </summary>
    private void ShowChoices(List<SimpleDialogueChoice> choices)
    {
        // Удаляем старые кнопки
        foreach (Transform child in choicesContainer)
        {
            Destroy(child.gameObject);
        }

        // Создаём новую кнопку для каждого выбора
        for (int i = 0; i < choices.Count; i++)
        {
            // Копируем префаб
            Button newButton = Instantiate(choiceButtonPrefab, choicesContainer);

            // Заполняем текст
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = choices[i].choiceText;

            // ====== БЕЗ ЛЯМБДЫ! Используем обычный метод ======
            // Сохраняем индекс выбора в отдельную переменную
            int choiceIndex = i;
            
            // Добавляем обработчик нажатия
            // Когда нажимают эту кнопку - вызовут OnChoiceClicked с индексом
            newButton.onClick.AddListener(delegate { OnChoiceClicked(choiceIndex); });
        }

        Debug.Log($"Показано {choices.Count} выборов");
    }

    /// <summary>
    /// Вызывается когда игрок нажимает кнопку выбора.
    /// ПАРАМЕТР: choiceIndex - номер выбранного варианта (0, 1, 2 и т.д.)
    /// </summary>
    private void OnChoiceClicked(int choiceIndex)
    {
        // Получаем выбранный вариант
        SimpleDialogueChoice selectedChoice = currentNode.choices[choiceIndex];

        // Переходим на следующий узел
        ShowNode(selectedChoice.nextNodeID);
    }

    // ========================================================================
    // ЗАВЕРШЕНИЕ ДИАЛОГА
    // ========================================================================
    /// <summary>
    /// Завершить диалог и скрыть панель.
    /// </summary>
    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        Debug.Log("Диалог завершён");
    }
}

// ============================================================================
// ШАГ 3: ТРИГГЕР НА NPC (ОЧЕНЬ ПРОСТОЙ)
// ============================================================================
/// <summary>
/// Простой триггер - когда игрок нажимает E рядом с NPC, начинается диалог.
/// </summary>
public class SimpleDialogueTrigger : MonoBehaviour
{
    [SerializeField] private string dialogueFileName = "npc_greeting";  // Имя JSON-файла
    [SerializeField] private SimpleDialogueManager dialogueManager;     // Ссылка на менеджер
    [SerializeField] private GameObject interactionUI;                  // "Press E" панель

    private bool isPlayerNear = false;  // Находится ли игрок рядом?

    // ========================================================================
    // ОБНАРУЖЕНИЕ ИГРОКА
    // ========================================================================
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Проверяем, это игрок?
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = true;
            interactionUI.SetActive(true);  // Показываем "Press E"
            Debug.Log("Игрок рядом!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Игрок ушёл?
        if (collision.CompareTag("Player"))
        {
            isPlayerNear = false;
            interactionUI.SetActive(false);  // Скрываем "Press E"
            Debug.Log("Игрок ушёл");
        }
    }

    // ========================================================================
    // НАЖАТИЕ E
    // ========================================================================
    private void Update()
    {
        // Если игрок рядом и нажал E
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("E нажата! Запускаем диалог!");
            
            // Скрываем "Press E"
            interactionUI.SetActive(false);
            
            // Запускаем диалог
            dialogueManager.LoadDialogue(dialogueFileName);
        }
    }
}
