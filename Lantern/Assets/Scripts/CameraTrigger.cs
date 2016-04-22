using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CameraTrigger : MonoBehaviour {

    /*public float innerRadius;
    public float outerRadius;
    bool inTrigger;*/

    public Vector3 target;
    public float size;

    GameObject mainCam;
    GameObject player;
    CameraController camController;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCam = GameObject.Find("Camera container");
        camController = mainCam.GetComponent<CameraController>();
    }


    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Debug.Log("In camera trigger");
            camController.SetTarget(target);
            camController.SetOffset(Vector3.zero);
            camController.SetOrthoSize(size);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            camController.ResetOrthoSize();
            camController.ResetOffset();
            camController.ResetTarget();
        }
    }
}
