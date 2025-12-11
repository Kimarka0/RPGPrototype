using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Transform heartsContainer;

    [Header("Settings")]
    [SerializeField] private int healthPerHeart = 20;
    [SerializeField] private bool createHeartsAtStart = true;

    private List<Image> hearts = new List<Image>();

    private void Start()
    {
        if(PlayerHealth.instance != null)
        {
            PlayerHealth.instance.OnHealthChanged.AddListener(UpdateHealthUI);

            if (createHeartsAtStart)
            {
                CreateHearts(PlayerHealth.instance.MaxHealth);
                UpdateHealthUI(PlayerHealth.instance.CurrentHealth, PlayerHealth.instance.MaxHealth);
            }
            else
            {
                Debug.Log("PlayerHealth not found!");
            }
        }
    }

    private void CreateHearts(int maxHealth)
    {
        foreach(Transform child in heartsContainer)
        {
            Destroy(child.gameObject);
        }
        hearts.Clear();

        int heartCount = Mathf.CeilToInt((float)maxHealth / healthPerHeart);

        for(int i = 0; i < heartCount; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartsContainer);
            Image heartImage = GetComponent<Image>();

            if(heartImage != null)
            {
                hearts.Add(heartImage);
            }
            else
            {
                Debug.Log("Heart Image dont have Image component!");
            }
        }
        Debug.Log($"Created hearts {heartCount}");
    }

    private void UpdateHealthUI(int current, int max)
    {
        int requiredHearts = Mathf.CeilToInt((float) max / healthPerHeart);
        if(requiredHearts != hearts.Count)
        {
            CreateHearts(max);
        }

        for(int i = 0; i < hearts.Count; i++)
        {
            int heartMinHP = i * healthPerHeart;
            int heartMaxHP = (i + 1) * healthPerHeart;

            if(current > heartMinHP)
            {
                hearts[i].fillAmount = 1f;
            }
            else if (current > heartMinHP)
            {
                float fill = (float)(current - heartMinHP) / healthPerHeart;
                hearts[i].fillAmount = fill;
            }
            else
            {
                hearts[i].fillAmount = 0f;
            }
        }
    }
    private void OnDestroy()
    {
        if(PlayerHealth.instance != null)
        {
            PlayerHealth.instance.OnHealthChanged?.RemoveListener(UpdateHealthUI);
        }
    }
}
