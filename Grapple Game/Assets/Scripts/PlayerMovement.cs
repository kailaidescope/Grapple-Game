using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public static bool grounded;
    public static bool mustCrouch;
    public static bool onLeftWall;
    public static bool onRightWall;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float deceleration;
    [SerializeField] private float jumpForce;
    [Range(0.1f,0.4f)] [SerializeField] private float endMovementParam;
    [SerializeField] private float crouchSpeedModifier;
    [SerializeField] private Collider2D crouchDisableCollider;
    [SerializeField] private float stopMovementTimeMax;

    private GameObject grapple;
    private Rigidbody2D grb;
    private Animator an;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private float direction;
    private float stopMovementTime;
    private string wallJumped;
    private bool jump;
    private bool crouch;
    private bool crouching;
    private bool stopMovement;
    private bool climbing;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name != "TilemapNoClip" && collision.gameObject.tag != "Player")
        {
            if (GrappleController.shoot)
            {
                GrappleController.returnToBody = true; ;
            }
            GrappleController.shoot = false;
        }
    }

    private void Start()
    {
        grapple = GameObject.Find("Grapple");
        grb = grapple.GetComponent<Rigidbody2D>();
        an = gameObject.GetComponent<Animator>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        sr = gameObject.GetComponent<SpriteRenderer>();

        stopMovementTime = 0f;
    }

    void Update()
    {
        direction = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
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
        if (Input.GetMouseButton(1) && !GrappleController.grappleOnBody)
        {
            GrappleController.lockLen = true;
        }
        if (Input.GetMouseButtonUp(1))
        {
            GrappleController.lockLen = false;
        }
    }

    private void FixedUpdate()
    {
        Vector2 grappleDirection = grb.position - rb.position;
        Vector2 prpGrappleDirection = new Vector2(grappleDirection.y, -grappleDirection.x);

        if (!stopMovement)
        {
            stopMovementTime++;
            if (grounded)
            {
                stopMovement = true;
            }
            else if (((direction > 0 && rb.velocity.x <= 0) || (direction < 0 && rb.velocity.x >= 0)) && wallJumped != null)
            {
                stopMovement = true;
            }
            else if(direction != 0 && wallJumped == null)
            {
                stopMovement = true;
            }
            else if(stopMovementTime >= stopMovementTimeMax)
            {
                stopMovement = true;
            }
        }
        else
        {
            stopMovementTime = 0f;
        }
        if (grounded)
        {
            wallJumped = null;
        }

        /*if (climbing)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f);
        }

        if ((!onRightWall && !onLeftWall) || grounded || ((direction == 0f) && (onRightWall || onLeftWall)));
        {
            climbing = false;
        }*/

        if (direction > 0f)
        {
            sr.flipX = false;
            if (!GrappleController.shoot)
            {
                if (!GrappleController.wasLockLen/* && !onRightWall*/)
                {
                    if (rb.velocity.x > 0f)
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
                        rb.velocity = new Vector2(rb.velocity.x + deceleration, rb.velocity.y);
                    }
                }
                else if (rb.position.y <= grb.position.y)
                {
                    rb.AddForce(prpGrappleDirection * acceleration / 15f, ForceMode2D.Impulse);
                }
                /*else if (onRightWall)
                {
                    climbing = true;
                }*/
            }
        }
        if (direction < 0f)
        {
            sr.flipX = true;
            if (!GrappleController.shoot)
            {
                if (!GrappleController.wasLockLen/* && !onLeftWall*/)
                {
                    if (rb.velocity.x < 0f)
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
                        rb.velocity = rb.velocity = new Vector2(rb.velocity.x - deceleration, rb.velocity.y);
                    }
                }
                else if (rb.position.y <= grb.position.y)
                {
                    rb.AddForce(-prpGrappleDirection * acceleration / 15f, ForceMode2D.Impulse);
                }
                /*else if (onLeftWall)
                {
                    climbing = true;
                }*/
            }
        }
        if(direction == 0f && !GrappleController.shoot && !GrappleController.wasLockLen && stopMovement)
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

        if (jump && grounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            jump = false;
        }
        else if (jump && onLeftWall && wallJumped != "left")
        {
            sr.flipX = false;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(0.6f, 0.8f) * jumpForce, ForceMode2D.Impulse);
            jump = false;
            wallJumped = "left";
            stopMovement = false;
        }
        else if (jump && onRightWall && wallJumped != "right")
        {
            sr.flipX = true;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(-0.6f, 0.8f) * jumpForce, ForceMode2D.Impulse);
            jump = false;
            wallJumped = "right";
            stopMovement = false;
        }
        else
        {
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
