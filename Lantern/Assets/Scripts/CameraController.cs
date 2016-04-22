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
    [HideInInspector]
    public bool customOffset;
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
        //stupid falling update that shouldn't need to be here, but it does
        if (playerController.moveState == PlayerController.MoveState.falling)
        {
            SetOffset(new Vector3(3 * Mathf.Sign(playerController.moveVector.x), -5, 0));
        }
        else
        {
            ResetOffset();
        }

        CameraMove();

        //debugging
        Debug.DrawRay(target, Vector3.up, Color.red);
        Debug.DrawRay(target, Vector3.down, Color.red);
        Debug.DrawRay(target, Vector3.left, Color.red);
        Debug.DrawRay(target, Vector3.right, Color.red);

        Debug.DrawLine(playerTransform.position, playerTransform.position + offset, Color.white);
    }

    void CameraMove()
    {
        Vector3 current = transform.position;
        if (!customOffset)
        {
            if (playerController.moveVector.x != 0)
            {
                offset = Mathf.Sign(playerController.moveVector.x) == -1 ? new Vector3(-standardOffset.x, standardOffset.y, 0) : new Vector3(standardOffset.x, standardOffset.y, 0);
            }
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
        customOffset = true;
        offset = newOffset;
    }

    public void ResetOffset()
    {
        customOffset = false;
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
