using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D prb;
    private Rigidbody2D rb;

    private bool playerIn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerIn = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerIn = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            playerIn = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        prb = player.GetComponent<Rigidbody2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        playerIn = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!playerIn)
        {
            rb.velocity = prb.velocity * 1.2f;
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}
