using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchCheck : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Player" && collision.gameObject.tag != "MainCamera")
        {
            PlayerMovement.mustCrouch = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Player" && collision.gameObject.tag != "MainCamera")
        {
            PlayerMovement.mustCrouch = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Player" && collision.gameObject.tag != "MainCamera")
        {
            PlayerMovement.mustCrouch = false;
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
