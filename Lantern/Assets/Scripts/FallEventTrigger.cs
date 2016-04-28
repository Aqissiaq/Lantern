﻿using UnityEngine;
using System.Collections;

public class FallEventTrigger : MonoBehaviour {

    //references
    CameraController camController;
    PlayerController playerController;
    GameObject player;
    Rigidbody2D playerRb;
    Collider2D col;
    //target of movement
    Vector3 worldTargetPos = new Vector3(300, 20, 0);


    void Awake()
    {
        col = GetComponent<Collider2D>();
        camController = GameObject.Find("Camera container").GetComponent<CameraController>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        playerRb = player.GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            col.enabled = false;
            StartCoroutine(FallSequence());
        }
    }


    public IEnumerator FallSequence()
    {
        Debug.Log("coroutine");
        if (!camController.isShaking)
        {
            camController.StartCoroutine(camController.ScreenShake(50, 2));
        }
        playerController.enabled = false;
        yield return new WaitForSeconds(3);
        if ((player.transform.position.y >= worldTargetPos.y - 10))
        {
            playerRb.AddForce((worldTargetPos - player.transform.position) * 100, ForceMode2D.Impulse);
            playerController.moveState = PlayerController.MoveState.falling;
        }
        camController.PlatformUnSnap();
        yield return new WaitForSeconds(1.5f);
        playerController.enabled = true;
        yield break;
    }
}
