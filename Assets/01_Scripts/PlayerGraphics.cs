using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGraphics : MonoBehaviour
{
    public Transform playerGraphicsTransform, armsTransform, neckTransform;

    private Vector2 initialPlayerGraphicScale;
    private InputHandler _inputHandler;
    private Camera mainCam;

    private void Awake()
    {
        _inputHandler = new InputHandler();
        mainCam = Camera.main;
        initialPlayerGraphicScale = playerGraphicsTransform.transform.localScale;
    }

    private void Update()
    {
        UpdateGraphicsOrientation();
    }

    private void OnEnable()
    {
        _inputHandler.Enable();
    }
    
    private void OnDisable()
    {
        _inputHandler.Disable();
    }

    private void UpdateGraphicsOrientation()
    {
        // Calculate angle between mouse and player
        Vector3 mousePos = new Vector3(_inputHandler.Player.MousePosition.ReadValue<Vector2>().x,
            _inputHandler.Player.MousePosition.ReadValue<Vector2>().y);
        
        var pos = mainCam.WorldToScreenPoint(this.gameObject.transform.position);
        var dir = -(mousePos - pos);
        var angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90;

        
        Debug.Log("Arm angle is "+angle);
        if (angle < 0 && angle > -180)
        {
            playerGraphicsTransform.localScale = new Vector3(-initialPlayerGraphicScale.x, initialPlayerGraphicScale.y);
            //playerGraphicsTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle-90));
            armsTransform.localScale = new Vector3(-1, 1);
            armsTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            neckTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, 270-angle));
        }
        else
        {
            playerGraphicsTransform.localScale = new Vector3(initialPlayerGraphicScale.x, initialPlayerGraphicScale.y);
            //playerGraphicsTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle-90));
            armsTransform.localScale = new Vector3(1, 1);
            armsTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            neckTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle-90));
        }
    }
}
