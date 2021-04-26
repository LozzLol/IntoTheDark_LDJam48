using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarracudaEnemy : Enemy
{
    public bool justDamagedPlayer;
    public float dashSpeed, lethalVelocity;
    
    public Transform graphicsTransform;

    private void Update()
    {
        transform.right = -rb2d.velocity.normalized;
        //_collider2D.transform.right = -rb2d.velocity.normalized;
    }
    
    
    public override void Attack()
    {
        Debug.Log(gameObject.name + " is attacking the player!");
        //Dash
        rb2d.AddForce(dashSpeed * (playerTransform.position - transform.position).normalized);
        StartCoroutine(WaitThenCanAttack());
        _audioSource.PlayOneShot(attackSound);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (justDamagedPlayer) return;
        if (rb2d.velocity.magnitude<lethalVelocity) return;
        Debug.Log(gameObject.name+" hit "+other.gameObject.name +" at "+rb2d.velocity.magnitude);
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<PlayerCombat>().TakeDamage(this);
            StartCoroutine(DamagedPlayerCooldown());
        }
    }

    private IEnumerator DamagedPlayerCooldown()
    {
        justDamagedPlayer = true;
        yield return new WaitForSeconds(attackDelay);
        justDamagedPlayer = false;
    }
}
