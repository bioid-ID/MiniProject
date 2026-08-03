using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class LootObject : MonoBehaviour
{
    [SerializeField] private float magnetDistance = 3f;
    [SerializeField] private float moveSpeed = 10f;

    private ItemData item;
    private int amount;

    private Transform player;

    public void Initialize(ItemData item, int amount)
    {
        this.item = item;
        this.amount = amount;

        player = PlayerManager.Instance.transform;
    }

    private void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector2.Distance(
                transform.position,
                player.position);

        if (distance > magnetDistance)
            return;

        transform.position =
            Vector2.MoveTowards(
                transform.position,
                player.position,
                moveSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        Inventory.Instance?.AddItem(item, amount);

      //  PoolManager.Instance.ReturnLoot(this);
    }

}