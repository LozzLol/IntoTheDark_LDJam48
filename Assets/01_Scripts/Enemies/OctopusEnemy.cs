using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusEnemy : Enemy
{
    public GameObject inkProjectilePrefab;
    public Transform graphicsTransform;

    private void Update()
    {
        transform.up = -rb2d.velocity.normalized;
        //_collider2D.transform.up = -rb2d.velocity.normalized;
    }
    
    public override void Attack()
    {
        Debug.Log(gameObject.name + " is attacking the player!");
        FireInk();
        _audioSource.PlayOneShot(attackSound);
        StartCoroutine(WaitThenCanAttack());
    }

    private void FireInk()
    {
        // Calculate angle to fire
        var dir =playerTransform.position-transform.position;
        var angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
        //Debug.Log("Angle between "+ transform.position+" and "+mousePos+" is "+angle);
        
        // Instantiate ink object
        GameObject inkProjectile = Instantiate(inkProjectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
        EnemyProjectile enemyProjectile = inkProjectile.GetComponent<EnemyProjectile>();
        enemyProjectile.parentEnemy = this;
        SpriteRenderer inkProjectileRenderer = inkProjectile.GetComponentInChildren<SpriteRenderer>();
        LeanTween.color(inkProjectileRenderer.gameObject,
            new Color(inkProjectileRenderer.color.r, inkProjectileRenderer.color.g, inkProjectileRenderer.color.b, 0), 5f);
        Destroy(inkProjectile,5f);

        // Add forces
        enemyProjectile.rb2d.AddForce(enemyProjectile.transform.right*enemyProjectile.projectileVelocity);
    }
}
