using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject Knife;
    public bool canAttack = true;
    public float attackCooldown = 0.3f;
    public bool isAttacking = false;
    public float knockbackForce = 10f; 
    public float attackDamage = 10f; // Damage dealt by the weapon

    private void Update()
    {
        if(Input.GetMouseButtonDown(0) )
        {
            if (canAttack)
            {
                knifeAttack();
                StartCoroutine(AttackCooldownCoroutine());
            }
        }
    }

    public void knifeAttack()
    {
        isAttacking = true; // For hit detection
        canAttack = false;
        Animator swordAnim = Knife.GetComponent<Animator>();
        swordAnim.SetTrigger("Attack");
    }

    // Start attack cooldown
    IEnumerator AttackCooldownCoroutine()
    {
        StartCoroutine(ResetAttackBool());
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    // Reset the isAttacking bool after a delay
    IEnumerator ResetAttackBool() {
        yield return new WaitForSeconds(1.0f);
        isAttacking = false;
    }
}