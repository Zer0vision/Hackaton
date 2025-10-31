using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public bool PauseGame;
    public GameObject pauseButton;
    public GameObject optionMenu;
    public GameObject pauseCanvas;
    public GameObject pauseMenu;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AudioManager.instance.PlaySFX("Tap");
            if (optionMenu.activeInHierarchy == false)
            {
                if (PauseGame)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
            else
            {
                CloseOption();
            }
        }
    }
    public void Resume()
    {
        pauseCanvas.SetActive(false);
        pauseMenu.SetActive(false);
        optionMenu.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 1f;
        PauseGame = false;
    }
    public void Pause()
    {
        pauseCanvas.SetActive(true);
        pauseMenu.SetActive(true);
        optionMenu.SetActive(false);
        pauseButton.SetActive(false);
        Time.timeScale = 0f;
        PauseGame = true;
    }
    public void CloseOption()
    {
        pauseCanvas.SetActive(true);
        pauseMenu.SetActive(true);
        optionMenu.SetActive(false);
        pauseButton.SetActive(false);
        Time.timeScale = 0f;
        PauseGame = true;
    }
    public void LoadMenu()
    {
        pauseCanvas.SetActive(false);
        pauseMenu.SetActive(false);
        optionMenu.SetActive(false);
        pauseButton.SetActive(true);
        Time.timeScale = 0f;
        SceneManager.LoadScene("Menu");
    }
}
