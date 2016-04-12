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

    //target variables
    Vector3 target = new Vector3();
    bool cueTarget;

    //window variable
    CamWindow cameraWindow;
    #endregion


    void Awake()
    {
        cameraWindow = new CamWindow(200, Screen.width - 200);
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
       
        #region targeting
        //mouse testing
        if (Input.GetMouseButtonDown(0))
        {
            SetTarget(mainCam.ScreenToWorldPoint(Input.mousePosition));
        }
        if (Input.GetMouseButtonDown(1))
        {
            ResetTarget();
        }

        if (!cueTarget)
        {
            target = playerTransform.position;
        }
        #endregion

        #region camera window
        Vector3 playerScreenPos = mainCam.WorldToScreenPoint(playerTransform.position);

        /*if (playerScreenPos.x <= cameraWindow.left)
        {
            Vector3 offset = playerTransform.position - mainCam.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 0));
            transform.position = playerTransform.position + offset;
        }*/

        #endregion

        #region physics-smoothing
        //vector from camera to target
        Vector3 moveVector = target - current;
        Vector3 moveDirection = moveVector.normalized;

        //add force
        rb.AddForce(moveDirection * Mathf.Max(moveVector.magnitude * 2, .1f));
        #endregion

    }

    public void SetTarget(Vector3 newTarget)
    {
        target = newTarget;
        cueTarget = true;
    }

    public void ResetTarget()
    {
        Debug.Log("resetTarget");
        cueTarget = false;
    }

    public struct CamWindow
    {
        public int left, right;

        public CamWindow(int l, int r)
        {
            left = l;
            right = r;
        }
    }
}
