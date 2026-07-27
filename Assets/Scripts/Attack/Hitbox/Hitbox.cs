using UnityEngine;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{
    private readonly HashSet<Hurtbox> alreadyHitTargets = new();

    private DamageInfo damageInfo;

    public void Initialize(DamageInfo info)
    {
        damageInfo = info;
        alreadyHitTargets.Clear();
    }

    private void OnEnable()
    {
        alreadyHitTargets.Clear();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();

        if (hurtbox == null)
            return;

        if (!alreadyHitTargets.Add(hurtbox))
            return;

        hurtbox.GetHit(damageInfo);
    }
}