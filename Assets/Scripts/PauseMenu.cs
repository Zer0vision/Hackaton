using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject pauseCanvas;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private InputActionReference pauseAction;

    private bool isPaused;

    private void OnEnable()
    {
        SubscribeToInput();
    }

    private void OnDisable()
    {
        UnsubscribeFromInput();
        SetPauseState(false);
    }

    public void Resume()
    {
        SetPauseState(false);
    }

    public void Pause()
    {
        SetPauseState(true);
    }

    public void CloseOptions()
    {
        SetPauseState(true);
        optionsMenu?.SetActive(false);
        pauseMenu?.SetActive(true);
    }

    public void LoadMenu()
    {
        SetPauseState(false);
        SceneManager.LoadScene("Menu");
    }

    private void SubscribeToInput()
    {
        if (pauseAction == null || pauseAction.action == null)
        {
            return;
        }

        pauseAction.action.performed += OnPausePerformed;
        if (!pauseAction.action.enabled)
        {
            pauseAction.action.Enable();
        }
    }

    private void UnsubscribeFromInput()
    {
        if (pauseAction == null || pauseAction.action == null)
        {
            return;
        }

        pauseAction.action.performed -= OnPausePerformed;
        if (pauseAction.action.enabled)
        {
            pauseAction.action.Disable();
        }
    }

    private void OnPausePerformed(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (optionsMenu != null && optionsMenu.activeSelf)
        {
            CloseOptions();
            return;
        }

        if (isPaused)
        {
            Resume();
        }
        else
        {
            Pause();
        }

        if (AudioManager.TryGetInstance(out var manager))
        {
            manager.PlaySFX("Tap");
        }
    }

    private void SetPauseState(bool pause)
    {
        isPaused = pause;

        pauseCanvas?.SetActive(pause);
        pauseMenu?.SetActive(pause);
        optionsMenu?.SetActive(false);
        pauseButton?.SetActive(!pause);

        Time.timeScale = pause ? 0f : 1f;
    }
}
