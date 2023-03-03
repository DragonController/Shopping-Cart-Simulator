using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class PauseMenuManager : MenuManager {
    [SerializeField] private GameObject _pauseMenu, _restartMenu, _exitMenu;
    [SerializeField] private Button[] _mainButtons;
    [SerializeField] private Button _defaultButton, _restartMenuButton, _defaultRestartMenuButton, _exitMenuButton, _defaultExitMenuButton;

    [SerializeField] private Image _loadingScreenImage;

    private PauseAction _pauseAction;

    private Button _lastButtonSelected;

    private int _paused = 0;
    private int _openSubMenuIndex;

    private void Awake() {
        _pauseAction = new PauseAction();
    }

    private void Start() {
        _pauseAction.Pause.Pause.performed += _ => TogglePause();
    }

    private void OnEnable() {
        _pauseAction.Enable();
    }

    private void OnDisable() {
        _pauseAction.Disable(); 
    }

    private void Update() {
        if (EventSystem.current.currentSelectedGameObject == null && _lastButtonSelected != null) {
            _lastButtonSelected.Select();
        }
    }

    private void TogglePause() {
        switch (_paused) {
            case 0:
                Pause();
                break;
            case 1:
                UnPause();
                break;
            case 2:
                switch (_openSubMenuIndex) {
                    case 0:
                        CloseRestartMenu();
                        break;
                    case 1:
                        CloseExitMenu();
                        break;
                }
                break;
        }
    }

    private void Pause() {
        Time.timeScale = 0.0f;
        _pauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;

        _defaultButton.Select();
        _lastButtonSelected = _defaultButton;

        _paused = 1;
    }

    public void UnPause() {
        Cursor.lockState = CursorLockMode.Locked;
        _pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;

        _paused = 0;
    }

    private void ChangMainButtonsInteractable(bool interactable) {
        foreach (Button button in _mainButtons) {
            button.interactable = interactable;
        }
    }

    public void OpenRestartMenu() {
        ChangMainButtonsInteractable(false);
        
        _restartMenu.SetActive(true);

        _defaultRestartMenuButton.Select();
        _lastButtonSelected = _defaultRestartMenuButton;

        _openSubMenuIndex = 0;
        _paused = 2;
    }

    public void CloseRestartMenu() {
        _restartMenu.SetActive(false);

        _restartMenuButton.Select();
        _lastButtonSelected = _restartMenuButton;

        ChangMainButtonsInteractable(true);

        _paused = 1;
    }

    public void OpenExitMenu() {
        ChangMainButtonsInteractable(false);
        
        _exitMenu.SetActive(true);

        _defaultExitMenuButton.Select();
        _lastButtonSelected = _defaultExitMenuButton;

        _openSubMenuIndex = 1;
        _paused = 2;
    }

    public void CloseExitMenu() {
        _exitMenu.SetActive(false);

        _exitMenuButton.Select();
        _lastButtonSelected = _exitMenuButton;

        ChangMainButtonsInteractable(true);

        _paused = 1;
    }

    public void ReloadSceneAsync() {
        _loadingScreenImage.enabled = true;
        Time.timeScale = 1.0f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadSceneAsync(string sceneName) {
        _loadingScreenImage.enabled = true;
        Time.timeScale = 1.0f;

        SceneManager.LoadSceneAsync(sceneName);
    }

    public void ExitGame() {
        Application.Quit();
    }

    public override void SetLastSelectedButton(Button button) {
        _lastButtonSelected = button;
    }
}
