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
    private static Light lightSource;

    void Awake()
    {
        source = GetComponent<AudioSource>();
        source.clip = audioClip;
        source.volume = volume;
        lightSource = GetComponent<Light>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            source.Play();
            StartCoroutine(Disappear(1));
        }
    }

    IEnumerator Disappear(float fader)
    {
        while (fader > 0)
        {
            lightSource.intensity = fader;
            fader -= 0.05f;
            yield return new WaitForSeconds(.1f);
        }
        Destroy(gameObject);
        yield break;
    }
}