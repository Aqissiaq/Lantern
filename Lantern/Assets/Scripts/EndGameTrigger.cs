using UnityEngine;
using System.Collections;

public class EndGameTrigger : MonoBehaviour {

    public GameObject levelLoaderObj;
    LevelLoaderScript levelLoader;
    GameObject player;

void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (GameObject.FindGameObjectWithTag("GameController") == null)
        {
            GameObject levelLoaderObject = Instantiate(levelLoaderObj);
            levelLoader = levelLoaderObject.GetComponent<LevelLoaderScript>();
        }
        else
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoaderScript>();
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        Destroy(player.GetComponent<Rigidbody2D>());
        StartCoroutine(EndGame(3f));
    }

    public IEnumerator EndGame(float timer)
    {
        Vector3 moveDirection = new Vector3(3, 1, 0);
        player.transform.position = new Vector3(285, 25, 0);
        player.GetComponent<PlayerController>().moveState = PlayerController.MoveState.ledgegrab;
        player.GetComponent<AnimationController>().skeletonAnimation.timeScale = 0;
        yield return new WaitForSeconds(Time.deltaTime);
        player.GetComponent<AnimationController>().enabled = false;
        player.GetComponent<PlayerController>().enabled = false;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            transform.Translate(moveDirection * Time.deltaTime);
            player.transform.Translate(moveDirection * Time.deltaTime);
            if (timer <= .2f)
            {
                levelLoader.StartCoroutine(levelLoader.LoadScene(3));
            }
            yield return new WaitForEndOfFrame();
        }
        yield break;
    }
}
