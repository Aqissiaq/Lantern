using UnityEngine;
using System.Collections;

public class WaterDetector : MonoBehaviour {

void OnTriggerEnter2D(Collider2D hit)
    {
        Rigidbody2D rb = hit.gameObject.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            transform.parent.GetComponent<WaterScript>().Splash(transform.position.x, rb.velocity.y * rb.mass / 2000f);
        }
    }
}