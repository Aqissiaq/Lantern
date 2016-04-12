using UnityEngine;
using System.Collections;

public class CameraTrigger : MonoBehaviour {

    public float innerRadius;
    public float outerRadius;

    GameObject mainCam;
    GameObject player;
    CameraController camController;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        camController = mainCam.GetComponent<CameraController>();
    }

    void Update()
    {
        float distanceToPlayer = (player.transform.position - transform.position).magnitude;
        if (distanceToPlayer <= innerRadius)
        {
            camController.SetTarget(transform.position);
            camController.SetOffset(Vector3.zero);

        }
        else if (distanceToPlayer < outerRadius && distanceToPlayer > innerRadius)
        {
            camController.SetTarget(Vector3.Lerp(transform.position, player.transform.position, (distanceToPlayer - innerRadius) / (outerRadius - innerRadius)));
            camController.SetOffset(Vector3.zero);
        }
        else
        {
            Debug.Log("reset");
            camController.ResetTarget();
            camController.ResetOffset();
        }
        
    }
}
