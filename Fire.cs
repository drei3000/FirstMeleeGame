using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileCollisionDetection : MonoBehaviour
{

    public GameObject HitParticle; // Reference to the hit particle effect
    public float damage = 10f; // Damage dealt by the projectile

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object is an enemy
        if (other.CompareTag("PlayerHitBox"))
        {
            // Apply damage to the player
            PlayerHealth player = other.GetComponent<PlayerHealth>();
            if (player != null)
            {
                player.TakeDamage(damage);
                
            } 

          
        }
    }
}
