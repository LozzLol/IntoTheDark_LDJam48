using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OxygenTank : MonoBehaviour
{
    public float healthProvided, floatRadius, floatSpeed;
    public Vector3 startingPosition;
    public LayerMask visionLayerMask;
    public Rigidbody2D rb2d;
    
    private Coroutine roamingCoroutine;
    private Vector3 currentRoamPoint;
    
    private void Awake()
    {
        startingPosition = transform.position;
        rb2d = GetComponent<Rigidbody2D>();
        rb2d.AddTorque(UnityEngine.Random.Range(0f,1f));
    }
    
    private void Start()
    {
        roamingCoroutine = StartCoroutine(RoamToPointInSafeZone());
    }
    
    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(startingPosition, floatRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(currentRoamPoint, 1);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.CompareTag("Player"))
        {
            other.collider.GetComponentInParent<PlayerCombat>().Heal(healthProvided);
            Destroy(gameObject);
        }
    }

    // Moves the enemy to a point in the safe zone
    private IEnumerator RoamToPointInSafeZone()
    {
        int pointsAttempted = 1;
        // Pick somewhere around the initial spawn point
        Vector3 randomPoint = (UnityEngine.Random.insideUnitCircle * floatRadius);
        randomPoint += startingPosition;
        //Debug.Log("Picked initial point "+randomPoint);
        
        // Raycast to make sure we can get there
        while (!RoamPointValid(randomPoint) && pointsAttempted<50)
        {
            pointsAttempted++;
            // If we cant pick another place until we can
            randomPoint = (UnityEngine.Random.insideUnitCircle * (floatRadius/2));
            randomPoint += startingPosition;
            //Debug.Log("Point invalid so picked new point of "+randomPoint);
        }

        if (pointsAttempted >= 50)
        {
            Debug.Log(gameObject.name+": Found no valid roam points");
            StopAllCoroutines();
            yield break;
        }

            currentRoamPoint = randomPoint;
        Debug.Log("Roaming to "+randomPoint);

        // Move towards it
        while (Vector2.Distance(transform.position, currentRoamPoint) > 0.001f)
        {
            rb2d.AddForce((floatSpeed) * (currentRoamPoint - transform.position).normalized);
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("Roam movement complete to "+currentRoamPoint);
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
}
