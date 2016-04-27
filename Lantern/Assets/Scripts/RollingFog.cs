using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RollingFog : MonoBehaviour {

    [Range(-.1f, .1f)]
    public float xScrollSpeed;
    [Range(0, .1f)]
    public float yScrollSpeed;
    public bool scroll;
    Vector2 scrollOffset;

    public Material fogMaterial;


    void Update()
    {
        if (scroll)
        {
            fogMaterial.SetFloat("_XSpeed", xScrollSpeed);
            float yBob = Mathf.Sin(Time.time) * yScrollSpeed;
            fogMaterial.SetFloat("_YSpeed", yBob);
        }
    }
}
