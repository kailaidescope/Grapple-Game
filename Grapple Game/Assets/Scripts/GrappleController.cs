using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleController : MonoBehaviour
{
    public static bool shoot;
    public static bool lockLen;
    public static bool wasLockLen;
    public static bool pull;
    public static bool inPlayer;
    public static bool blockOnPlayer;
    public static bool grappleOnBody;
    public static bool returnToBody;
    public static bool inWall;

    [SerializeField] private float offset;
    [SerializeField] private float grappleSpeed;
    [SerializeField] private float grapplePullMod;
    [SerializeField] private float playerPullMod;
    [SerializeField] private float blockPullMod;
    [SerializeField] private float maxTimeOut;

    private GameObject player;
    private GameObject collectTrigger;
    private GameObject moveableBlock;
    private GameObject rope;
    private Collider2D headCollider;
    private Rigidbody2D rb;
    private Rigidbody2D prb;
    private DistanceJoint2D grappleJoint;
    private Vector2 mousePos;
    private Vector2 mouseDirection;
    private Vector2 playerDirection;
    private float time;
    private bool inMoveableWall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "MoveableBlock" && !pull)
        {
            inMoveableWall = true;
            moveableBlock = collision.gameObject;
            transform.parent = moveableBlock.transform;
            collectTrigger.SetActive(true);
        }
        if (collision.gameObject.tag != "Player" && collision.gameObject.name != "TilemapNoClip" && collision.gameObject.tag != "Grapple")
        {
            inWall = true;
            collectTrigger.SetActive(true);
            rb.velocity = Vector2.zero;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" && collision.gameObject.name != "TilemapNoClip" && collision.gameObject.tag != "Grapple")
        {
            inWall = true;
            collectTrigger.SetActive(true);
            if (!pull)
            {
                rb.velocity = Vector2.zero;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "MoveableBlock")
        {
            inMoveableWall = false;
        }
        if (collision.gameObject.tag != "Player" && collision.gameObject.name != "TilemapNoClip" && collision.gameObject.tag != "MainCamera" && collision.gameObject.tag != "Grapple")
        {
            inWall = false;
        }
    }

    void Start()
    {
        player = GameObject.Find("Player");

        collectTrigger = GameObject.Find("GrappleCollectTrigger");
        collectTrigger.SetActive(false);

        moveableBlock = null;

        rope = GameObject.Find("Rope");

        headCollider = gameObject.GetComponent<Collider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        prb = player.GetComponent<Rigidbody2D>();
        grappleJoint = gameObject.GetComponent<DistanceJoint2D>();

        GetDirections();

        grappleOnBody = true;
        shoot = false;
        lockLen = false;
        wasLockLen = false;
        pull = false;
        blockOnPlayer = false;
        inWall = false;
        inMoveableWall = false;

        time = 0f;
    }

    void FixedUpdate()
    {
        GetDirections();
        LockLength();
        ShootGrapple();
        PullGrapple();
        PointToCursor();

        if (grappleOnBody)
        {
            transform.parent = player.transform;
            inWall = false;
            inMoveableWall = false;
            headCollider.enabled = false;
            if (shoot)
            {
                shoot = false;
            }
        }
        else
        {
            headCollider.enabled = true;
        }

        if(!grappleOnBody && !inWall || pull)
        {
            time += Time.fixedDeltaTime;
        }
        else
        {
            time = 0f;
        }
        ReturnGrappleToBody();
    }

    void GetDirections()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseDirection = mousePos - rb.position;
        mouseDirection.Normalize();
        playerDirection = prb.position - rb.position;
        playerDirection.Normalize();
    }

    void ShootGrapple()
    {
        if (shoot && grappleOnBody)
        {
            grappleOnBody = false;
            rb.velocity = new Vector2(mouseDirection.x * grappleSpeed + rb.velocity.x, mouseDirection.y * grappleSpeed + rb.velocity.y);
            shoot = false;
            transform.parent = null;
        }
        else if (shoot && !grappleOnBody && inWall)
        {
            prb.velocity = new Vector2(-playerDirection.x * grappleSpeed / playerPullMod + prb.velocity.x, -playerDirection.y * grappleSpeed / playerPullMod + prb.velocity.y);
        }
        else
        {
            shoot = false;
        }
    }
    void PullGrapple()
    {
        if (pull && !grappleOnBody && inMoveableWall)
        {
            rb.velocity = new Vector2(playerDirection.x * grappleSpeed / blockPullMod + rb.velocity.x, playerDirection.y * grappleSpeed / blockPullMod + rb.velocity.y);
            moveableBlock.GetComponent<Rigidbody2D>().velocity = new Vector2(playerDirection.x * grappleSpeed / blockPullMod + rb.velocity.x, playerDirection.y * grappleSpeed / blockPullMod + rb.velocity.y);
        }
        else if (pull && !grappleOnBody)
        {
            rb.velocity = new Vector2(playerDirection.x * grappleSpeed/grapplePullMod + rb.velocity.x, playerDirection.y * grappleSpeed/grapplePullMod + rb.velocity.y);
        }
        else
        {
            pull = false;
        }
    }
    void LockLength()
    {
        if (lockLen && !(prb.position.y <= rb.position.y + 2f) && !wasLockLen)
        {
            pull = true;
        }
        else if (lockLen && !pull && inWall)
        {
            if (!wasLockLen)
            {
                wasLockLen = true;
                grappleJoint.enabled = true;
                grappleJoint.distance = Vector2.Distance(rb.position, prb.position);
            }
            pull = false;
            shoot = false;
        }
        else if (!PlayerMovement.grounded && wasLockLen)
        {
            grappleJoint.enabled = false;
            pull = true;
        }
        else if(PlayerMovement.grounded && wasLockLen)
        {
            grappleJoint.enabled = false;
            pull = true;
            wasLockLen = false;
        }
        else
        {
            grappleJoint.enabled = false;
            wasLockLen = false;
        }
    }
    

    void PointToCursor()
    {
        if (grappleOnBody)
        {
            var mouseDirection = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            var angle = Mathf.Atan2(mouseDirection.y, mouseDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
        }
    }

    void ReturnGrappleToBody()
    {
        if ((time > maxTimeOut) || inPlayer || (blockOnPlayer && pull) || returnToBody)
        {
            grappleOnBody = true;
            rb.velocity = Vector2.zero;
            if (moveableBlock != null)
            {
                moveableBlock.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            }
            transform.parent = player.transform;
            transform.localPosition = new Vector2(0.02f, -0.13f);
            collectTrigger.SetActive(false);
            inPlayer = false;
            returnToBody = false;
        }
    }
}
