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
    public bool customOffset = false;
    #endregion


    void Awake()
    {
        playerTransform = player.GetComponent<Transform>();
        playerController = player.GetComponent<PlayerController>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        mainCam = GetComponentInChildren<Camera>();
        ResetOffset();
        ResetTarget();
        target = playerTransform.position;
        transform.position = target + offset;
    }

    void Update()
    {
        CameraMove();
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(ScreenShake(150, 1));
        }
        //debugging
        Debug.DrawRay(target + offset, Vector3.up, Color.red);
        Debug.DrawRay(target + offset, Vector3.down, Color.red);
        Debug.DrawRay(target + offset, Vector3.left, Color.red);
        Debug.DrawRay(target + offset, Vector3.right, Color.red);

        Debug.DrawLine(playerTransform.position, playerTransform.position + offset, Color.white);
    }

    void CameraMove()
    {
        Vector3 current = transform.position;
        //targetting
        if (!targeted)
        {
            target = playerTransform.position;
        }

        //custom offsets
        if (!customOffset)
        {
            float yOffset = playerController.moveState == PlayerController.MoveState.falling ? -standardOffset.y : standardOffset.y;
            if (playerController.moveVector.x != 0)
            {
                offset = Mathf.Sign(playerController.moveVector.x) == -1 ? new Vector3(-standardOffset.x, yOffset, 0) : new Vector3(standardOffset.x, yOffset, 0);
            }
            else
            {
                offset = new Vector3(offset.x, yOffset, 0);
            }
        }


        //physics-smoothing
        //vector from camera to target
        Vector3 moveVector = (target + offset) - current;
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
        //customOffset = true;
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
        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, standardSize, Time.deltaTime * 30);
    }

    public IEnumerator ScreenShake(float strength, float duration)
    {
        Debug.Log("shake started");
        //initiate perlin noise variables
        float perlinX = Random.Range(-100, 100);
        float perlinY = Random.Range(-100, 100);
        yield return new WaitForSeconds(Time.deltaTime);

        //perform shake for duration
        while (duration >= 0)
        {
            Debug.Log("shaking");
            //decrease timer
            duration -= Time.deltaTime;
            //increment noise
            perlinX += .8f;
            perlinY += .8f;
            //Debug.Log(perlinY);

            //create new vector from perlin noise
            Vector3 translation = new Vector3((Mathf.PerlinNoise(perlinX, perlinY) - .5f) * strength, (Mathf.PerlinNoise(perlinX + .5f, perlinY + .5f) - .5f) * strength, 0);
            transform.position = Vector3.Lerp(transform.position, transform.position + translation, Time.deltaTime);
            //transform.position = Vector3.Lerp(target + offset, target + offset + translation, Time.deltaTime);
            yield return new WaitForSeconds(Time.deltaTime);
        }
        yield break;
        
    }
}
