using UnityEngine;

public class LootMagnet : MonoBehaviour
{
    [SerializeField]
    float radius = 5;

    [SerializeField]
    float speed = 8;

    private void Update()
    {
        Collider2D[] cols =
            Physics2D.OverlapCircleAll(
                transform.position,
                radius);

        foreach (var col in cols)
        {
            Loot loot =
                col.GetComponent<Loot>();

            if (loot == null)
                continue;

            loot.transform.position =
                Vector3.MoveTowards(
                    loot.transform.position,
                    transform.position,
                    speed * Time.deltaTime);
        }
    }
}