using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RightWallCheck : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MoveableBlock")
        {
            GrappleController.blockOnPlayer = true;
        }
        else if (collision.gameObject.name != "TilemapNoClip" && collision.gameObject.tag != "Player" && collision.gameObject.name != "Grapple")
        {
            PlayerMovement.onRightWall = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MoveableBlock")
        {
            GrappleController.blockOnPlayer = true;
        }
        else if (collision.gameObject.name != "TilemapNoClip" && collision.gameObject.tag != "Player" && collision.gameObject.name != "Grapple")
        {
            PlayerMovement.onRightWall = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "MoveableBlock")
        {
            GrappleController.blockOnPlayer = false;
        }
        else if (collision.gameObject.name != "TilemapNoClip" && collision.gameObject.tag != "Player" && collision.gameObject.name != "Grapple")
        {
            PlayerMovement.onRightWall = false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
