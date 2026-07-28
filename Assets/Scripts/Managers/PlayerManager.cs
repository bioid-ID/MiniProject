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
        if (Health != null)
        {
            DamageInfo simpleDamage = new DamageInfo(
                attacker: null,                 // 공격자 없음 (환경 요소 등)
                damage: amount,                 // 전달받은 대미지 수치
                damageType: DamageType.Physical,// 기본 물리 대미지 타입
                team: TeamType.Enemy            // 적 진영으로부터 받는 대미지 처리
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



