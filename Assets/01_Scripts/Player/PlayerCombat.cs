using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    public bool canShoot = true;
    public float playerHealth, shootingDelay = 0.1f, harpoonVelocity = 2000, damageKnockBackForce = 2000f;
    public GameObject harpoonPrefab, bloodPrefab;
    public SpriteRenderer loadedHarpoon;
    public Image hpImage;
    public CanvasGroup deathOverlay;
    public AudioClip shootSound, hurtSound, healSound;

    private float maxHealth;
    private Rigidbody2D rb2d;
    private InputHandler _inputHandler;
    private Collider2D _collider2D;
    private PlayerMovement _playerMovement;
    private Camera mainCam;
    private AudioSource _audioSource;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        _audioSource = GetComponent<AudioSource>();
        _playerMovement = GetComponent<PlayerMovement>();
        _collider2D = GetComponentInChildren<Collider2D>();
        _inputHandler = new InputHandler();
        mainCam = Camera.main;
        LeanTween.init();

        maxHealth = playerHealth;
        hpImage.fillAmount = 1;
    }

    private void OnEnable()
    {
        _inputHandler.Enable();
        _inputHandler.Player.Shoot.performed += ShootOnPerformed;
    }

    private void OnDisable()
    {
        _inputHandler.Player.Shoot.performed -= ShootOnPerformed;
        _inputHandler.Disable();
    }

    private void ShootOnPerformed(InputAction.CallbackContext obj)
    {
        if (!canShoot || playerHealth.Equals(0)) return;
        FireHarpoon();
    }

    private void FireHarpoon()
    {
        // Calculate angle to fire
        Vector3 mousePos = new Vector3(_inputHandler.Player.MousePosition.ReadValue<Vector2>().x,
            _inputHandler.Player.MousePosition.ReadValue<Vector2>().y);
        
        var pos = mainCam.WorldToScreenPoint(this.gameObject.transform.position);
        var dir = -(mousePos - pos);
        var angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90;
        //Debug.Log("Angle between "+ transform.position+" and "+mousePos+" is "+angle);
        
        // Instantiate harpoon
        GameObject harpoonObj = Instantiate(harpoonPrefab, transform.position, Quaternion.Euler(0, 0, angle-90));
        Harpoon currentHarpoon = harpoonObj.GetComponent<Harpoon>();
        Destroy(harpoonObj,7f);

        // Add forces
        currentHarpoon.rb2d.AddForce(currentHarpoon.transform.right*harpoonVelocity);
        _audioSource.PlayOneShot(shootSound);
        canShoot = false;
        loadedHarpoon.enabled = false;
        StartCoroutine(WaitThenCanShoot());
    }

    public void TakeDamage(Enemy attackingEnemy)
    {
        playerHealth -= attackingEnemy.enemyStrength;
        rb2d.AddForce((transform.position-attackingEnemy.transform.position).normalized*damageKnockBackForce);
        hpImage.fillAmount = 1*(playerHealth/maxHealth);
        _audioSource.PlayOneShot(hurtSound);
        if (playerHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float health)
    {
        playerHealth += health;
        _audioSource.PlayOneShot(healSound);
        if (playerHealth > maxHealth)
        {
            playerHealth = maxHealth;
        }
        hpImage.fillAmount = 1*(playerHealth/maxHealth);
    }

    private void Die()
    {
        _collider2D.enabled = false;
        _playerMovement.enabled = false;
        LeanTween.alphaCanvas(deathOverlay, 10, 1);
        this.enabled = false;
    }

    private IEnumerator WaitThenCanShoot()
    {
        yield return new WaitForSeconds(shootingDelay);
        loadedHarpoon.enabled = true;
        canShoot = true;
    }
}