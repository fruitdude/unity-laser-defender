using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour{
    [Header("Player")]
    [SerializeField] float moveSpeed = 10.0f;
    [SerializeField] float padding = 0.5f;
    [SerializeField] float playerHealth = 300;
    float damagePerHit = 100;

    [Header("Projectile")]
    [SerializeField] float projectileSpeed = 10.0f;
    [SerializeField] float projectileFiringPeriod;
    [SerializeField] GameObject laserPrefab;

    [Header("Audio")]
    [SerializeField] AudioClip laserSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0,1)] float laserSoundVolume = 0.5f;
    [SerializeField] [Range(0,1)] float deathSoundVolume = 0.5f;

    [Header("Explosion")]
    [SerializeField] GameObject explosionEffectParticle;
    [SerializeField] float explosionLastingTimer = 0.8f;

    Coroutine firingCoroutine;

    float xMin;
    float xMax;
    float yMin;
    float yMax;

    // Start is called before the first frame update
    void Start(){
        SetUpMoveBoundaries();
    }

    // Update is called once per frame
    void Update(){
        Movement();
        Fire();
    }

    private void Fire() {
        if (Input.GetButtonDown("Fire1")) {
            firingCoroutine = StartCoroutine(FireContinuously());
        }

        if (Input.GetButtonUp("Fire1")) {
            StopCoroutine(firingCoroutine);
        }
        
    }

    IEnumerator FireContinuously() {
        while (true) {
            GameObject laserInstance = Instantiate(laserPrefab, transform.position, Quaternion.identity) as GameObject;
            laserInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, projectileSpeed);
            AudioSource.PlayClipAtPoint(laserSound, Camera.main.transform.position, laserSoundVolume);
            yield return new WaitForSeconds(projectileFiringPeriod);
            
        }
    }

    private void Movement() {
        var deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * moveSpeed;
        var deltaY = Input.GetAxis("Vertical") * Time.deltaTime * moveSpeed;

        var newXPosition = Mathf.Clamp(transform.position.x + deltaX, xMin, xMax);
        var newYPosition = Mathf.Clamp(transform.position.y + deltaY, yMin, yMax);
        transform.position = new Vector2(newXPosition, newYPosition);
    }

    private void SetUpMoveBoundaries() {
        Camera gameCamera = Camera.main;
        xMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).x + padding;
        xMax = gameCamera.ViewportToWorldPoint(new Vector3(1, 0, 0)).x - padding;
        yMin = gameCamera.ViewportToWorldPoint(new Vector3(0, 0, 0)).y + padding;
        yMax = gameCamera.ViewportToWorldPoint(new Vector3(0, 1, 0)).y - new Vector3(0, 0.5f, 0).y;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();
        //if the gameobject does not have a damagedealer script it will return and wont do anything anymore
        if (!damageDealer) {
            return;
        } 
        ProccesHit(damageDealer);
    }

    private void ProccesHit(DamageDealer damageDealer) {
        playerHealth -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (playerHealth <= 0) {
            playerDies();
        }
    }

    public void playerDies() {
        FindObjectOfType<Level>().LoadGameOver(); //this way we can call a function from another script
        ExplosionEffect();
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
        Destroy(gameObject);
    }

    public void ExplosionEffect() {
        GameObject explosionEffect = Instantiate(explosionEffectParticle, transform.position, Quaternion.identity) as GameObject;
        Destroy(explosionEffect, explosionLastingTimer);
    }

    public float GetPlayerHealth() {
        return playerHealth;
    }

    public void PlayerGetsHealed() {
        playerHealth += 100;
    }
}
