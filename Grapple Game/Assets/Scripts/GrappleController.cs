using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleController : MonoBehaviour
{
    public static bool shoot;
    public static bool pull;
    public static bool inPlayer;

    [SerializeField] private float offset;
    [SerializeField] private float grappleSpeed;
    [SerializeField] private float grapplePullMod;
    [SerializeField] private float playerPullMod;
    [SerializeField] private float maxTimeOut;

    private GameObject player;
    private GameObject collectTrigger;
    private Collider2D headCollider;
    private Rigidbody2D rb;
    private Rigidbody2D prb;
    private Vector2 mousePos;
    private Vector2 mouseDirection;
    private Vector2 playerDirection;
    private float time;
    private bool grappleOnBody;
    private bool inWall;
    private bool wasInWall;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name != player.name && collision.gameObject.name != "TilemapNoClip")
        {
            inWall = true;
            collectTrigger.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name != player.name && collision.gameObject.name != "TilemapNoClip")
        {
            inWall = false;
        }
    }

    void Start()
    {
        player = GameObject.Find("Player");
        collectTrigger = GameObject.Find("GrappleCollectTrigger");
        collectTrigger.SetActive(false);
        headCollider = gameObject.GetComponent<Collider2D>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        prb = player.GetComponent<Rigidbody2D>();
        GetDirections();
        grappleOnBody = true;
        shoot = false;
        pull = false;
        inWall = false;
        wasInWall = false;
        time = 0f;
    }

    void FixedUpdate()
    {
        GetDirections();
        ShootGrapple();
        PullGrapple();
        PullToGrapple();
        PointToCursor();
        if (inPlayer)
        {
            ReturnGrappleToBody();
        }
        if (inWall && !pull)
        {
            rb.velocity = Vector2.zero;
        }

        if (grappleOnBody)
        {
            transform.parent = player.transform;
            headCollider.enabled = false;
            if (shoot)
            {
                shoot = false;
            }
        }
        else if (!grappleOnBody && pull)
        {
            headCollider.enabled = true;
        }
        else
        {
            transform.parent = null;
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

        if(time > maxTimeOut)
        {
            ReturnGrappleToBody();
        }
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
        }
    }
    void PullGrapple()
    {
        if (pull && !grappleOnBody)
        {
            rb.velocity = new Vector2(playerDirection.x * grappleSpeed/grapplePullMod + rb.velocity.x, playerDirection.y * grappleSpeed/grapplePullMod + rb.velocity.y);
        }
        else
        {
            pull = false;
        }
    }
    void PullToGrapple()
    {
        if (shoot && !grappleOnBody && (inWall || wasInWall))
        {
            prb.velocity = new Vector2(-playerDirection.x * grappleSpeed/playerPullMod + prb.velocity.x, -playerDirection.y * grappleSpeed/playerPullMod + prb.velocity.y);
            wasInWall = true;
        }
        else
        {
            shoot = false;
            wasInWall = false;
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
        grappleOnBody = true;
        rb.velocity = Vector2.zero;
        transform.parent = player.transform;
        transform.localPosition = new Vector2(0.02f, -0.13f);
        collectTrigger.SetActive(false);
        inPlayer = false;
    }
}
