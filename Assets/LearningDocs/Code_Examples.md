# 💻 ГОТОВЫЕ ПРИМЕРЫ КОДА ДЛЯ ВСТАВКИ

Все примеры взяты из реальной работы. Просто копируй и используй.

---

## 1. ПРОСТОЙ NPC С ДИАЛОГОМ

### Структура на сцене

```
NPC (GameObject)
├── SpriteRenderer (с картинкой торговца)
├── CircleCollider2D (Is Trigger ✓, размер 2.5)
├── SimpleDialogueTrigger (скрипт)
└── Canvas → InteractionUI (показывает "Press E")
```

### SimpleDialogueTrigger на NPC

```csharp
[SerializeField] private string dialogueFileName = "trader";
[SerializeField] private SimpleDialogueManager dialogueManager;
[SerializeField] private GameObject interactionUI;

// На сцене просто привяжи нужные объекты в Inspector
```

### JSON файл `trader.json`

```json
{
  "dialogueID": "trader",
  "dialogueName": "Торговец",
  "startNodeID": "hello",
  "nodes": [
    {
      "nodeID": "hello",
      "speakerName": "Торговец",
      "dialogueText": "О, привет! Чем я помогу?",
      "nextNodeID": "choice",
      "choices": []
    },
    {
      "nodeID": "choice",
      "speakerName": "Торговец",
      "dialogueText": "Хочешь купить или продать?",
      "nextNodeID": "",
      "choices": [
        {
          "choiceText": "Купить",
          "nextNodeID": "buy"
        },
        {
          "choiceText": "Продать",
          "nextNodeID": "sell"
        },
        {
          "choiceText": "Уходить",
          "nextNodeID": "bye"
        }
      ]
    },
    {
      "nodeID": "buy",
      "speakerName": "Торговец",
      "dialogueText": "У меня есть зелья здоровья!",
      "nextNodeID": "",
      "choices": []
    },
    {
      "nodeID": "sell",
      "speakerName": "Торговец",
      "dialogueText": "Показывай мне твои предметы!",
      "nextNodeID": "",
      "choices": []
    },
    {
      "nodeID": "bye",
      "speakerName": "Торговец",
      "dialogueText": "Приходи ещё!",
      "nextNodeID": "",
      "choices": []
    }
  ]
}
```

---

## 2. КВЕСТ ЧЕРЕЗ ДИАЛОГ

### JSON с запуском квеста

```json
{
  "nodeID": "start_quest",
  "speakerName": "Старик",
  "dialogueText": "Мне нужно убить 3 гоблинов. Согласен?",
  "nextNodeID": "",
  "choices": [
    {
      "choiceText": "Согласен!",
      "nextNodeID": "quest_accepted"
    },
    {
      "choiceText": "Отказываюсь",
      "nextNodeID": "quest_refused"
    }
  ]
},
{
  "nodeID": "quest_accepted",
  "speakerName": "Старик",
  "dialogueText": "Спасибо! Я буду ждать результата.",
  "nextNodeID": "",
  "choices": [],
  "actions": [
    {
      "actionType": "StartQuest",
      "questID": "kill_goblins"
    }
  ]
}
```

### Код в Enemy.cs (когда враг умирает)

```csharp
public class Enemy : MonoBehaviour
{
    [SerializeField] private string enemyType = "goblin";
    
    private void OnDestroy()
    {
        // Обновляем квест
        if (QuestSystem.instance != null)
        {
            QuestSystem.instance.UpdateObjective("kill_goblins", "kill_3_goblins", 1);
            Debug.Log("Врага убил! Квест обновлен.");
        }
    }
}
```

### Quest ScriptableObject

```
Create → Quest/Quests
Quest Name: Убить гоблинов
Quest ID: kill_goblins
Objectives:
  - ID: kill_3_goblins
  - Type: DefeatEnemy
  - Required: 3
```

---

## 3. СБОР ПРЕДМЕТОВ

### JSON с поиском предметов

```json
{
  "nodeID": "collect_quest",
  "speakerName": "Маг",
  "dialogueText": "Мне нужны 5 редких кристаллов. Найдёшь?",
  "nextNodeID": "",
  "choices": [
    {
      "choiceText": "Найду!",
      "nextNodeID": "quest_start"
    }
  ]
},
{
  "nodeID": "quest_start",
  "speakerName": "Маг",
  "dialogueText": "Хорошо, жду!",
  "nextNodeID": "",
  "choices": [],
  "actions": [
    {
      "actionType": "StartQuest",
      "questID": "collect_crystals"
    }
  ]
}
```

### Код в Crystal.cs (предмет в миру)

```csharp
public class Crystal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Это игрок?
        if (other.CompareTag("Player"))
        {
            // Обновляем квест на сбор
            if (QuestSystem.instance != null)
            {
                QuestSystem.instance.UpdateObjective(
                    "collect_crystals",     // ID квеста
                    "collect_5_crystals",   // ID цели
                    1                       // Количество
                );
            }

            // Удаляем предмет
            Destroy(gameObject);
            
            Debug.Log("Кристалл собран!");
        }
    }
}
```

### Quest ScriptableObject

```
Quest Name: Собрать кристаллы
Quest ID: collect_crystals
Objectives:
  - ID: collect_5_crystals
  - Type: CollectItem
  - Required: 5
```

---

## 4. ЗАВЕРШЕНИЕ КВЕСТА И НАГРАДА

### JSON с получением награды

```json
{
  "nodeID": "reward",
  "speakerName": "Маг",
  "dialogueText": "Ты принёс все кристаллы! Вот твоя награда!",
  "nextNodeID": "",
  "choices": [],
  "actions": [
    {
      "actionType": "CompleteQuest",
      "questID": "collect_crystals"
    },
    {
      "actionType": "GiveItem",
      "questID": "",
      "itemID": "mage_staff",
      "amount": 1,
      "parameter": ""
    }
  ]
}
```

### Обновлённый SimpleDialogueManager (с поддержкой actions)

Добавь в класс:

```csharp
[System.Serializable]
public class SimpleDialogueAction
{
    [SerializeField] public string actionType;
    [SerializeField] public string questID;
    [SerializeField] public string itemID;
    [SerializeField] public int amount;
}

// Обнови класс SimpleDialogueNode:
[System.Serializable]
public class SimpleDialogueNode
{
    [SerializeField] public string nodeID;
    [SerializeField] public string speakerName;
    [SerializeField] public string dialogueText;
    [SerializeField] public string nextNodeID = "";
    [SerializeField] public List<SimpleDialogueChoice> choices = new List<SimpleDialogueChoice>();
    [SerializeField] public List<SimpleDialogueAction> actions = new List<SimpleDialogueAction>(); // ← ДОБАВЬ ЭТО
}

// И в методе ShowNode добавь перед концом:
private void ShowNode(string nodeID)
{
    // ... существующий код ...
    
    // ====== ВЫПОЛНЯЕМ ДЕЙСТВИЯ ======
    if (currentNode.actions != null && currentNode.actions.Count > 0)
    {
        ExecuteActions(currentNode.actions);
    }
}

// Добавь новый метод:
private void ExecuteActions(List<SimpleDialogueAction> actions)
{
    foreach (var action in actions)
    {
        if (action.actionType == "StartQuest")
        {
            if (QuestSystem.instance != null)
            {
                QuestSystem.instance.StartQuest(action.questID);
                Debug.Log($"Квест начался: {action.questID}");
            }
        }
        else if (action.actionType == "CompleteQuest")
        {
            if (QuestSystem.instance != null)
            {
                QuestSystem.instance.CompleteQuest(action.questID);
                Debug.Log($"Квест завершён: {action.questID}");
            }
        }
    }
}
```

---

## 5. ПРОВЕРКА СТАТУСА КВЕСТА В ДИАЛОГЕ

### JSON с проверкой

```json
{
  "nodeID": "greeting",
  "speakerName": "NPC",
  "dialogueText": "Привет! Как дела?",
  "nextNodeID": "",
  "choices": [
    {
      "choiceText": "Хочу квест",
      "nextNodeID": "check_quest"
    }
  ]
},
{
  "nodeID": "check_quest",
  "speakerName": "NPC",
  "dialogueText": "??? (это узел для проверки)",
  "nextNodeID": "",
  "choices": []
}
```

### Код проверки в SimpleDialogueManager

Добавь в ShowNode:

```csharp
private void ShowNode(string nodeID)
{
    // ... существующий код ...
    
    // ====== ПРОВЕРКА УСЛОВИЙ ======
    if (nodeID == "check_quest")
    {
        // Если квест завершён - показываем reward
        if (QuestSystem.instance.IsQuestCompleted("kill_goblins"))
        {
            ShowNode("reward");
            return;
        }
        // Если квест активен - показываем "жди"
        else if (QuestSystem.instance.IsQuestActive("kill_goblins"))
        {
            ShowNode("wait_for_completion");
            return;
        }
        // Если не начинал - показываем предложение
        else
        {
            ShowNode("start_quest");
            return;
        }
    }
    
    // Остальной код...
}
```

---

## 6. НЕСКОЛЬКО КВЕСТОВ ОТ ОДНОГО NPC

### JSON с выбором квеста

```json
{
  "dialogueID": "guild_master",
  "dialogueName": "Мастер Гильдии",
  "startNodeID": "welcome",
  "nodes": [
    {
      "nodeID": "welcome",
      "speakerName": "Мастер",
      "dialogueText": "Добро пожаловать в гильдию! Какой квест выбираешь?",
      "nextNodeID": "",
      "choices": [
        {
          "choiceText": "Охота на монстров",
          "nextNodeID": "hunt_quest"
        },
        {
          "choiceText": "Поиск артефакта",
          "nextNodeID": "artifact_quest"
        },
        {
          "choiceText": "Помощь деревне",
          "nextNodeID": "village_quest"
        }
      ]
    },
    {
      "nodeID": "hunt_quest",
      "speakerName": "Мастер",
      "dialogueText": "Убей 5 монстров в лесу!",
      "nextNodeID": "",
      "choices": [],
      "actions": [
        {
          "actionType": "StartQuest",
          "questID": "hunt_monsters"
        }
      ]
    },
    {
      "nodeID": "artifact_quest",
      "speakerName": "Мастер",
      "dialogueText": "Найди артефакт в подземелье!",
      "nextNodeID": "",
      "choices": [],
      "actions": [
        {
          "actionType": "StartQuest",
          "questID": "find_artifact"
        }
      ]
    },
    {
      "nodeID": "village_quest",
      "speakerName": "Мастер",
      "dialogueText": "Помоги деревне, собери дрова!",
      "nextNodeID": "",
      "choices": [],
      "actions": [
        {
          "actionType": "StartQuest",
          "questID": "collect_wood"
        }
      ]
    }
  ]
}
```

---

## 7. ПРОВЕРКА КОНСОЛИ ДЛЯ ОТЛАДКИ

### Что ты должен видеть при работе

Когда всё работает правильно:

```
// При загрузке диалога:
[DialogueManager] Диалог загружен успешно: Приветствие

// При нажатии E:
[DialogueTrigger] E нажата! Запускаем диалог!
[DialogueTrigger] Диалог начался: npc_greeting

// При выборе:
[DialogueManager] Показано 2 варианта ответа

// При завершении:
[DialogueTrigger] Диалог завершился

// При запуске квеста:
[QuestSystem] Квест начался: Собрать травы

// При обновлении:
[QuestSystem] Обновлена цель: collect_herbs/herb_1
```

### Если что-то не работает

Добавь свои Debug.Log в код:

```csharp
void Update()
{
    if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
    {
        Debug.Log($"E нажата! Player near: {isPlayerNear}"); // ← ДОБАВЬ
        Debug.Log($"Dialogue Manager: {dialogueManager}");   // ← ДОБАВЬ
        dialogueManager.LoadDialogue(dialogueFileName);
    }
}
```

---

## 8. БЫСТРЫЙ ЧЕКЛИСТ ПЕРЕД ТЕСТОМ

- [ ] JSON файл в `Assets/Resources/Dialogues/`
- [ ] JSON синтаксис правильный (проверь jsonlint.com)
- [ ] На NPC есть Collider 2D с Is Trigger ✓
- [ ] SimpleDialogueManager назначен в Inspector
- [ ] InteractionUI Panel создан
- [ ] Игрок имеет тег "Player"
- [ ] Нет красных ошибок в консоли
- [ ] Выборы ведут на существующие узлы

---

**Скопируй нужный пример, адаптируй под себя и пробуй! 🚀**
