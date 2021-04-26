using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float followSpeed = 1;
    public Vector3 offset;
    private Transform playerTransform;
    
    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPos = Vector2.Lerp(transform.position, playerTransform.position+offset, followSpeed*Time.deltaTime);
        transform.position = new Vector3(newPos.x,newPos.y,transform.position.z);
    }
}
