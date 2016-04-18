using UnityEngine;
using System.Collections;

[RequireComponent(typeof (AudioSource))]
[RequireComponent(typeof (Collider2D))]
[RequireComponent(typeof (Rigidbody2D))]
public class AudioPlayer : MonoBehaviour {

    public AudioClip audioClip;
    [Range(0, 1)]
    public float volume;
    private static AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = audioClip;
        source.volume = volume;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            source.Play();
        }
    }
}