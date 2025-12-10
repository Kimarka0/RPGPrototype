# 🚀 НАСТРОЙКА МИНИМАЛЬНОЙ СИСТЕМЫ ДИАЛОГОВ (БЕЗ КВЕСТОВ)

## Что здесь

- Только диалоги (без квестов)
- БЕЗ лямбд и сложных вещей
- 3 простых класса + JSON файл
- **Всё что нужно для начала**

---

## ШАГИ НАСТРОЙКИ

### 1️⃣ Скопируй код

Файл `SimpleDialogueSystem.cs` содержит 3 класса:
- `SimpleDialogueNode` - одна фраза
- `SimpleDialogueManager` - показывает диалог
- `SimpleDialogueTrigger` - слушает E и запускает диалог

Скопируй ВЕСЬ файл в папку `Assets/Scripts/Dialogue/`

### 2️⃣ Создай JSON файл

В папке `Assets/Resources/Dialogues/` создай файл `npc_greeting.json`:

```json
{
  "dialogueID": "npc_greeting",
  "dialogueName": "Приветствие",
  "startNodeID": "0",
  "nodes": [
    {
      "nodeID": "0",
      "speakerName": "Торговец",
      "dialogueText": "Привет! Как дела?",
      "nextNodeID": "1",
      "choices": []
    },
    {
      "nodeID": "1",
      "speakerName": "Торговец",
      "dialogueText": "Может быть помогу тебе с чем-то?",
      "nextNodeID": "",
      "choices": [
        {
          "choiceText": "Да, помоги!",
          "nextNodeID": "2"
        },
        {
          "choiceText": "Нет, спасибо",
          "nextNodeID": "3"
        }
      ]
    },
    {
      "nodeID": "2",
      "speakerName": "Торговец",
      "dialogueText": "Отлично! Слушаю тебя.",
      "nextNodeID": "",
      "choices": []
    },
    {
      "nodeID": "3",
      "speakerName": "Торговец",
      "dialogueText": "Ладно, удачи!",
      "nextNodeID": "",
      "choices": []
    }
  ]
}
```

**Сохрани как:** `Assets/Resources/Dialogues/npc_greeting.json`

### 3️⃣ Создай Canvas с UI

На сцене:
1. **Right Click → UI → Canvas** (если его нет)

В Canvas создай:
```
Canvas
├── DialoguePanel (Panel)
│   ├── NameText (TextMeshProUGUI)
│   ├── DialogueText (TextMeshProUGUI)
│   └── ChoicesContainer (Panel с Vertical Layout Group)
│       └── ChoiceButton (Button) - префаб для кнопок
├── InteractionUI (Panel)
│   └── Text (TextMeshProUGUI) - "Press E"
```

### 4️⃣ Добавь компоненты

#### На пустой GameObject создай SimpleDialogueManager:

1. **Create Empty** на сцене, назови "DialogueManager"
2. **Add Component → SimpleDialogueManager** (из скрипта)
3. В Inspector заполни:
   - **Name Text:** NameText из Canvas
   - **Dialogue Text:** DialogueText из Canvas
   - **Choices Container:** ChoicesContainer из Canvas
   - **Choice Button Prefab:** ChoiceButton
   - **Dialogue Panel:** DialoguePanel

#### На NPC добавь SimpleDialogueTrigger:

1. Select NPC GameObject
2. **Add Component → Collider 2D** (Box Collider 2D или Circle Collider 2D)
3. **Отметь "Is Trigger"** ✓
4. **Add Component → SimpleDialogueTrigger** (из скрипта)
5. В Inspector:
   - **Dialogue File Name:** `npc_greeting` (БЕЗ .json!)
   - **Dialogue Manager:** Перетащи GameObject с SimpleDialogueManager
   - **Interaction UI:** Перетащи InteractionUI Panel

### 5️⃣ Настрой игрока

- **Tag:** "Player" (добавь тег если нет)
- На игроке должен быть Collider 2D

---

## ТЕСТИРОВАНИЕ

1. **Сыграй сцену**
2. **Подойди к NPC**
3. **Должно появиться "Press E"**
4. **Нажми E**
5. **Появится диалог**
6. **Выбери вариант и смотри результат**

---

## КАК ДОБАВИТЬ НОВОГО NPC?

### Способ 1: Простая копия

1. Скопируй существующий NPC (Ctrl+D)
2. Измени его спрайт
3. В `SimpleDialogueTrigger` измени **Dialogue File Name** на другой файл
4. Создай новый JSON для этого диалога

### Способ 2: Новый JSON файл

1. Создай новый файл в `Assets/Resources/Dialogues/` например `merchant_quest.json`
2. Скопируй структуру из `npc_greeting.json`
3. Измени текст и узлы как нужно
4. На NPC в **Dialogue File Name** напиши `merchant_quest`

---

## СТРУКТУРА JSON ФАЙЛА - ПРОСТО

### Минимум:

```json
{
  "dialogueID": "any_id",
  "dialogueName": "Любое имя",
  "startNodeID": "0",
  "nodes": [
    {
      "nodeID": "0",
      "speakerName": "NPC",
      "dialogueText": "Текст",
      "nextNodeID": "",
      "choices": []
    }
  ]
}
```

Это самый простой диалог - одна фраза и конец.

### С выборами:

```json
{
  "nodeID": "0",
  "speakerName": "NPC",
  "dialogueText": "Выбери",
  "nextNodeID": "",
  "choices": [
    {
      "choiceText": "Вариант 1",
      "nextNodeID": "1"
    },
    {
      "choiceText": "Вариант 2",
      "nextNodeID": "2"
    }
  ]
}
```

### Несколько фраз подряд:

```json
{
  "nodeID": "0",
  "speakerName": "NPC",
  "dialogueText": "Первая фраза",
  "nextNodeID": "1",
  "choices": []
},
{
  "nodeID": "1",
  "speakerName": "NPC",
  "dialogueText": "Вторая фраза",
  "nextNodeID": "2",
  "choices": []
},
{
  "nodeID": "2",
  "speakerName": "NPC",
  "dialogueText": "Третья фраза",
  "nextNodeID": "",
  "choices": []
}
```

---

## КОД БЕЗ ЛЯМБД - КАК ЭТО РАБОТАЕТ

Самая запутанная часть в оригинальном коде это был вот это:

```csharp
// ❌ БЫЛО (с лямбдой):
choiceButton.onClick.AddListener(() => 
{
    DisplayNode(choice.NextNodeID);
});
```

В упрощённой версии **БЕЗ лямбд**:

```csharp
// ✅ СТАЛО (без лямбды):
// 1. Сохраняем номер выбора
int choiceIndex = i;

// 2. Вызываем обычный метод
newButton.onClick.AddListener(delegate { OnChoiceClicked(choiceIndex); });

// 3. Метод просто получает номер и делает нужное
private void OnChoiceClicked(int choiceIndex)
{
    // Берём выбор по номеру
    SimpleDialogueChoice selected = currentNode.choices[choiceIndex];
    
    // Переходим на узел
    ShowNode(selected.nextNodeID);
}
```

**Суть:** вместо `() => { код }` используем обычный метод.

---

## ПРИМЕРЫ JSON ДИАЛОГОВ

### Пример 1: Продавец в лавке

```json
{
  "dialogueID": "shop",
  "dialogueName": "Магазин",
  "startNodeID": "0",
  "nodes": [
    {
      "nodeID": "0",
      "speakerName": "Продавец",
      "dialogueText": "Добро пожаловать в мою лавку!",
      "nextNodeID": "1",
      "choices": []
    },
    {
      "nodeID": "1",
      "speakerName": "Продавец",
      "dialogueText": "Что тебе нужно?",
      "nextNodeID": "",
      "choices": [
        {
          "choiceText": "Купить зелье",
          "nextNodeID": "2"
        },
        {
          "choiceText": "Продать вещи",
          "nextNodeID": "3"
        },
        {
          "choiceText": "Уходить",
          "nextNodeID": "4"
        }
      ]
    },
    {
      "nodeID": "2",
      "speakerName": "Продавец",
      "dialogueText": "Вот мои зелья. Бери!",
      "nextNodeID": "",
      "choices": []
    },
    {
      "nodeID": "3",
      "speakerName": "Продавец",
      "dialogueText": "Покажи мне свои вещи.",
      "nextNodeID": "",
      "choices": []
    },
    {
      "nodeID": "4",
      "speakerName": "Продавец",
      "dialogueText": "Приходи ещё!",
      "nextNodeID": "",
      "choices": []
    }
  ]
}
```

### Пример 2: Квестгивер

```json
{
  "dialogueID": "quest_giver",
  "dialogueName": "Квестгивер",
  "startNodeID": "0",
  "nodes": [
    {
      "nodeID": "0",
      "speakerName": "Старик",
      "dialogueText": "Слушай, мне нужна помощь!",
      "nextNodeID": "1",
      "choices": []
    },
    {
      "nodeID": "1",
      "speakerName": "Старик",
      "dialogueText": "Мне нужно убить монстра в пещере. Согласен?",
      "nextNodeID": "",
      "choices": [
        {
          "choiceText": "Согласен!",
          "nextNodeID": "2"
        },
        {
          "choiceText": "Отказываюсь",
          "nextNodeID": "3"
        }
      ]
    },
    {
      "nodeID": "2",
      "speakerName": "Старик",
      "dialogueText": "Спасибо! Вот тебе 100 золота авансом.",
      "nextNodeID": "",
      "choices": []
    },
    {
      "nodeID": "3",
      "speakerName": "Старик",
      "dialogueText": "Жаль. Если передумаешь - приходи.",
      "nextNodeID": "",
      "choices": []
    }
  ]
}
```

---

## ЧАСТЫ ОШИБКИ

| Ошибка | Причина | Решение |
|--------|---------|---------|
| "Диалог не появляется" | JSON не найден | Проверь путь: `Assets/Resources/Dialogues/` и имя файла |
| "Кнопки не появляются" | Prefab не заполнен | В Inspector заполни `Choice Button Prefab` |
| "E не работает" | Нет Player тега | Добавь тег "Player" к игроку |
| "Консоль: null reference" | Manager не назначен | На NPC заполни `Dialogue Manager` |
| "JSON ошибка" | Синтаксис JSON неправильный | Проверь на jsonlint.com |

---

## ГОТОВ К КВЕСТАМ?

Когда эта система будет работать в совершенстве:

1. **Добавляем Quest.cs** и создаём Quest ScriptableObject
2. **Добавляем в диалог действие** - StartQuest
3. **Добавляем QuestSystem** - управляет квестами
4. **Готово!**

Но сначала убедись что диалоги работают идеально.

---

**Почти готово! Пишешь первого NPC? Дай знать что не получается 🎮**
