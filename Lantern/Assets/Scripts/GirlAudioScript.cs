using UnityEngine;
using System.Collections.Generic;

public class GirlAudioScript : MonoBehaviour {

    //references
    AudioSource audioSource;

    //walking sounds
    [Header("Footsteps")]
    public List<AudioClip> leafWalking = new List<AudioClip>();
    public List<AudioClip> woodWalking = new List<AudioClip>();
    public List<AudioClip> caveWalking = new List<AudioClip>();

    //variables
    public SurfaceType walkingSurface;
    int soundSelector = 0;

    public enum SurfaceType
    {
        leaves, wood, cave
    }

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            walkingSurface = SurfaceType.leaves;
            FootStepAudio();
        }
    }

    public void FootStepAudio()
    {
        switch (walkingSurface)
        {
            case SurfaceType.leaves:
                audioSource.clip = leafWalking[soundSelector]; //slot in clip
                if (!audioSource.isPlaying)
                {
                    audioSource.Play(); //play clip
                }
                soundSelector++; //increment counter
                if (soundSelector >= leafWalking.Count) //clamp counter and reshuffle list
                {
                    soundSelector = 0;
                    ShuffleList(leafWalking);
                }
                break;
            case SurfaceType.wood:
                break;
            case SurfaceType.cave:
                break;
            default:
                break;
        }

    }

    void ShuffleList(List<AudioClip> list)
    {
        int n = list.Count;

        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n - 1);
            AudioClip value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
