using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance {  get; private set; }

    [Header("Player Components")]

    public PlayerController Controller { get; private set; }
    public PlayerAttack Attack { get; private set; }
    public PlayerHealth Health { get; private set; }
    public PlayerStat Stat { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
           
        }
        Controller = GetComponent<PlayerController>();
        Attack = GetComponent<PlayerAttack>();
        Health = GetComponent<PlayerHealth>();
        Stat = GetComponent<PlayerStat>();
    }

    private void Start()
    {
        Debug.Log("Success");
    }

    public void TargetTakeDamage(float amount)
    {
        if(Health != null) 
        {
            Health.TakeDamage(amount);
        }
    }

}


