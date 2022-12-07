using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class S_PauseMenu : MonoBehaviour
{

    public GameObject _pauseMenu;
    public static bool _isPaused;

    void Start()
    {

        _pauseMenu.SetActive(false);

    }


    void Update()
    {
        if(Input.GetButtonDown("MenuPause"))
        {
            if(_isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        _pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        //AudioListener = false;
        _isPaused = true;
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        //AudioListener = true;
        _isPaused = false;
    }

    public void RestartLevel()
    {
        ResumeGame();
        Scene _scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(_scene.name);
        
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu_Scene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
