using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;

public class MainMenuManager : MenuManager {
    [SerializeField] private GameObject[] _menus;
    [SerializeField] private GameObject _exitMenu;
    [SerializeField] private Button[] _mainButtons;
    [SerializeField] private Button _subtractButton, _addButton;
    [SerializeField] private Button _defaultExitMenuButton;
    [SerializeField] private TMP_Text _itemCountText, _totalText;
    [SerializeField] private int _maxItems;

    [SerializeField] private Image _loadingScreenImage;

    private PauseAction _pauseAction;

    private Button _lastButtonSelected, _lastMainButtonSelected;

    private int _activeMenuIndex;

    private void Awake() {
        _pauseAction = new PauseAction();
    }

    private void Start() {
        _pauseAction.Pause.Pause.performed += _ => CloseExitMenu();

        for (int i = 0; i < _menus.Length; i++) {
            if (_menus[i].activeSelf) {
                _activeMenuIndex = i;

                return;
            }
        }
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

    public void SetActiveMenu(int menuIndex) {
        if (menuIndex == _activeMenuIndex) {
            return;
        }

        _menus[_activeMenuIndex].SetActive(false);
        _menus[menuIndex].SetActive(true);

        _activeMenuIndex = menuIndex;
    }

    public void ChangeItemCount(int change) {
        GameCreationParams.itemCount = Mathf.Clamp(GameCreationParams.itemCount + change, 1, _maxItems);

        _subtractButton.interactable = (GameCreationParams.itemCount > 1);
        _addButton.interactable = (GameCreationParams.itemCount < _maxItems);

        _itemCountText.SetText(GameCreationParams.itemCount + "x");

        UpdateTotal();
    }

    public void SetIsStandardMode(bool isStandardMode) {
        GameCreationParams.isStandardMode = isStandardMode;
    }

    private void UpdateTotal() {
        _totalText.SetText("Total: $" + (GameCreationParams.itemCount - 1) + ".99");
    }

    public void LoadSceneAsync(string sceneName) {
        _loadingScreenImage.enabled = true;

        SceneManager.LoadSceneAsync(sceneName);
    }

    private void ChangMainButtonsInteractable(bool interactable) {
        foreach (Button button in _mainButtons) {
            button.interactable = interactable;
        }

        if (interactable) {
            _subtractButton.interactable = (GameCreationParams.itemCount > 1);
            _addButton.interactable = (GameCreationParams.itemCount < _maxItems);
        } else {
            _subtractButton.interactable = false;
            _addButton.interactable = false;
        }
    }

    public void OpenExitMenu() {
        ChangMainButtonsInteractable(false);

        _lastMainButtonSelected = _lastButtonSelected;

        _exitMenu.SetActive(true);

        _defaultExitMenuButton.Select();
        _lastButtonSelected = _defaultExitMenuButton;
    }

    public void CloseExitMenu() {
        ChangMainButtonsInteractable(true);

        _exitMenu.SetActive(false);

        _lastMainButtonSelected.Select();
        _lastButtonSelected = _lastMainButtonSelected;
    }

    public void ExitGame() {
        Application.Quit();
    }

    public override void SetLastSelectedButton(Button button) {
        _lastButtonSelected = button;
    }
}