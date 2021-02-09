using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour{
    [Header("Enemy Stats")]
    [SerializeField] float health = 100;
    [SerializeField] int pointsPerKill = 150;
    float enemyPositionX;

    [Header("Shooting")]
    float shotCounter;
    [SerializeField] float minTimeBetweenShots = 0.2f;
    [SerializeField] float maxTimeBetweenShots = 3.0f;
    [SerializeField] float projectileSpeed = 10.0f;
    [SerializeField] GameObject enemyLaserPrefab;

    [Header("Enemy Explosion")]
    [SerializeField] GameObject explosionEffectParticle;
    [SerializeField] float explosionLastingTimer = 0.8f;

    [Header("Health")]
    [SerializeField] GameObject healthItem;

    [Header("Audio")]
    [SerializeField] AudioClip laserSound;
    [SerializeField] AudioClip deathSound;
    [SerializeField] [Range(0, 1)] float laserSoundVolume = 0.5f;
    [SerializeField] [Range(0, 1)] float deathSoundVolume = 0.5f;
    

    //cached reference
    GameSession addPointsToScore;


    // Start is called before the first frame update
    void Start(){
        shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
    }

    // Update is called once per frame
    void Update(){
        CountdownAndShoot();
    }

    private void CountdownAndShoot() {
        shotCounter -= Time.deltaTime;
        if (shotCounter <= 0f) {
            Fire();
            shotCounter = UnityEngine.Random.Range(minTimeBetweenShots, maxTimeBetweenShots);
        }
    }

    public void Fire() {
        GameObject laserInstance = Instantiate(enemyLaserPrefab, transform.position, Quaternion.identity) as GameObject;
        laserInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(0.0f, -projectileSpeed);
        AudioSource.PlayClipAtPoint(laserSound, Camera.main.transform.position, laserSoundVolume);
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
        health -= damageDealer.GetDamage();
        damageDealer.Hit();
        if (health <= 0) {
            FindObjectOfType<GameSession>().AddToScore(pointsPerKill);
            AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, deathSoundVolume);
            ExplosionEffect();
            SpawnHealthItem();
            Destroy(gameObject);
        }
    }

    public void ExplosionEffect() {
        GameObject explosionEffect = Instantiate(explosionEffectParticle, transform.position, Quaternion.identity) as GameObject;
        Destroy(explosionEffect, explosionLastingTimer);
    }

    public void SpawnHealthItem() {
        float randomNumber = UnityEngine.Random.Range(0, 21);
        if (randomNumber == 0) {
            GameObject health = Instantiate(healthItem, transform.position, Quaternion.identity) as GameObject;
        }
    }
}
