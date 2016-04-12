using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {

    public float speed;

    void Update()
    {
        gameObject.transform.Translate(PlayerMove());
    }

    Vector3 PlayerMove()
    {
        Vector3 move = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0);
        return move * speed;

    }
}
