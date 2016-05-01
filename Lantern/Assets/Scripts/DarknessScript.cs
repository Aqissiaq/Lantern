using UnityEngine;
using System.Collections;

//[ExecuteInEditMode]
public class DarknessScript : MonoBehaviour {

    public Material material;
    public Texture initialTexture;
    public RenderTexture texture;
    private RenderTexture buffer;

    public float updateInterval = .1f;
    private float lastUpdateTime;

    void Start()
    {
        Graphics.Blit(initialTexture, texture);
        buffer = new RenderTexture(texture.width, texture.height, texture.depth, texture.format);
    }

    public void Update()
    {
        if (Time.time > lastUpdateTime + updateInterval)
        {
            UpdateTexture();
            lastUpdateTime = Time.time;
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, material);
    }

    public void UpdateTexture()
    {
        Graphics.Blit(texture, buffer, material);
        Graphics.Blit(buffer, texture);
    }


}
