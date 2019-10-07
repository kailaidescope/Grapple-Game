using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static bool grounded;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float jumpForce;
    [Range(0.1f,0.4f)] [SerializeField] private float endMovementParam;
    [SerializeField] private float crouchSpeedModifier;
    [SerializeField] private Collider2D crouchDisableCollider;
    [SerializeField] private Collider2D crouchTrigger;

    private GameObject grapple;
    private Rigidbody2D grb;
    private Animator an;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float direction;
    private bool jump;
    private bool crouch;
    private bool crouching;
    private bool mustCrouch;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MoveableBlock")
        {
            GrappleController.blockOnPlayer = true;
        }
        if (collision.gameObject.name != "TilemapNoClip")
        {
            grounded = true;
            if (GrappleController.shoot)
            {
                GrappleController.pull = true;
            }
            GrappleController.shoot = false;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MoveableBlock")
        {
            GrappleController.blockOnPlayer = true;
        }
        if (collision.gameObject.name != "TilemapNoClip")
        {
            grounded = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "MoveableBlock")
        {
            GrappleController.blockOnPlayer = false;
        }
        grounded = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Player")
        {
            mustCrouch = true;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name != "Player")
        {
            mustCrouch = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.name != "Player")
        {
            mustCrouch = false;
        }
    }

    private void Start()
    {
        grapple = GameObject.Find("Grapple");
        grb = grapple.GetComponent<Rigidbody2D>();
        an = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        direction = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            jump = true;
        }

        if (Input.GetKeyDown("s"))
        {
            crouch = true;
        }
        else if (Input.GetKeyUp("s"))
        {
            crouch = false;
        }

        if (Input.GetMouseButtonDown(0))
        {
            GrappleController.shoot = true;
        }
        if (Input.GetMouseButton(1) && !GrappleController.grappleOnBody && rb.position.y <= grb.position.y)
        {
            GrappleController.lockLen = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            GrappleController.lockLen = false;
        }
        if (Input.GetMouseButtonDown(2))
        {
            GrappleController.pull = true;
        }
    }

    private void FixedUpdate()
    {
        Vector2 grappleDirection = grb.position - rb.position;
        Vector2 prpGrappleDirection = new Vector2(grappleDirection.y, -grappleDirection.x);

        if (direction > 0f)
        {
            sr.flipX = false;
            if (!GrappleController.lockLen)
            {
                if (rb.velocity.x < maxSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x + acceleration, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
                }
            }
            else
            {
                rb.AddForce(prpGrappleDirection * acceleration/10f, ForceMode2D.Impulse);
            }
        }
        if (direction < 0f)
        {
            sr.flipX = true;
            if (!GrappleController.lockLen)
            {
                if (rb.velocity.x > -maxSpeed)
                {
                    rb.velocity = new Vector2(rb.velocity.x - acceleration, rb.velocity.y);
                }
                else
                {
                    rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
                }
            }
            else
            {
                rb.AddForce(-prpGrappleDirection * acceleration/10f, ForceMode2D.Impulse);
            }
        }
        if(direction == 0f && !GrappleController.shoot && !GrappleController.lockLen && !GrappleController.wasLockLen)
        {
            if(rb.velocity.x > endMovementParam)
            {
                rb.velocity = new Vector2(rb.velocity.x - deceleration, rb.velocity.y);
            }
            if (rb.velocity.x < -endMovementParam)
            {
                rb.velocity = new Vector2(rb.velocity.x + deceleration, rb.velocity.y);
            }
            if (rb.velocity.x > -endMovementParam || rb.velocity.x < endMovementParam)
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
        }

        if (jump)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jump = false;
        }

        if (crouch && !crouching || mustCrouch && !crouching)
        {
            maxSpeed *= crouchSpeedModifier;
            crouchDisableCollider.enabled = false;
            crouching = true;
        }
        if(!crouch && !mustCrouch && crouching)
        {
            maxSpeed /= crouchSpeedModifier;
            crouchDisableCollider.enabled = true;
            crouching = false;
        }
    }
}
