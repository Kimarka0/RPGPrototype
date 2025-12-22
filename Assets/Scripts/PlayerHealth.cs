using UnityEditor.Callbacks;
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
        OnDeath.AddListener(OnPlayerDeath);
    }

    private void OnPlayerDeath()
    {
        GetComponent<PlayerController>().enabled = false;
        GetComponent<PlayerAttack>().enabled = false;
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0,0);
        GetComponentInChildren<Animator>().SetBool("isDie", true);
        Destroy(gameObject, 5f);
    }
}
