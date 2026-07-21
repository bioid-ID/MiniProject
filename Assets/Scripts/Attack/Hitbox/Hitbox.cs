using UnityEngine;
using System.Collections.Generic;

public class Hitbox : MonoBehaviour
{
    private float damage;
    private List<Hurtbox> alreadyHitTargets = new List<Hurtbox>();

    public void Initialize(float dmg)
    {
        damage = dmg;
        alreadyHitTargets.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null && !alreadyHitTargets.Contains(hurtbox))
        {
            alreadyHitTargets.Add(hurtbox);
            hurtbox.GetHit(damage);

        }
    }
}