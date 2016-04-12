using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    #region variables
    //references
    public GameObject player;
    Transform playerTransform;
    Rigidbody2D rb;
    Camera mainCam;
    public GameObject testObject;
    #endregion


    void Awake()
    {
        playerTransform = player.GetComponent<Transform>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        mainCam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        CameraMove();
    }

    void CameraMove()
    {
        Vector3 current = transform.position;
        Vector3 target = current;

        #region targeting
        //put this in an if statement later for cues and regionbased anchors
        if (Input.GetKey(KeyCode.Space))
        {
            target = testObject.transform.position;
        }
        else
	    {
            target = playerTransform.position;
        }
        #endregion

        #region physics-smoothing
        //vector from camera to target
        Vector3 moveVector = target - current;
        Vector3 moveDirection = moveVector.normalized;

        //add force
        rb.AddForce(moveDirection * Mathf.Max(moveVector.magnitude * 2, .1f));
        #endregion

    }


    //spring dampening equation (not used for now)
    float SpringDampener(float a, float b, float t)
    {
        return Mathf.Exp(a * t) * (Mathf.Cos(b * t) - (a / b) * (Mathf.Sin(b * t)));
    }
}
