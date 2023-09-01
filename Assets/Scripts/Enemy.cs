using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody enemyRb;
    private GameObject player;
    private Rigidbody playerRb;
    protected float speed = 1.0f;
    public float strength = 1;

    private Vector3 ZeroY = new Vector3(1, 0, 1);

    // Start is called before the first frame update
    protected void Start()
    {
        enemyRb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        playerRb = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected void Update()
    {
        
        Vector3 lookDirection = (player.transform.position - transform.position).normalized;
        lookDirection.y = 0; //Don't follow upwards.
        //Debug.Log(enemyRb.velocity);
        enemyRb.AddForce(lookDirection * speed);
        
        
        //enemyRb.velocity = (lookDirection * speed);
        
        if (transform.position.y < -10)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector3 towardPlayer = (playerRb.position - transform.position).normalized;
            playerRb.AddForce(towardPlayer * strength, ForceMode.Impulse);
        }
    }
}
