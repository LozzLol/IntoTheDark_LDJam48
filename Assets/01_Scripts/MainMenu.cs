using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;

public class MainMenu : MonoBehaviour
{

    public bool skipIntro;
    
    public Animator playerAnim, graphicsAnim;
    public PlayerCombat playerCombat;
    public PlayerMovement playerMovement;
    public PlayerGraphics playerGraphics;
    public Collider2D playerCollider;
    public CameraController cameraController;
    public Light2D playerLight;

    private CanvasGroup _canvasGroup;
    private InputHandler _inputHandler;

    private void Awake()
    {
        _canvasGroup = GetComponentInChildren<CanvasGroup>();
        _inputHandler = new InputHandler();
        Time.timeScale = 1;
        if (skipIntro)
        {
            playerCombat.transform.localPosition = new Vector3(0.1f, -1.5f, 0);
            cameraController.enabled = true;
            _canvasGroup.alpha = 0;
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.interactable = false;
            playerCollider.enabled = true;
            playerCombat.enabled = true;
            playerMovement.enabled = true;
            playerAnim.enabled = false;
            playerGraphics.enabled = true;
        }
    }

    private void OnEnable()
    {
        _inputHandler.Enable();
        _inputHandler.Player.Restart.performed += RestartGame;
    }

    private void RestartGame(InputAction.CallbackContext obj)
    {
        Application.LoadLevel(0);
    }

    private void OnDisable()
    {
        _inputHandler.Player.Restart.performed -= RestartGame;
        _inputHandler.Disable();
    }

    public void StartGame()
    {
        StartCoroutine(GameStartAnimation());
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator GameStartAnimation()
    {
        cameraController.enabled = true;
        playerAnim.SetTrigger("Start");
        graphicsAnim.enabled = false;
        LeanTween.alphaCanvas(_canvasGroup, 0, 0.5f);
        yield return new WaitForSeconds(2);
        playerGraphics.enabled = true;
        _canvasGroup.blocksRaycasts = false;
        _canvasGroup.interactable = false;
        playerCollider.enabled = true;
        playerCombat.enabled = true;
        playerMovement.enabled = true;
        playerAnim.enabled = false;
        graphicsAnim.enabled = true;
        playerLight.enabled = true;
    }
}
