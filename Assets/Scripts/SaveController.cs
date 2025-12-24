using System.IO;
using UnityEngine;

using System.Collections.Generic;

public class SaveController : MonoBehaviour
{
    private string saveLocation;
    private InventoryController inventoryController;

    void Start()
    {
        saveLocation = Path.Combine(Application.persistentDataPath, "saveData.json");

        inventoryController = FindAnyObjectByType<InventoryController>();
        Debug.Log($"InventoryController найден: {inventoryController != null}");

        if (PlayerController.OnPlayerReady != null)
            LoadGame();
        else
            PlayerController.OnPlayerReady += LoadGame;
    }

    public void SaveGame()
    {
        var items = inventoryController.GetInventoryItems();

        SaveData saveData = new SaveData
        {
            playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position,
            inventorySaveData = items
        };

        File.WriteAllText(saveLocation, JsonUtility.ToJson(saveData));
        Debug.Log($"[SAVE] Сохранено, предметов: {items.Count}");
    }

    private void LoadGame()
    {
        Debug.Log("LoadGame вызван!");

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("PLAYER НЕ НАЙДЕН!");
            return;
        }

        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        if (File.Exists(saveLocation))
        {
            SaveData saveData = JsonUtility.FromJson<SaveData>(File.ReadAllText(saveLocation));

            rb.linearVelocity = Vector2.zero;
            player.transform.position = saveData.playerPosition;
            Debug.Log($"Загружена позиция: {saveData.playerPosition}");

            if (inventoryController != null)
                inventoryController.SetInventoryItems(saveData.inventorySaveData);
        }
        else
        {
            Debug.Log("Нет сохранения, создаю новое");
            inventoryController.SetInventoryItems(new List<InventorySaveData>());
            SaveGame();
        }
    }

    void OnDestroy()
    {
        PlayerController.OnPlayerReady -= LoadGame;
    }
}

