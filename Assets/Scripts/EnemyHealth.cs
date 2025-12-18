using UnityEngine;

public class EnemyHealth : Health
{
    [Header("Enemy Setting")]
    [SerializeField] private string enemyID;
    private void Start()
    {
        base.Awake();
        OnDeath.AddListener(OnEnemyDeath);
    }

    private void OnEnemyDeath()
    {
        UpdateKillQuests();
        Destroy(gameObject);
    }

    private void UpdateKillQuests()
    {
        if(QuestSystem.instance == null) return;

        var activeQUests = QuestSystem.instance.GetAllActiveQuests();

        foreach(var questProgress in activeQUests)
        {
            foreach(var objective in questProgress.Objectives)
            {
                if(objective.Type == ObjectiveType.DefeatEnemy && !objective.IsCompleted)
                {
                    if (string.IsNullOrWhiteSpace(objective.RequiredEnemyID))
                    {
                        QuestSystem.instance.UpdateObjective(questProgress.Quest.QuestID, objective.ObjectiveID, 1);
                    }
                    else if(objective.RequiredEnemyID == enemyID)
                    {
                        QuestSystem.instance.UpdateObjective(questProgress.Quest.QuestID, objective.ObjectiveID, 1);
                    }
                }
            }
        }
    }
}
