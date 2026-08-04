using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LootPhysicsBehavior : MonoBehaviour
{
    [SerializeField] private float popForceMin = 2.5f;
    [SerializeField] private float popForceMax = 5.5f;
    [SerializeField] private float settleSpeedThreshold = 0.35f;
    [SerializeField] private float magnetRadius = 4.5f;
    [SerializeField] private float magnetAcceleration = 18f;
    [SerializeField] private float maxMagnetSpeed = 10f;
    [SerializeField] private float pickupDistance = 0.42f;

    private Rigidbody2D rb;
    private Loot loot;
    private bool popFinished;
    private bool magnetEnabled = true;

    public bool CanPickup { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        loot = GetComponent<Loot>();
    }

    public void ResetPhysics()
    {
        popFinished = false;
        CanPickup = false;

        if (rb == null)
            return;

        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.linearDamping = 4f;
        rb.angularDamping = 1.5f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    public void LaunchPop(Vector3 spawnPosition)
    {
        ResetPhysics();
        transform.position = spawnPosition;

        Vector2 randomDirection = Random.insideUnitCircle;
        if (randomDirection.sqrMagnitude < 0.01f)
            randomDirection = Vector2.up;

        randomDirection.Normalize();
        rb.linearVelocity = randomDirection * Random.Range(popForceMin, popForceMax);

        StopAllCoroutines();
        StartCoroutine(WaitUntilSettled());
    }

    private IEnumerator WaitUntilSettled()
    {
        yield return new WaitForSeconds(0.08f);

        float timeout = 0.8f;
        while (timeout > 0f && rb.linearVelocity.magnitude > settleSpeedThreshold)
        {
            timeout -= Time.deltaTime;
            yield return null;
        }

        popFinished = true;
        CanPickup = true;
    }

    private void FixedUpdate()
    {
        if (!popFinished || loot == null || !loot.IsSpawned)
            return;

        Transform playerTransform = GetPlayerTransform();
        if (playerTransform == null)
            return;

        Vector2 toPlayer = (Vector2)(playerTransform.position - transform.position);
        float distance = toPlayer.magnitude;

        if (distance <= pickupDistance && CanPickup)
        {
            loot.TryPickup();
            return;
        }

        if (!magnetEnabled || distance > magnetRadius)
            return;

        Vector2 desiredVelocity = toPlayer.normalized * maxMagnetSpeed;
        rb.linearVelocity = Vector2.MoveTowards(rb.linearVelocity, desiredVelocity, magnetAcceleration * Time.fixedDeltaTime);
    }

    private static Transform GetPlayerTransform()
    {
        GameObject player = PlayerSpawnUtility.FindExistingPlayer();
        return player != null ? player.transform : null;
    }
}
