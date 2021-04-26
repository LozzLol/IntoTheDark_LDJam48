using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float movementSpeed;


    [SerializeField]
    private Vector2 _inputVector;


    private Rigidbody2D rb2d;

    private InputHandler _inputHandler;

    private void Awake()
    {
        _inputHandler = new InputHandler();
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        rb2d.AddForce(_inputVector*movementSpeed);
    }

    private void OnEnable()
    {
        _inputHandler.Enable();
        _inputHandler.Player.VerticalMovement.performed += VerticalMovementOnPerformed;
        _inputHandler.Player.VerticalMovement.canceled += VerticalMovementOnCanceled;
        _inputHandler.Player.HorizontalMovement.performed += HorizontalMovementOnPerformed;
        _inputHandler.Player.HorizontalMovement.canceled += HorizontalMovementOnCanceled;
    }

    private void OnDisable()
    {
        _inputHandler.Player.VerticalMovement.performed -= VerticalMovementOnPerformed;
        _inputHandler.Player.VerticalMovement.canceled -= VerticalMovementOnCanceled;
        _inputHandler.Player.HorizontalMovement.performed -= HorizontalMovementOnPerformed;
        _inputHandler.Player.HorizontalMovement.canceled -= HorizontalMovementOnCanceled;
        _inputHandler.Disable();
    }

    private void HorizontalMovementOnPerformed(InputAction.CallbackContext obj)
    {
        _inputVector = new Vector2(obj.ReadValue<float>(), _inputVector.y);
    }

    private void HorizontalMovementOnCanceled(InputAction.CallbackContext obj)
    {
        _inputVector = new Vector2(0, _inputVector.y);
    }


    private void VerticalMovementOnPerformed(InputAction.CallbackContext obj)
    {
        _inputVector = new Vector2(_inputVector.x, obj.ReadValue<float>());
    }

    private void VerticalMovementOnCanceled(InputAction.CallbackContext obj)
    {
        _inputVector = new Vector2(_inputVector.x, 0);
    }
}