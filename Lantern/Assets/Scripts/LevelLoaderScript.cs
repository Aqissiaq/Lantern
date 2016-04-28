using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoaderScript : MonoBehaviour
{
    Fader fader;
    int MainMenu = 0;
    int Level1 = 1;
    int level2 = 2;

    //dont destroy this object when loading to a new scene
    void Awake()
    {
        GameObject.DontDestroyOnLoad(gameObject);
        fader = GetComponent<Fader>();
    }

    public IEnumerator LoadScene(int scene)
    {
        fader.BeginFade(1);
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene(scene);
    }

}
