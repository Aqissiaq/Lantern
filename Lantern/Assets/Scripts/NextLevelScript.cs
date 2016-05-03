using UnityEngine;
using System.Collections;

public class NextLevelScript : MonoBehaviour {

    LevelLoaderScript levelLoader;
    public GameObject obj;
    public int level;

    void Update()
    {
        if (GameObject.FindGameObjectWithTag("GameController") == null)
        {
            GameObject levelLoaderObject =  Instantiate(obj);
            levelLoader = levelLoaderObject.GetComponent<LevelLoaderScript>();
        }
        else
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoaderScript>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            levelLoader.StartCoroutine(levelLoader.LoadScene(level));
        }
    }
}
