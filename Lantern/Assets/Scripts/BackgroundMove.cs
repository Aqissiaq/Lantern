using UnityEngine;
using System.Collections;

public class BackgroundMove : MonoBehaviour {

    public float moveSpeed;

    void Update()
    {
        float x = 1.5f * Mathf.Sin(Time.time * moveSpeed);
        float y = 1.5f * Mathf.Cos(Time.time * moveSpeed);
        transform.position = new Vector3(x, y, 0);
    }
}
