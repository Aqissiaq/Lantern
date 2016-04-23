using UnityEngine;
using System.Collections;

public class FallEventTrigger : MonoBehaviour {

    //references
    CameraController camController;
    PlayerController playerController;
    GameObject player;


    void Awake()
    {
        camController = GameObject.Find("Camera container").GetComponent<CameraController>();
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            camController.ScreenShake(50, 3);
            playerController.enabled = false;
            playerController.StartCoroutine(playerController.MovePlayer(Vector3.right, 2));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerController.enabled = true;
        }
    }
}
