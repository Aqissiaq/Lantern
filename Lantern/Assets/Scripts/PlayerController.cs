using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class PlayerController : MonoBehaviour {

    #region variabels
    //movement variables for tuning
    [Header("Movement Variables")]
    public float moveSpeed;
    public float jumpStrength;
    public float gravity;

    //checking values (customizable)
    [Header("Check Values")]
    public LayerMask groundCheck;
    public Vector3 ledgeGrabOffset;
    Vector3 offset;
    public Vector2 rectSize;
    public GameObject ledgeCheckCollider;
    Rect ledgeGrabRect;

    //references
    public GameObject camHolder;
    CameraController camController;
    Collider2D col;

    public MoveState moveState;
    bool jumping;
    float jumpTimer;
    RaycastHit2D groundSurface;
    Vector2 groundNormal;
    bool debugging = true;
    bool checkState = true;
    Vector3 climbDestination;
    [HideInInspector]
    public Vector3 moveVector;

    private static float pi = Mathf.PI;
    #endregion

    public enum MoveState
    {
        standing, walking, jumping, ledgegrab, falling
    }

    void Awake()
    {
        camController = camHolder.GetComponent<CameraController>();
        col = GetComponent<Collider2D>();
    }

    void Update()
    {
        //keeping zPos == 0
        if (transform.position.z != 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            debugging = !debugging;
        }

        //check movestate
        if (checkState)
        {
            CheckState();
        }
        
        //perform movement
        Move();

        //improve collisions
        if (moveState == MoveState.walking || moveState == MoveState.standing)
        {
            if (groundNormal.normalized == Vector2.up)
            {
                if (col.bounds.Intersects(groundSurface.collider.bounds))
                {
                    Debug.Log("Intersects");
                    Vector3 toSurface = new Vector3(0, groundSurface.collider.bounds.max.y - col.bounds.min.y, 0);
                    transform.position = Vector3.Lerp(transform.position, transform.position + toSurface, Time.deltaTime * 30);
                }
            }
        }


        //debugging
        if (Input.GetKeyDown(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
        }  
        if (debugging)
        {
            Debug.DrawRay(transform.position, moveVector * 10, Color.blue);
            Debug.Log(moveState);
            Debug.DrawRay(transform.position, groundNormal * 10, Color.yellow);

            //ledgegrab rectangle
            Debug.DrawLine(new Vector3(ledgeGrabRect.xMin, ledgeGrabRect.yMin), new Vector3(ledgeGrabRect.xMax, ledgeGrabRect.yMin), Color.green); //top edge
            Debug.DrawLine(new Vector3(ledgeGrabRect.xMin, ledgeGrabRect.yMin), new Vector3(ledgeGrabRect.xMin, ledgeGrabRect.yMax), Color.green); //left edge
            Debug.DrawLine(new Vector3(ledgeGrabRect.xMin, ledgeGrabRect.yMax), new Vector3(ledgeGrabRect.xMax, ledgeGrabRect.yMax), Color.green); //bottom edge
            Debug.DrawLine(new Vector3(ledgeGrabRect.xMax, ledgeGrabRect.yMax), new Vector3(ledgeGrabRect.xMax, ledgeGrabRect.yMin), Color.green); //right edge
        }
    }

    void Move()
    {
        switch (moveState)
        {
            case MoveState.standing:
                //align with surface
                //probably set an idle animation
                break;

            case MoveState.walking:
                //define moveVector
                float horizontalMove = Input.GetAxis("Horizontal");
                moveVector = new Vector3(horizontalMove, 0, 0);

                //move along ground surface based on moveVector
                float theta = Mathf.Acos(Vector3.Dot(moveVector.normalized, groundNormal.normalized));
                theta = moveVector.x < 0 ? (pi / 2) - theta : -((pi / 2) - theta);
                moveVector = new Vector3(moveVector.x * Mathf.Cos(theta) - moveVector.y * Mathf.Sin(theta), moveVector.x * Mathf.Sin(theta) + moveVector.y * Mathf.Cos(theta), 0);

                if (!CheckCollision(moveVector))
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + moveVector * moveSpeed, Time.deltaTime);
                }
                break;

            case MoveState.jumping:
                //timer
                jumpTimer += Time.deltaTime;
                if (jumpTimer >= .2f)
                {
                    jumping = false;
                }

                //actual jumping
                moveVector = new Vector3(Input.GetAxis("Horizontal") * 5, Mathf.Lerp(jumpStrength, jumpStrength * .5f, jumpTimer), 0);
                if (!CheckCollision(moveVector))
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + moveVector, Time.deltaTime);
                }
                break;

            case MoveState.ledgegrab:
                transform.position = Vector3.Lerp(transform.position, climbDestination, Input.GetAxis("Vertical"));
                checkState = transform.position == climbDestination;
                break;

            case MoveState.falling:
                moveVector = Vector3.Lerp(moveVector, new Vector3(Input.GetAxis("Horizontal") * 5, -gravity, 0), Time.deltaTime);
                if (!CheckCollision(moveVector.normalized * 2.6f))
                {
                    transform.position = Vector3.Lerp(transform.position, transform.position + moveVector, Time.deltaTime);
                }
                break;

            default:
                //idle animation just in case?
                break;
        }
    }

    void CheckState()
    {
        //get groundnormal and shit
        groundSurface = Physics2D.Raycast(transform.position, Vector3.down, 9001, groundCheck);
        groundNormal = Vector3.Lerp(groundNormal, groundSurface.normal, Time.deltaTime * 30);

        //get jump input
        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            jumpTimer = 0;
            jumping = true;
        }

        //ledgecheck
        bool onLedge = CheckLedge();

        //determine movestate
        if (onLedge)
        {
            climbDestination = transform.position + offset;
            moveState = MoveState.ledgegrab;
        }
        else if (jumping)
        {
            moveState = MoveState.jumping;
        }
        else if (IsGrounded() && Input.GetAxisRaw("Horizontal") == 0)
        {
            moveState = MoveState.standing;
        }
        else if (IsGrounded())
        {
            moveState = MoveState.walking;
        }
        else if (!IsGrounded())
        {
            moveState = MoveState.falling;
        }

    }

    public bool IsGrounded()
    {
        //these values need to be updated to match the sprite
        return Physics2D.CircleCast(transform.position, 1.3f, -groundNormal, 1.4f, groundCheck);
    }

    public RaycastHit2D CheckCollision(Vector3 check)
    {
        return Physics2D.Raycast(transform.position, check, check.magnitude, groundCheck);
    }

    public bool CheckLedge()
    {
        bool onLedge;
        if (moveVector.x != 0)
        {
            offset = Mathf.Sign(moveVector.x) == 1 ? new Vector3(ledgeGrabOffset.x, ledgeGrabOffset.y, 0) : new Vector3(-ledgeGrabOffset.x, ledgeGrabOffset.y, 0);
        }

        //debugging
        if (debugging)
        {
            Debug.DrawLine(transform.position, transform.position + offset, Color.magenta);
        }

        //spawn rectangular object to check for collision
        ledgeGrabRect = new Rect((transform.position + offset) - .5f * new Vector3(rectSize.x, rectSize.y), rectSize);
        GameObject ledgeCheckObject =  GameObject.Instantiate(ledgeCheckCollider);
        Collider2D ledgeCheck = ledgeCheckObject.GetComponent<Collider2D>();
        ledgeCheckObject.transform.position = ledgeGrabRect.position;
        ledgeCheckObject.transform.localScale = ledgeGrabRect.size;

        //return collision and destroy check-object
        onLedge = !ledgeCheck.IsTouchingLayers(groundCheck) && Physics2D.Raycast(transform.position, Vector3.right * Mathf.Sign(moveVector.x), 1.3f, groundCheck);
        DestroyImmediate(ledgeCheckObject);
        return onLedge;
    }
}
