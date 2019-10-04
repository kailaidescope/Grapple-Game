using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeController : MonoBehaviour
{
    [SerializeField] private Vector2 playerPosOffset;
    [SerializeField] private Vector2 grapplePosOffset;

    private LineRenderer rope;
    private GameObject player;
    private GameObject grapple;

    // Start is called before the first frame update
    void Start()
    {
        rope = gameObject.GetComponent<LineRenderer>();
        player = GameObject.Find("Player");
        grapple = GameObject.Find("Grapple");
    }

    // Update is called once per frame
    void Update()
    {
        rope.SetPositions(new Vector3[] {new Vector3(player.transform.position.x + playerPosOffset.x, player.transform.position.y + playerPosOffset.y, 0f),
                                         new Vector3(grapple.transform.position.x + grapplePosOffset.x, grapple.transform.position.y + grapplePosOffset.y, 0f) });
    }
}
