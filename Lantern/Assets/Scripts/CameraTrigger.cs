using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class CameraTrigger : MonoBehaviour {

    [Header("Targeting")]
    public bool setTarget;
    public Vector3 target;
    public bool resetTarget;
    [Header("Offset")]
    public bool setOffset;
    public Vector3 offset;
    public bool resetOffset;
    [Header("setZoom")]
    public bool setZoom;
    public float size;
    public bool resetZoom;
    [Header("Lerping")]
    public bool setLerpSpeed;
    public float lerpSpeed;
    public bool resetLerpSpeed;

    GameObject mainCam;
    GameObject player;
    CameraController camController;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCam = GameObject.FindGameObjectWithTag("CameraContainer");
        camController = mainCam.GetComponent<CameraController>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "CameraContainer")
        {
            //set lerp speed
            if (setLerpSpeed)
            {
                camController.SetLerpSpeed(lerpSpeed);
            }
            //set offset
            if (setOffset)
            {
                camController.SetOffset(offset);
            }
            //set target
            if (setTarget)
            {
                camController.SetTarget(target);
            }
            //set size
            if (setZoom)
            {
                camController.SetZoom(size);
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            if (resetLerpSpeed)
            {
                camController.ResetLerpSpeed();
            }
            if (resetOffset)
            {
                camController.ResetOffset();
            }

            if (resetTarget)
            {
                camController.ResetTarget();
            }
            if (resetZoom)
            {
                camController.ResetZoom();
            }
        }
    }
}
