using UnityEngine;
using System.Collections.Generic;

public class Hitbox2D : MonoBehaviour
{
    private float damage;
    private List<Hurtbox2D> alreadyHitTargets = new List<Hurtbox2D>(); 

    public void Initialize(float dmg)
    {
        damage = dmg;
        alreadyHitTargets.Clear(); 
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        Hurtbox2D hurtbox = other.GetComponent<Hurtbox2D>();
        
        if (hurtbox != null && !alreadyHitTargets.Contains(hurtbox))
        {
            alreadyHitTargets.Add(hurtbox); 
            hurtbox.GetHit(damage);         
        }
    }
}
