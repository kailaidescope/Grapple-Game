﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrappleCollect : MonoBehaviour
{
    private GameObject player;
    private bool inPlayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == player.name)
        {
            inPlayer = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        inPlayer = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (inPlayer)
        {
            GrappleController.inPlayer = inPlayer;
            inPlayer = false;
        }
    }
}
