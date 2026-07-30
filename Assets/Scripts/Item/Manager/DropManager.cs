using UnityEngine;

public class DropManager : MonoBehaviour
{
    public static DropManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void Drop(Enemy enemy)
    {
        foreach (var drop in enemy.Data.dropTable)
        {
            if (Random.value * 100f > drop.chance)
                continue;

            Loot loot =
                PoolManager.Instance.Get<Loot>();

            loot.Initialize(drop.loot);

            loot.transform.position =
                enemy.transform.position;
        }
    }
}