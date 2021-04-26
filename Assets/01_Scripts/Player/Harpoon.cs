using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Harpoon : MonoBehaviour
{
    public float projectileStrength;
    public Rigidbody2D rb2d;

    private Collider2D thisCollider;

    private void Awake()
    {
        thisCollider = GetComponentInChildren<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(gameObject+" collided with "+other.gameObject);
        if (!other.CompareTag("Enemy"))
        {
            rb2d.isKinematic = true;
            rb2d.velocity = Vector2.zero;
            rb2d.freezeRotation = true;
            thisCollider.enabled = false;
        }
        else
        {
            Enemy enemy = other.GetComponentInParent<Enemy>();
            GameObject bloodObj = Instantiate(FindObjectOfType<PlayerCombat>().bloodPrefab,transform.position,transform.rotation);
            Destroy(bloodObj,3);
            enemy.TakeDamage(this);
            if (enemy.GetType() == typeof(PiranhaEnemy))
            {
                FixedJoint2D fixedJoint2D = enemy.gameObject.AddComponent<FixedJoint2D>();
                fixedJoint2D.connectedBody = rb2d;
            }else if (enemy.GetType() == typeof(AnemoneEnemy))
            {
                thisCollider.enabled = false;
                FixedJoint2D fixedJoint2D = enemy.gameObject.AddComponent<FixedJoint2D>();
                fixedJoint2D.connectedBody = rb2d;
            }
            else if (enemy.GetType() == typeof(OctopusEnemy))
            {
                thisCollider.enabled = false;
                FixedJoint2D fixedJoint2D = enemy.gameObject.AddComponent<FixedJoint2D>();
                fixedJoint2D.connectedBody = rb2d;
            }
            else if (enemy.GetType() == typeof(BarracudaEnemy))
            {
                thisCollider.enabled = false;
                FixedJoint2D fixedJoint2D = enemy.gameObject.AddComponent<FixedJoint2D>();
                fixedJoint2D.connectedBody = rb2d;
            }
            //transform.SetParent(other.collider.transform,true);

            //enemy.transform.SetParent(transform,true);
        }
        
        //transform.SetParent(other.transform,false);
    }
}
