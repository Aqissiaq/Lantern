using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class PlayerController : MonoBehaviour {

    #region variabels
    //movement variables for tuning
    [Header("Movement Variables")]
    public float moveSpeed;
    public float jumpStrength;
    public float friction;
    public float gravity;

    //checking values (customizable)
    [Header("Check Values")]
    public LayerMask groundCheck;
    public Vector3 ledgeGrabOffset;
    public Vector2 rectSize;
    public GameObject ledgeCheckCollider;
    Rect ledgeGrabRect;

    //references
    CameraController camController;

    MoveState moveState;
    bool jumping;
    float jumpTimer;
    //[HideInInspector]
    public Vector3 moveVector;
    #endregion

    enum MoveState
    {
        standing, walking, jumping, ledgegrab, falling
    }

    void Awake()
    {
        camController = GameObject.Find("Camera container").GetComponent<CameraController>();
    }

    void Update()
    {
        //keeping zPos == 0
        if (transform.position.z != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        CheckState();
        //debugging
        Debug.DrawRay(transform.position, moveVector, Color.blue);
        Debug.Log(moveState);
        Debug.DrawLine(transform.position, transform.position + ledgeGrabOffset, Color.magenta);

        Debug.DrawLine(new Vector3(ledgeGrabRect.xMin, ledgeGrabRect.yMin), new Vector3(ledgeGrabRect.xMax, ledgeGrabRect.yMin), Color.green); //top edge
        Debug.DrawLine(new Vector3(ledgeGrabRect.xMin, ledgeGrabRect.yMin), new Vector3(ledgeGrabRect.xMin, ledgeGrabRect.yMax), Color.green); //left edge
        Debug.DrawLine(new Vector3(ledgeGrabRect.xMin, ledgeGrabRect.yMax), new Vector3(ledgeGrabRect.xMax, ledgeGrabRect.yMax), Color.green); //bottom edge
        Debug.DrawLine(new Vector3(ledgeGrabRect.xMax, ledgeGrabRect.yMax), new Vector3(ledgeGrabRect.xMax, ledgeGrabRect.yMin), Color.green); //right edge
    }

    void LateUpdate()
    {
        Move();

        if (moveState != MoveState.falling)
        {
            camController.ResetOffset();
        }
    }

    void Move()
    {
        switch (moveState)
        {
            case MoveState.standing:
                //probably set an idle animation
                break;

            case MoveState.walking:
                //defining ground normal
                RaycastHit2D groundSurface = Physics2D.Raycast(transform.position, Vector3.down, 9001, groundCheck);
                Vector2 groundNormal = groundSurface.normal;
                Debug.DrawRay(transform.position, groundNormal * 100, Color.yellow);

                //define moveVector
                float horizontalMove = Input.GetAxis("Horizontal");
                moveVector = new Vector3(horizontalMove, 0, 0);

                //move along ground surface based on moveVector
                //rotation is acting weird, needs fix
                float theta = Vector3.Dot(moveVector.normalized, groundNormal.normalized);
                moveVector = new Vector3(moveVector.x * Mathf.Cos(theta) - moveVector.y * Mathf.Sin(theta), moveVector.x* Mathf.Sin(theta) + moveVector.y * Mathf.Cos(theta), 0);

                transform.position = Vector3.Lerp(transform.position, transform.position + moveVector * moveSpeed, Time.deltaTime);
                break;

            case MoveState.jumping:
                //timer
                jumpTimer += Time.deltaTime;
                if (jumpTimer >= .5f)
                {
                    jumping = false;
                }

                //actual jumping
                moveVector = new Vector3(Input.GetAxis("Horizontal") * 5, Mathf.Lerp(jumpStrength, jumpStrength * .5f, jumpTimer), 0);
                transform.position = Vector3.Lerp(transform.position, transform.position + moveVector, Time.deltaTime);
                break;

            case MoveState.ledgegrab:
                break;

            case MoveState.falling:
                //falling is a bit abrupt and intersect with ground somtimes, needs fix
                camController.SetOffset(new Vector3(3 * Mathf.Sign(moveVector.x), -5, 0));
                moveVector = Vector3.Lerp(moveVector, new Vector3(Input.GetAxis("Horizontal") * 5, -gravity, 0), Time.deltaTime);
                transform.position = Vector3.Lerp(transform.position, transform.position + moveVector, Time.deltaTime);
                break;

            default:
                //idle animation just in case?
                break;
        }
    }

    void CheckState()
    {
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jumpTimer = 0;
            jumping = true;
        }

        if (jumping)
        {
            moveState = MoveState.jumping;
        }
        else if (IsGrounded())
        {
            moveState = MoveState.walking;
        }
        else if (CheckLedge())
        {
            moveState = MoveState.ledgegrab;
        }
        else if (!IsGrounded())
        {
            moveState = MoveState.falling;
        }
        else if (IsGrounded() && Input.GetAxisRaw("Horizontal") == 0)
        {
            moveState = MoveState.standing;
        }
    }

    public bool IsGrounded()
    {
       return Physics2D.Raycast(transform.position, Vector3.down, 2.6f, groundCheck);
    }

    public RaycastHit2D CheckCollision(Vector3 check)
    {
        return Physics2D.Raycast(transform.position, check, check.magnitude, groundCheck);
    }

    public bool CheckLedge()
    {
        bool onLedge;
        //spawn rectangular object to check for collision
        ledgeGrabRect = new Rect((transform.position + ledgeGrabOffset) - .5f * new Vector3(rectSize.x, rectSize.y), rectSize);
        GameObject ledgeCheckObject =  GameObject.Instantiate(ledgeCheckCollider);
        Collider2D ledgeCheck = ledgeCheckObject.GetComponent<Collider2D>();
        ledgeCheckObject.transform.position = ledgeGrabRect.position;
        ledgeCheckObject.transform.localScale = ledgeGrabRect.size;

        //return collision and destroy check-object
        onLedge = !ledgeCheck.IsTouchingLayers(groundCheck) && Physics2D.Raycast(transform.position, Vector3.right * Mathf.Sign(moveVector.x), 2.5f, groundCheck);
        DestroyImmediate(ledgeCheckObject);
        return onLedge;
    }
}
