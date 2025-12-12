using UnityEngine;

public class PlayerHealth : Health
{
    public static PlayerHealth instance;

    protected override void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        base.Awake();
    }

    private void Update()
    {
        TakeDamage(10);
    }
}
