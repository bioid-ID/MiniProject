using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile2D : MonoBehaviour
{
    [Header("투사체")]
    [SerializeField] private float speed = 12f;
    [SerializeField] private float lifeTime = 2.5f;

    private float currentDamage;     // 관통하면서 실시간으로 깎일 현재 대미지
    private float damageDecayRate;   // 관통 시 대미지 감쇠율 (PlayerStat의 FinalDamageDecay)
    private int remainingPiercing;   // 남은 관통 횟수 (PlayerStat의 TotalPiercingCount)

    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    /// <param name="baseDmg">플레이어의 최종 공격 대미지 
    /// <param name="piercingCount">플레이어의 최종 관통 횟수 
    /// <param name="decayRate">플레이어의 최종 대미지 감쇠율
    public void Launch(float baseDmg, int piercingCount, float decayRate)
    {
        currentDamage = baseDmg;
        remainingPiercing = piercingCount;
        damageDecayRate = decayRate;

        // 유니티 최신 버전의 rb2d.linearVelocity 대응
        rb2d.linearVelocity = transform.right * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hurtbox2D hurtbox = other.GetComponent<Hurtbox2D>();
        if (hurtbox != null)
        {
            // 1. 현재 계산된 대미지로 적을 타격 (소수점 대미지 전달 가능)
            hurtbox.GetHit(currentDamage);

            // 2. 관통 처리 판정
            if (remainingPiercing > 0)
            {
                remainingPiercing--; // 관통 횟수 1회 차감

                // 대미지 경감 공식: 현재 대미지에서 감쇠율만큼 차감
                // (예: 대미지 100, 감쇠율 0.2(20%)면 -> 100 * (1 - 0.2) = 80)
                currentDamage *= (1f - damageDecayRate);

                // 투사체를 파괴하지 않고 그대로 통과
                Debug.Log($"적 관통 완료! 남은 관통수: {remainingPiercing}, 다음 대미지: {currentDamage}");
            }
            else
            {
                // 관통 횟수가 남아있지 않다면 투사체를 파괴
                Destroy(gameObject);
            }
        }
    }
}