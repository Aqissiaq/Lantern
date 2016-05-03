using UnityEngine;
using System.Collections;

public class CreditsScroller : MonoBehaviour
{
    public GameObject levelLoaderObj;
    public float scrollSpeed;
    LevelLoaderScript levelLoader;
    float yPos;


    void Start()
    {
        yPos = transform.position.y;
        if (GameObject.FindGameObjectWithTag("GameController") == null)
        {
            GameObject levelLoaderObject = Instantiate(levelLoaderObj);
            levelLoader = levelLoaderObject.GetComponent<LevelLoaderScript>();
        }
        else
        {
            levelLoader = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelLoaderScript>();
        }
    }

    void Update()
    {
        yPos += scrollSpeed;
        transform.position = new Vector3(0, yPos, 0);

        if (yPos >= 11)
        {
            levelLoader.StartCoroutine(levelLoader.LoadScene(0));
        }
    }
}
