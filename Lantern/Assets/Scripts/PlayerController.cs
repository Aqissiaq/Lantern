﻿using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    #region variabels
    //movement variables for tuning
    [Header("Movement Variables")]
    public float moveSpeed;
    public float jumpStrength;
    public float maxJumpTime;

    //checking values (customizable)
    [Header("Check Values")]
    public LayerMask groundCheck;
    public Vector3 ledgeGrabOffset;
    public Vector2 rectSize;
    public GameObject ledgeCheckCollider;
    Rect ledgeGrabRect;

    //references
    CameraController camController;
    Collider2D col;
    GameObject ledgeCheckObject;
    Collider2D ledgeCheck;
    Rigidbody2D rb;

    public MoveState moveState;
    bool jumping;
    float jumpTimer;
    RaycastHit2D groundSurface;
    Vector2 groundNormal;
    bool debugging = true;
    bool checkState = true;
    Vector3 climbDestination;
    Vector3 offset;
    bool xClimbed;
    bool yClimbed;
    [HideInInspector]
    public Vector2 moveVector;

    private static float pi = Mathf.PI;
    private static float sqr2 = Mathf.Sqrt(2);
    #endregion

    public enum MoveState
    {
        standing, walking, jumping, ledgegrab, falling
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        camController = GameObject.Find("Camera container").GetComponent<CameraController>();
        col = GetComponent<Collider2D>();

        ledgeCheckObject = GameObject.Instantiate(ledgeCheckCollider);
        ledgeCheck = ledgeCheckObject.GetComponent<Collider2D>();
    }

    void Start()
    {
        Physics2D.IgnoreCollision(col, ledgeCheck);
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

        //set to kinematic to move player
        if (moveState == MoveState.ledgegrab)
        {
            rb.isKinematic = true;
        }
        else
        {
            rb.isKinematic = false;
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

            //ledgegrab climbdestination
            Debug.DrawRay(climbDestination, Vector3.up, Color.white);
            Debug.DrawRay(climbDestination, Vector3.down, Color.white);
            Debug.DrawRay(climbDestination, Vector3.left, Color.white);
            Debug.DrawRay(climbDestination, Vector3.right, Color.white);
        }
    }

    void FixedUpdate()
    {
        //do moving
        Move();
    }

    void Move()
    {
        switch (moveState)
        {
            case MoveState.standing:
                //align with surface?
                //probably set an idle animation

                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, Time.deltaTime);
                break;

            case MoveState.walking:
                //set movevector based on input
                moveVector = new Vector2(Input.GetAxis("Horizontal") * moveSpeed, 0);
                //rotate movevector orthogonal to ground surface
                float theta = Mathf.Acos(Vector3.Dot(moveVector.normalized, groundNormal.normalized));
                theta = moveVector.x < 0 ? (pi / 2) - theta : -((pi / 2) - theta);
                moveVector = new Vector2(moveVector.x * Mathf.Cos(theta) - moveVector.y * Mathf.Sin(theta),
                    moveVector.x * Mathf.Sin(theta) + moveVector.y * Mathf.Cos(theta));
                //preserve y-velocity if the ground is flat
                if (!(Mathf.Abs(Vector3.Dot(groundNormal.normalized, Vector3.up)) <= sqr2 / 2))
                {
                    moveVector = new Vector2(moveVector.x, rb.velocity.y);
                }
                //set velocity of rigidbody
                rb.velocity = moveVector;
                break;

            case MoveState.jumping:
                Vector2 jumpVector = new Vector2(0, jumpStrength);
                rb.AddForce(jumpVector, ForceMode2D.Impulse);
                jumping = false;
                break;

            case MoveState.ledgegrab:
                CheckLedge();
                checkState = false;
                Vector3 xClimb = new Vector3(climbDestination.x, transform.position.y, 0) - transform.position;
                Vector3 yClimb = new Vector3(transform.position.x, climbDestination.y, 0) - transform.position;
                //check if vertical climbed
                if (Mathf.Abs(transform.position.y - climbDestination.y) <= .1f)
                {
                    yClimbed = true;
                }
                //check if horizontal climbed
                if (Mathf.Abs(transform.position.x - climbDestination.x) <= .1f)
                {
                    xClimbed = true;
                }
                //climb vertically
                if (!yClimbed && Input.GetAxis("Vertical") > 0)
                {
                    rb.MovePosition(transform.position + yClimb * Time.deltaTime * 10);
                }
                //then horizontally
                if (yClimbed && !xClimbed && Input.GetAxis("Horizontal") * Mathf.Sign(offset.x) > 0)
                {
                    rb.MovePosition(transform.position + xClimb * Time.deltaTime * 10);
                }
                //check if climb is complete
                if (Mathf.Abs((transform.position - climbDestination).magnitude) <= 1)
                {
                    checkState = true;
                    yClimbed = false;
                    xClimbed = false;
                }
                break;

            case MoveState.falling:

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
        groundNormal = Vector3.Lerp(groundNormal, groundSurface.normal, Time.deltaTime * 60);

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
            climbDestination = transform.position + offset + new Vector3(0, 1.26f, 0);
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
        //modified to work only on flat surfaces
        bool groundIsFlat = !(Mathf.Abs(Vector3.Dot(groundNormal.normalized, Vector3.up)) <= sqr2 / 2);
        RaycastHit2D ground =  Physics2D.CircleCast(transform.position, 1.5f, -groundNormal, 1.4f, groundCheck);
        //Debug.Log("GroundIsFlat" + groundIsFlat);
        //Debug.Log(Mathf.Abs(Vector3.Dot(groundNormal.normalized, Vector3.up)));
        return ground && groundIsFlat;
    }

    public RaycastHit2D CheckCollision(Vector3 check)
    {
        return Physics2D.CircleCast(transform.position, 1.3f, check, check.magnitude - 1.3f, groundCheck);
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

        //move rectangular object to check for collision
        ledgeGrabRect = new Rect((transform.position + offset) - .5f * new Vector3(rectSize.x, rectSize.y), rectSize);
        ledgeCheckObject.transform.position = ledgeGrabRect.position;
        ledgeCheckObject.transform.localScale = ledgeGrabRect.size;

        //return collision and destroy check-object
        onLedge = !ledgeCheck.IsTouchingLayers(groundCheck) && Physics2D.Raycast(transform.position, Vector3.right * Mathf.Sign(moveVector.x), 1.5f, groundCheck);
        return onLedge;
    }

    public Collider2D CheckSideCollisions()
    {
        return Physics2D.OverlapCircle(transform.position, 1.28f, groundCheck);
    }

    //function to move player arbitrarily
    public IEnumerator MovePlayer(Vector3 move, float timer)
    {
        rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(Time.deltaTime);
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            rb.isKinematic = true;
            rb.MovePosition(transform.position + move * Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        rb.isKinematic = false;
        yield break;
    }
}
