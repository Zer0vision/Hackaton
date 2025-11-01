using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }
    public static bool IsPaused => Instance != null && Instance._paused;

    [Header("UI")]
    public GameObject pauseRoot;       // корневая панель паузы (активна ТОЛЬКО когда пауза)
    public Selectable firstSelected;   // что выделить при входе в паузу
    public Button pauseButton;         // кнопка "Pause" в HUD (необязательно)

    bool _paused;

    void Awake()
    {
        if (Instance && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        // Привяжем кнопку к нашему обработчику и уберём старые слушатели
        if (pauseButton)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(OnPauseButtonClicked);
        }
        ApplyStateToUI();
    }

    void Update()
    {
        // ОДНО место, где решаем про Esc/Start
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
        else if (Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame)
            TogglePause();
    }

    public void OnPauseButtonClicked() => TogglePause();

    public void TogglePause() => SetPaused(!_paused);

    public void SetPaused(bool value)
    {
        if (_paused == value) { ApplyStateToUI(); return; } // идемпотентность
        _paused = value;
        Time.timeScale = _paused ? 0f : 1f;
        ApplyStateToUI();
    }

    void ApplyStateToUI()
    {
        if (pauseRoot) pauseRoot.SetActive(_paused);

        // Навигация по UI
        var es = EventSystem.current;
        if (es)
        {
            if (_paused && firstSelected) es.SetSelectedGameObject(firstSelected.gameObject);
            else if (!_paused) es.SetSelectedGameObject(null);
        }
    }
}
