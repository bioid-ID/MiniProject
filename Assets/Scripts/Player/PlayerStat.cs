using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [Header("Level System")]
    [SerializeField] private int level = 1;
    [SerializeField] private int currentExp = 0;
    [SerializeField] private int maxExp = 100;

    [Header("Base Status")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackSpeed = 1f;

    public int Level => level;
    public float MaxHealth => maxHealth;
    public float AttackDamage => attackDamage;
    public float AttackSpeed => attackSpeed;

    public void AddExp(int amount)
    {
        currentExp += amount;
        if (currentExp >= maxExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExp -= maxExp;
        level++;
        maxExp = Mathf.RoundToInt(maxExp * 1.2f);

        maxHealth += 10f;
        attackDamage += 2f;

        PlayerManager.Instance.Health.Heal(maxHealth);
        Debug.Log($"level up");
    }
}
