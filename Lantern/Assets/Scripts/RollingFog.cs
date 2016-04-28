using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class RollingFog : MonoBehaviour {

    [Range(-.1f, .1f)]
    public float xScrollSpeed;
    [Range(0, .1f)]
    public float yScrollSpeed;
    Vector2 scrollOffset;

    public Material fogMaterial;


    void Update()
    {
            fogMaterial.SetFloat("_XSpeed", xScrollSpeed);
            float yBob = Mathf.Sin(Time.time) * yScrollSpeed;
            fogMaterial.SetFloat("_YSpeed", yBob);
    }
}
