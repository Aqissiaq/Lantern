using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    #region variables
    public Vector3 standardOffset;
    //references
    public GameObject player;
    PlayerMovement playerMovement;
    Transform playerTransform;
    Rigidbody2D rb;
    Camera mainCam;

    //targetting variables
    Vector3 target = new Vector3();
    Vector3 offset = new Vector3();
    bool targeted;
    #endregion


    void Awake()
    {
        playerTransform = player.GetComponent<Transform>();
        playerMovement = player.GetComponent<PlayerMovement>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        mainCam = GetComponentInChildren<Camera>();
        SetOffset(standardOffset);
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

        #region targeting
        if (!targeted)
        {
            target = playerTransform.position;
        }
        target += offset;
        #endregion

        //transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime);

        #region physics-smoothing
        //vector from camera to target
        Vector3 moveVector = target - current;
        Vector3 moveDirection = moveVector.normalized;

        //add force
        rb.AddForce(moveDirection * Mathf.Max(moveVector.magnitude * 2, .1f) * Time.deltaTime * 60);
        #endregion

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
}
