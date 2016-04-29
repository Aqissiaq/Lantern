using UnityEngine;
using System.Collections;

public class SurfaceTypeTrigger : MonoBehaviour {

    GirlAudioScript girlAudio;
    public GirlAudioScript.SurfaceType surfaceType;

    void Awake()
    {
        girlAudio = GameObject.FindGameObjectWithTag("Player").GetComponent<GirlAudioScript>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            girlAudio.walkingSurface = surfaceType;
        }
    }
}
