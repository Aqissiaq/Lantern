using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
    #region variables
    //default values
    public Vector3 standardOffset;
    public float standardSize = 15;
    //references
    public GameObject player;
    PlayerController playerController;
    Transform playerTransform;
    Rigidbody2D rb;
    Camera mainCam;

    //targetting variables
    Vector3 target = new Vector3();
    Vector3 offset = new Vector3();
    [HideInInspector]
    public bool targeted;
    #endregion


    void Awake()
    {
        playerTransform = player.GetComponent<Transform>();
        playerController = player.GetComponent<PlayerController>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        mainCam = GetComponentInChildren<Camera>();
        SetOffset(standardOffset);
        SetTarget(playerTransform.position);
        transform.position = target + offset;
    }

    void Update()
    {
        CameraMove();

        //debugging
        Debug.DrawRay(target, Vector3.up, Color.red);
        Debug.DrawRay(target, Vector3.down, Color.red);
        Debug.DrawRay(target, Vector3.left, Color.red);
        Debug.DrawRay(target, Vector3.right, Color.red);
    }

    void CameraMove()
    {
        Vector3 current = transform.position;
        if (playerController.moveVector != Vector3.zero)
        {
            offset.x = Mathf.Lerp(0, standardOffset.x, playerController.moveVector.magnitude) * Mathf.Sign(playerController.moveVector.x);
        }

        //targetting
        if (!targeted)
        {
            target = playerTransform.position;
        }
        target += offset;
       

        //physics-smoothing
        //vector from camera to target
        Vector3 moveVector = target - current;
        Vector3 moveDirection = moveVector.normalized;

        //add force
        rb.AddForce(moveDirection * Mathf.Max(moveVector.magnitude * 5, .1f) * Time.deltaTime * 60);

    }

    public void SetTarget(Vector3 newTarget)
    {
            target = newTarget;
            targeted = true;
    }

    public void ResetTarget()
    {
        targeted = false;
    }

    public void SetOffset(Vector3 newOffset)
    {
        offset = newOffset;
    }

    public void ResetOffset()
    {
        offset = standardOffset;
    }

    public void SetOrthoSize(float size)
    {
        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, size, Time.deltaTime);
    }

    public void ResetOrthoSize()
    {
        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, standardSize, Time.deltaTime);
    }
}
