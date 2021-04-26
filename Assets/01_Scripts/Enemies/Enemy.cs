using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    public bool aggrovated, canAttack, returningToSafeZone;

    public float enemyHealth,
        enemySpeed,
        enemyStrength,
        aggroRadius,
        aggroRange,
        attackRadius,
        attackDelay,
        damageKnockBackForce;

    public Vector3 startingPosition;
    public LayerMask visionLayerMask;
    public Transform playerTransform;
    public Rigidbody2D rb2d;
    public Animator enemyGraphicsAnim;
    public AudioClip attackSound, hurtSound;
    
    public Collider2D _collider2D;
    private Coroutine roamingCoroutine;
    private Vector3 currentRoamPoint;
    public AudioSource _audioSource;

    private PlayerCombat _playerCombat;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
        startingPosition = transform.position;
        _playerCombat = FindObjectOfType<PlayerCombat>();
        playerTransform = _playerCombat.transform;
        _collider2D = GetComponentInChildren<Collider2D>();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (enemySpeed.Equals(0)) return;
        roamingCoroutine = StartCoroutine(RoamToPointInSafeZone());
    }


    private void FixedUpdate()
    {
        // Check if we should be aggrovated
        bool closeEnoughToAggro = (Vector3.Distance(transform.position, playerTransform.position) < aggroRadius);
        bool closeEnoughToSafeZone = (Vector3.Distance(transform.position, startingPosition) < aggroRange);
        bool closeEnoughToAttack = (Vector3.Distance(transform.position, playerTransform.position) < attackRadius);
        aggrovated = closeEnoughToAggro && closeEnoughToSafeZone && !returningToSafeZone;
        if (_playerCombat !=null && _playerCombat.playerHealth.Equals(0))
        {
            return;
        }

        if(enemyGraphicsAnim!=null)
            enemyGraphicsAnim.SetBool("Attacking",aggrovated);
        
        
        if (aggrovated)
        {
            if(roamingCoroutine!=null)
                StopCoroutine(roamingCoroutine);
            
            // Make sure we dont get too close
            if (Vector3.Distance(transform.position, playerTransform.position) > attackRadius / 2)
            {
                rb2d.AddForce(enemySpeed * (playerTransform.position - transform.position).normalized);
            }
            
            if (closeEnoughToAttack && canAttack)
            {
                Attack();
            }
        }
        else if (!closeEnoughToSafeZone && !enemySpeed.Equals(0))
        {
            if(roamingCoroutine!=null)
                StopCoroutine(roamingCoroutine);
            
            returningToSafeZone = true;
            rb2d.AddForce(enemySpeed * (startingPosition - transform.position).normalized);
        }
        else if (returningToSafeZone && !enemySpeed.Equals(0))
        {
            if(roamingCoroutine!=null)
                StopCoroutine(roamingCoroutine);
            
            rb2d.AddForce(enemySpeed * (startingPosition - transform.position).normalized);
            if (Vector3.Distance(transform.position, startingPosition) < 0.1f)
            {
                returningToSafeZone = false;
                roamingCoroutine = StartCoroutine(RoamToPointInSafeZone());
            }
        }
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startingPosition, aggroRadius/2);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(currentRoamPoint, 2);
    }

    // Moves the enemy to a point in the safe zone
    private IEnumerator RoamToPointInSafeZone()
    {
        int pointsAttempted = 1;
        
        // Pick somewhere around the initial spawn point
        Vector3 randomPoint = (UnityEngine.Random.insideUnitCircle * aggroRadius);
        randomPoint += startingPosition;
        //Debug.Log("Picked initial point "+randomPoint);
        
        // Raycast to make sure we can get there
        while (!RoamPointValid(randomPoint) && pointsAttempted<100)
        {
            pointsAttempted++;
            // If we cant pick another place until we can
            randomPoint = (UnityEngine.Random.insideUnitCircle * (aggroRadius/2));
            randomPoint += startingPosition;
            //Debug.Log("Point invalid so picked new point of "+randomPoint);
        }

        if (pointsAttempted >= 100)
        {
            Debug.Log(gameObject.name+": Found no valid roam points");
            StopAllCoroutines();
            yield break;
        }
        
        currentRoamPoint = randomPoint;
        //Debug.Log("Roaming to "+randomPoint);

        // Move towards it
        while (Vector2.Distance(transform.position, currentRoamPoint) > 0.1f)
        {
            rb2d.AddForce((enemySpeed*0.2f) * (currentRoamPoint - transform.position).normalized);
            yield return new WaitForFixedUpdate();
        }
        //Debug.Log("Roam movement complete to "+currentRoamPoint);
        roamingCoroutine = StartCoroutine(RoamToPointInSafeZone());
    }

    private bool RoamPointValid(Vector3 randomPoint)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, randomPoint-transform.position,1000,visionLayerMask);

        // If it hits something...
        if (hit.collider != null)
        {
            return false;
        }

        return true;
    }

    public virtual void Attack()
    {
        Debug.Log(gameObject.name + " is attacking the player!");
        _playerCombat.TakeDamage(this);
        StartCoroutine(WaitThenCanAttack());
        _audioSource.PlayOneShot(attackSound);
    }

    public virtual void TakeDamage(Harpoon harpoon)
    {
        enemyHealth -= harpoon.projectileStrength;
        rb2d.AddForce((transform.position - harpoon.transform.position).normalized * damageKnockBackForce);
        _audioSource.PlayOneShot(hurtSound);
        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        _collider2D.enabled = false;
        //rb2d.gravityScale = 0.75f;
        //rb2d.AddTorque(UnityEngine.Random.Range(-500,500));
        //rb2d.angularDrag = 1f;
        if (enemyGraphicsAnim != null)
        {
            enemyGraphicsAnim.SetBool("Attacking",false);
        }
        
        // Fade out this object
        SpriteRenderer[] spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            Color currentColour = spriteRenderers[i].color;
            LeanTween.color(spriteRenderers[i].gameObject,
                new Color(currentColour.r, currentColour.g, currentColour.b, 0), 5f);
        }
        
        // Fade out all connected objects
        FixedJoint2D[] fixedJoints = GetComponentsInChildren<FixedJoint2D>();
        for (int i = 0; i < fixedJoints.Length; i++)
        {
            if (fixedJoints[i].connectedBody != null &&
                fixedJoints[i].connectedBody.GetComponentInChildren<SpriteRenderer>() != null)
            {
                Color currentColour = fixedJoints[i].connectedBody.GetComponentInChildren<SpriteRenderer>().color;
                LeanTween.color(fixedJoints[i].connectedBody.GetComponentInChildren<SpriteRenderer>().gameObject,
                    new Color(currentColour.r, currentColour.g, currentColour.b, 0), 5f);
            }
        }
        this.enabled = false;
        Destroy(gameObject,15f);
    }

    public IEnumerator WaitThenCanAttack()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackDelay);
        canAttack = true;
    }
}