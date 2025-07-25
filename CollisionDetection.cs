using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public WeaponController weaponController;
    public GameObject HitParticle;

    
    private void OnTriggerEnter(Collider other) {
        // Check if the collided object is an enemy and the weapon is attacking
        if (other.CompareTag("Enemy") && weaponController.isAttacking) {
            Debug.Log("Hit");

            // Ensure this script is attached to the knife
            if (gameObject.CompareTag("Knife")) {
                // Create hit particle effect
                Instantiate(HitParticle, new Vector3(other.transform.position.x, 
                transform.position.y, other.transform.position.z), other.transform.rotation);

                // Apply damage to the enemy
                EnemyAi enemyAi = other.GetComponent<EnemyAi>();
                if (enemyAi != null) {
                    float damage = weaponController.attackDamage; // Replace with your weapon's damage value
                    enemyAi.TakeDamage(damage);
                    Debug.Log("Target took " + damage + " damage.");
                }
            }
        }
    }
}

