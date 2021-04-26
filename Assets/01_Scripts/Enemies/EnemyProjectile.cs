using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public Enemy parentEnemy;
    public float projectileVelocity;
    public Rigidbody2D rb2d;

    private Collider2D thisCollider;

    private void Awake()
    {
        thisCollider = GetComponentInChildren<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(gameObject+" collided with "+other.gameObject);
        if (other.CompareTag("Player"))
        {
            other.GetComponentInParent<PlayerCombat>().TakeDamage(parentEnemy);
            Destroy(gameObject);
        }else if (!other.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
        
        
        
        //transform.SetParent(other.transform,false);
    }
}