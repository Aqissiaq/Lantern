using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour {
    GameObject menuHolder;
    Canvas mainMenu;
    Canvas optionsMenu;
    Camera mainCam;

    void Awake()
    {
        menuHolder = GameObject.Find("MenuHolder");
        mainMenu = GameObject.FindGameObjectWithTag("MainMenu").GetComponent<Canvas>();
        optionsMenu = GameObject.FindGameObjectWithTag("OptionsMenu").GetComponent<Canvas>();
        mainCam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("MainScene", LoadSceneMode.Single);
    }

    public void GotoOptions()
    {
        mainMenu.enabled = false;
        optionsMenu.enabled = true;
    }

    public void GotoMain()
    {
        mainMenu.enabled = true;
        optionsMenu.enabled = false;
        if (SceneManager.GetActiveScene().name != "MainMenu")
        {
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
        }
    }

    public void ChangeVolume(float input)
    {
        AudioListener.volume = input;
    }

    public void EnableAA()
    {
        mainCam.GetComponent<UnityStandardAssets.ImageEffects.Antialiasing>().enabled = !mainCam.GetComponent<UnityStandardAssets.ImageEffects.Antialiasing>().isActiveAndEnabled;
    }

    public void EnableBloom()
    {
        mainCam.GetComponent<UnityStandardAssets.ImageEffects.BloomOptimized>().enabled = !mainCam.GetComponent<UnityStandardAssets.ImageEffects.BloomOptimized>().isActiveAndEnabled;
    }
}
