using UnityEngine;
using System.Collections;

public class PauseMenu : MonoBehaviour {

    Canvas optionsUI;
    bool isPaused = false;

    void Awake()
    {
        optionsUI = transform.FindChild("OptionsUI").GetComponent<Canvas>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPaused = !isPaused;
        }

        if (isPaused)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
        optionsUI.enabled = isPaused;
    }
}
