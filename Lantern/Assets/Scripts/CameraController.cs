using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
    #region variables
    //default values
    [Header("Defaults")]
    public Vector3 standardOffset;
    public float standardSize = 15;
    //references
    GameObject player;
    PlayerController playerController;
    Transform playerTransform;
    Rigidbody2D rb;
    Camera mainCam;
    Camera parallaxFar;
    Camera parallaxNear;

    //targetting variables
    Vector3 target = new Vector3();
    Vector3 offset = new Vector3();
    Vector3 result = new Vector3();
    float orthoSize = 15;
    [HideInInspector]
    public bool targeted;
    [HideInInspector]
    public bool customOffset = false;
    //bool to determine whether screenshake is on
    [HideInInspector]
    public bool isShaking;
    float platformYPos;
    bool onGround = true;
    public float lerpSpeed;
    float standardLerpSpeed = 10;
    #endregion


    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        rb = gameObject.GetComponent<Rigidbody2D>();
        mainCam = GetComponentInChildren<Camera>();
        parallaxFar = GameObject.Find("ParallaxCam Far").GetComponent<Camera>();
        parallaxNear = GameObject.Find("ParallaxCam Near").GetComponent<Camera>();
        ResetOffset();
        ResetTarget();
        ResetLerpSpeed();
        target = new Vector3(23, 2.8f, 0);
        transform.position = target;
    }

	void Start()
	{
		playerTransform = player.GetComponent<Transform>();
		playerController = player.GetComponent<PlayerController>();
	}

    void Update()
    {
        //lerp zoom
        mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, orthoSize, Time.deltaTime);
        //make sure FoV matches zoom
        parallaxFar.fieldOfView = (Mathf.Atan(mainCam.orthographicSize / 100) * Mathf.Rad2Deg) * 2f;
        parallaxNear.fieldOfView = (Mathf.Atan(mainCam.orthographicSize / 100) * Mathf.Rad2Deg) * 2f;

        CameraMove();

        //debugging
        Debug.DrawRay(result, Vector3.up, Color.red);
        Debug.DrawRay(result, Vector3.down, Color.red);
        Debug.DrawRay(result, Vector3.left, Color.red);
        Debug.DrawRay(result, Vector3.right, Color.red);

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
            float yOffset = standardOffset.y;
            if (!(Vector3.Distance(player.transform.position, playerController.groundSurface.point) <= 15))
            {
                yOffset = Mathf.Lerp(offset.y,
                            playerController.moveState == PlayerController.MoveState.falling ? -standardOffset.y : standardOffset.y,
                            Time.deltaTime * 10);
            }


            //change offset based on movement direction
            if (playerController.moveVector.x != 0)
            {
                offset = Vector3.Lerp(offset,
                                    Mathf.Sign(playerController.moveVector.x) == -1 ? new Vector3(-standardOffset.x, yOffset, 0) : new Vector3(standardOffset.x, yOffset, 0),
                                    Time.deltaTime * 10);
            }
            else
            {
                offset = new Vector3(offset.x, yOffset, 0);
            }
        }

        result = target + offset;

        //adjust target to be same height above ground
        if (onGround)
        {
            result = Vector3.Lerp(result, new Vector3(result.x, platformYPos + 8, result.z), Time.deltaTime * 10);
        }

        transform.position = Vector3.Lerp(transform.position, result, Time.deltaTime * lerpSpeed);
    }

    public bool InViewRect()
    {
        return true;
        //mainCam.WorldToScreenPoint();
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

    public void SetZoom(float size)
    {
        orthoSize = size;
    }

    public void ResetZoom()
    {
        orthoSize = standardSize;
    }

    public void PlatformSnap(Vector3 groundPos)
    {
        onGround = true;
        platformYPos = groundPos.y;
    }

    public void PlatformUnSnap()
    {
        onGround = false;
    }

    public void SetLerpSpeed(float newSpeed)
    {
        lerpSpeed = newSpeed;
    }

    public void ResetLerpSpeed()
    {
        lerpSpeed = standardLerpSpeed;
    }

    public IEnumerator ScreenShake(float strength, float duration)
    {
        isShaking = true;
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
        isShaking = false;
        yield break;
        
    }
}
