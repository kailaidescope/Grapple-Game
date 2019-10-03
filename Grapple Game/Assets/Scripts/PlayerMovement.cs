using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float jumpForce;
    [Range(0.1f,0.4f)] [SerializeField] private float endMovementParam;
    [SerializeField] private float crouchSpeedModifier;
    [SerializeField] private Collider2D crouchDisableCollider;

    private Animator an;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float direction;
    private bool jump;
    private bool crouch;
    private bool crouching;
    private bool mustCrouch;
    private bool grounded;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        grounded = true;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        grounded = true;
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        grounded = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        mustCrouch = true;
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        mustCrouch = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        mustCrouch = false;
    }

    private void Start()
    {
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
    }

    private void FixedUpdate()
    {
        if(direction > 0f)
        {
            sr.flipX = false;

            if(rb.velocity.x < maxSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x + acceleration, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(maxSpeed, rb.velocity.y);
            }
        }
        if (direction < 0f)
        {
            sr.flipX = true;

            if (rb.velocity.x > -maxSpeed)
            {
                rb.velocity = new Vector2(rb.velocity.x - acceleration, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(-maxSpeed, rb.velocity.y);
            }
        }
        if(direction == 0f)
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

        if (crouch && !crouching)
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
