using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoaderScript : MonoBehaviour
{
    Fader fader;


    //dont destroy this object when loading to a new scene
    void Awake()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        fader = GetComponent<Fader>();
    }

    public IEnumerator LoadScene(int scene)
    {
        fader.BeginFade(1);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(scene);
        if (scene == 0)
        {
            Destroy(gameObject);
        }
    }

}
