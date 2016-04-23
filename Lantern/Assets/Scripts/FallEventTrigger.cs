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
            StartCoroutine(FallSequence());
        }
    }

    /*void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            playerController.enabled = true;
        }
    }*/

    public IEnumerator FallSequence()
    {
        Debug.Log("coroutine");
        camController.StartCoroutine(camController.ScreenShake(50, 3));
        yield return new WaitForSeconds(3);
        playerController.StartCoroutine(playerController.MovePlayer(Vector3.right, 1.1f));
        playerController.enabled = false;
        yield return new WaitForSeconds(1.1f);
        playerController.enabled = true;
        yield break;
    }
}
