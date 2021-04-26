using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnemoneTentacle : MonoBehaviour
{
    public Transform[] rotatableJoints;
    private Renderer thisRenderer;

    private void Awake()
    {
        InvokeRepeating("RotateRandomly",0,2);
        thisRenderer = GetComponentInChildren<Renderer>();
    }

    private void RotateRandomly()
    {
        // Check if we are in the viewport
        if (!thisRenderer.isVisible) return;
        
        for (int i = 0; i < rotatableJoints.Length; i++)
        {
            LeanTween.cancel(rotatableJoints[i].gameObject);
            LeanTween.rotateLocal(rotatableJoints[i].gameObject, new Vector3(0,0,UnityEngine.Random.Range(-90f, 90f)), UnityEngine.Random.Range(3f, 5f));
        }
    }
}
