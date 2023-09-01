using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody playerRb;
    private GameObject focalPoint;
    public GameObject powerupIndicator;
    public GameObject missile;
    public float speed = 1.0f;
    private float powerupStrength = 100;
    private float missileFrequency = 0.1f;
    private float jumpForce = 10;
    private float smashForce = 30;

    private bool isOnGround = true;
    private bool hasBouncyPowerup = false;
    private bool hasJumpPowerup = true;
    private bool waitingForExplosion = false;



    private class PowerUpType
    {
        public const string bouncy = "BOUNCY";
        public const string missile = "MISSILE";
        public const string smash = "SMASH";
        //Don't forget to add new powerup type to below list
        public static readonly IList<String> powerupTagList = 
            new ReadOnlyCollection<string>(new List<String> {
                "Powerup", "Powerup2", "Powerup3"
            });
    }

    // Start is called before the first frame update
    void Start()
    {
        playerRb = GetComponent<Rigidbody>();
        focalPoint = GameObject.Find("FocalPoint");
    }

    // Update is called once per frame
    void Update()
    {
        
        float horizontalInput = Input.GetAxis("Vertical");
        //Move the powerup Indicator 
        playerRb.AddForce(focalPoint.transform.forward * horizontalInput * speed);
        
        powerupIndicator.transform.position = transform.position + new Vector3(0, -0.5f, 0);

        if (hasJumpPowerup)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (isOnGround)
                {
                    Debug.Log("Jumping");
                    playerRb.velocity = (Vector3.up * jumpForce);
                    waitingForExplosion = true;
                    isOnGround = false;
                }
                else
                {
                    Debug.Log("Smashing");
                    playerRb.velocity = (Vector3.down * smashForce);
                }

            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //If powerup...destroy other object, start routine for that powerup
        string otherTag = other.tag;
        if (PowerUpType.powerupTagList.Contains(other.tag) )
        {    
            Destroy(other.gameObject);
            powerupIndicator.SetActive(true);

            IEnumerator co = null;
            switch (otherTag)
            {
                case "Powerup":
                    co = PowerupRoutine(PowerUpType.bouncy);
                    break;
                case "Powerup2":
                    co = PowerupRoutine(PowerUpType.missile);
                    break;
                case "Powerup3":
                    co = PowerupRoutine(PowerUpType.smash);
                    break;
            }
            if (co != null)
            {
                StartCoroutine(co);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && hasBouncyPowerup)
        {
            Rigidbody enemyRb = collision.gameObject.GetComponent<Rigidbody>();
            Vector3 awayFromPlayer = enemyRb.position - transform.position;
            enemyRb.AddForce(awayFromPlayer * powerupStrength, ForceMode.Impulse);
            Debug.Log("Collided with " +  collision.gameObject.name + "while powerup = " + hasBouncyPowerup);
            if (waitingForExplosion)
            {
                Explosion();
            }
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (hasJumpPowerup && waitingForExplosion)
            {
                Explosion();
            }
            isOnGround = true;
        }
    }

    IEnumerator PowerupRoutine(string powerUpPickedUp)
    {
        if (powerUpPickedUp.Equals(PowerUpType.bouncy)) 
        {
            hasBouncyPowerup = true;
            yield return new WaitForSeconds(5);
            hasBouncyPowerup = false;
        }
        if (powerUpPickedUp.Equals(PowerUpType.missile)) 
        {
            InvokeRepeating("ShootMissiles", 0, missileFrequency);
            yield return new WaitForSeconds(3);
            CancelInvoke("ShootMissiles");
        }
        if (powerUpPickedUp.Equals(PowerUpType.smash))
        {
            hasJumpPowerup = true;
            yield return new WaitForSeconds(5);
            hasJumpPowerup = false;
        }

        powerupIndicator.SetActive(false);
    }

    private void ShootMissiles()
    {
        //Missile logic
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length; i++)
        {
            Rigidbody enemyRb = enemies[i].GetComponent<Rigidbody>(); ;
            Instantiate(missile, playerRb.position, Quaternion.LookRotation(enemyRb.position - playerRb.position));
        }
    }

    private void pushUp(float force)
    {
        playerRb.AddForce(Vector3.up * 100, ForceMode.Acceleration);
    }

    private void Explosion()
    {
        Debug.Log("Boom!");
        waitingForExplosion = false;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemies.Length;i++)
        {
            Rigidbody enemyRb = enemies[i].GetComponent<Rigidbody>();
            enemyRb.AddExplosionForce(5000, transform.position, 10);
        }
        
    }
}