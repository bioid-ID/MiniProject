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
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;

        Controller = GetComponent<PlayerController>();
        Attack = GetComponent<PlayerAttack>();
        Health = GetComponent<PlayerHealth>();
        Stat = GetComponent<PlayerStat>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Start()
    {
        Debug.Log("Success");
    }
    public void TargetTakeDamage(float amount)
    {
        if (Health != null)
        {
            DamageInfo simpleDamage = new DamageInfo(
                attacker: null,                 // ������ ���� (ȯ�� ��� ��)
                damage: amount,                 // ���޹��� ����� ��ġ
                damageType: DamageType.Physical,// �⺻ ���� ����� Ÿ��
                team: TeamType.Enemy            // �� �������κ��� �޴� ����� ó��
            );

            Health.TakeDamage(simpleDamage);
        }
    }
    public void TargetTakeDamage(DamageInfo damageInfo)
    {
        if (Health != null)
        {
            Health.TakeDamage(damageInfo);
        }
    }
}



