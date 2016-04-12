using UnityEngine;
using System.Collections;

public class CameraTrigger : MonoBehaviour {

    public float triggerRadius;

    GameObject mainCam;
    GameObject player;
    CameraController camController;
    bool inTrigger;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        camController = mainCam.GetComponent<CameraController>();
    }

    void Update()
    {
        float distanceToPlayer = (player.transform.position - transform.position).magnitude;

        if (distanceToPlayer <= triggerRadius)
        {
            camController.SetTarget(transform.position);
            inTrigger = true;
        }
        else if(distanceToPlayer > triggerRadius && inTrigger)
        {
            camController.ResetTarget();
            inTrigger = false;
        }
    }
}
