using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    public CanvasGroup winCanvasGroup;
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("End triggered by "+other.gameObject.name);
        if (!other.CompareTag("Player")) return;
        Time.timeScale = 0;
        LeanTween.alphaCanvas(winCanvasGroup, 1, 1).setIgnoreTimeScale(true);
    }
}
