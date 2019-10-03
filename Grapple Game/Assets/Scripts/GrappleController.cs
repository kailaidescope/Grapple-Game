using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleController : MonoBehaviour
{
    public static bool shoot;

    [SerializeField] private float offset;
    [SerializeField] private float grappleSpeed;

    private Rigidbody2D rb;
    private bool grappleOnBody;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        grappleOnBody = true;
        shoot = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Debug.Log(Input.mousePosition);
        ShootGrapple();
        PointToCursor();
    }

    void ShootGrapple()
    {
        if(shoot)
        {
            grappleOnBody = false;
        }
        shoot = false;
    }

    void PointToCursor()
    {
        if (grappleOnBody)
        {
            var direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
            var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle + offset, Vector3.forward);
        }
    }
}
