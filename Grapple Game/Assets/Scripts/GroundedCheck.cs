using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MoveableBlock")
        {
            GrappleController.blockOnPlayer = true;
        }
        if (collision.gameObject.name != "TilemapNoClip" && collision.gameObject.tag != "Player" && collision.gameObject.name != "Grapple")
        {
            PlayerMovement.grounded = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MoveableBlock")
        {
            GrappleController.blockOnPlayer = true;
        }
        if (collision.gameObject.name != "TilemapNoClip" && collision.gameObject.tag != "Player" && collision.gameObject.name != "Grapple")
        {
            PlayerMovement.grounded = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MoveableBlock")
        {
            GrappleController.blockOnPlayer = false;
        }
        if (collision.gameObject.name != "TilemapNoClip" && collision.gameObject.tag != "Player" && collision.gameObject.name != "Grapple")
        {
            PlayerMovement.grounded = false;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
