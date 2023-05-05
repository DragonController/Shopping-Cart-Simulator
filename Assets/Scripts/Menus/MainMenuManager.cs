using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class MainMenuManager : MenuManager {
    [SerializeField] private GameObject[] _menus;
    [SerializeField] private GameObject _exitMenu;
    [SerializeField] private Button[] _mainButtons;
    [SerializeField] private Button _defaultButton, _subtractButton, _addButton, _standardButton, _expressButton, _placeOrderButton;
    [SerializeField] private Button _defaultExitMenuButton;
    [SerializeField] private TMP_Text _itemCountText, _totalText, _ordersListText;
    [SerializeField] private int _maxItems;
    [SerializeField] private float _expressCost;

    [SerializeField] private PlayerInput _playerInput;
    [SerializeField] string _keyboardControlScheme, _controllerControlScheme;
    [SerializeField] private Image _loadingScreenImage;

    private PauseAction _pauseAction;

    private Button _lastButtonSelected, _lastMainButtonSelected;

    private int _activeMenuIndex;

    private bool _exitMenuOpen = false;

    private void Awake() {
        _pauseAction = new PauseAction();
    }

    private void Start() {
        _pauseAction.Pause.Pause.performed += _ => ToggleExitMenu();
        _pauseAction.Pause.NavigateMenuLeft.performed += _ => SetActiveMenu(Mathf.Max(0, _activeMenuIndex - 1));
        _pauseAction.Pause.NavigateMenuRight.performed += _ => SetActiveMenu(Mathf.Min(_menus.Length - 1, _activeMenuIndex + 1));

        _lastButtonSelected = _defaultButton;

        for (int i = 0; i < _menus.Length; i++) {
            if (_menus[i].activeSelf) {
                _activeMenuIndex = i;

                return;
            }
        }
    }

    private void OnEnable() {
        _pauseAction.Enable();

        ChangeItemCount(0);

        if (File.Exists(Application.persistentDataPath + "/LastOrderScore.json")) {
            OrderScore lastOrderScore = new OrderScore();
            OrderScore highOrderScore = new OrderScore();
            string lastOrderJson = System.IO.File.ReadAllText(Application.persistentDataPath + "/LastOrderScore.json");
            string highOrderJson = System.IO.File.ReadAllText(Application.persistentDataPath + "/HighOrderScore.json");
            lastOrderScore = JsonUtility.FromJson<OrderScore>(lastOrderJson);
            highOrderScore = JsonUtility.FromJson<OrderScore>(highOrderJson);

            string lastOrderMode = "???";

            switch (lastOrderScore.mode) {
                case 0:
                    lastOrderMode = "Tutorial";
                    break;
                case 1:
                    lastOrderMode = "Standard";
                    break;
                case 2:
                    lastOrderMode = "Express";
                    break;
            }

            string lastOrderTime;

            if (lastOrderScore.time >= 60.0f) {
                lastOrderTime = Mathf.Floor(lastOrderScore.time / 60.0f) + ":" + Mathf.Floor(lastOrderScore.time % 60.0f).ToString("00") + "." + Mathf.Floor((lastOrderScore.time % 1.0f) * 100.0f).ToString("00");
            } else if (lastOrderScore.time >= 10.0f) {
                lastOrderTime = Mathf.Floor(lastOrderScore.time % 60.0f).ToString("00") + "." + Mathf.Floor((lastOrderScore.time % 1.0f) * 100.0f).ToString("00");
            } else {
                lastOrderTime = Mathf.Floor(lastOrderScore.time % 60.0f).ToString("0") + "." + Mathf.Floor((lastOrderScore.time % 1.0f) * 100.0f).ToString("00");
            }

            if (lastOrderScore.mode == highOrderScore.mode && lastOrderScore.itemCount == highOrderScore.itemCount && lastOrderScore.time == highOrderScore.time) {
                _ordersListText.SetText("<b>Highest scoring (and most recent) order</b>\nMode: " + lastOrderMode + "\nNumber of items: " + lastOrderScore.itemCount + "\nTime Remaining: " + lastOrderTime);
            } else {
                string highOrderMode = "???";

                switch (highOrderScore.mode) {
                    case 0:
                        highOrderMode = "Tutorial";
                        break;
                    case 1:
                        highOrderMode = "Standard";
                        break;
                    case 2:
                        highOrderMode = "Express";
                        break;
                }

                string highOrderTime;

                if (highOrderScore.time >= 60.0f) {
                    highOrderTime = Mathf.Floor(highOrderScore.time / 60.0f) + ":" + Mathf.Floor(highOrderScore.time % 60.0f).ToString("00") + "." + Mathf.Floor((highOrderScore.time % 1.0f) * 100.0f).ToString("00");
                } else if (highOrderScore.time >= 10.0f) {
                    highOrderTime = Mathf.Floor(highOrderScore.time % 60.0f).ToString("00") + "." + Mathf.Floor((highOrderScore.time % 1.0f) * 100.0f).ToString("00");
                } else {
                    highOrderTime = Mathf.Floor(highOrderScore.time % 60.0f).ToString("0") + "." + Mathf.Floor((highOrderScore.time % 1.0f) * 100.0f).ToString("00");
                }

                _ordersListText.SetText("<b>Highest scoring order</b>\nMode: " + highOrderMode + "\nNumber of items: " + highOrderScore.itemCount + "\nTime Remaining: " + highOrderTime + "\n\n<b>Most recent order</b>\nMode: " + lastOrderMode + "\nNumber of items: " + lastOrderScore.itemCount + "\nTime Remaining: " + lastOrderTime);
            }
        }
    }
 
    private void OnDisable() {
        _pauseAction.Disable(); 
    }

    private void Update() {
        if (EventSystem.current.currentSelectedGameObject == null && _lastButtonSelected != null) {
            _lastButtonSelected.Select();
        }

        if (_playerInput.currentControlScheme == _keyboardControlScheme || _playerInput.currentControlScheme == _controllerControlScheme) {
            GameCreationParams.currentControlScheme = _playerInput.currentControlScheme;
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

    public void SetMode(int mode) {
        GameCreationParams.mode = mode;

        UpdateTotal();
    }

    private void UpdateTotal() {
        switch (GameCreationParams.mode) {
            case 0:
                _totalText.SetText("Total: $0.00");
                break;
            case 1:
                _totalText.SetText((GameCreationParams.itemCount - 0.01f).ToString("Total: $0.00"));
                break;
            case 2:
                _totalText.SetText((GameCreationParams.itemCount + _expressCost - 0.01f).ToString("Total: $0.00"));
                break;
        }
    }

    public void LoadSceneAsync(string sceneName) {
        _loadingScreenImage.enabled = true;

        GameCreationParams.itemTypeIndices.Clear();

        for (int i = 0; i < GameCreationParams.itemCount; i++) {
            int randomIndex = Random.Range(0, _maxItems - i);

            for (int j = 0; j <= randomIndex && j < GameCreationParams.itemTypeIndices.Count; j++) {
                if (GameCreationParams.itemTypeIndices[j] <= randomIndex) {
                    randomIndex++;
                }
            }

            GameCreationParams.itemTypeIndices.Add(randomIndex);
            GameCreationParams.itemTypeIndices.Sort();
        }

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

    public void ToggleExitMenu() {
        if (_exitMenuOpen) {
            CloseExitMenu();
        } else {
            OpenExitMenu();
        }
    }

    public void OpenExitMenu() {
        _exitMenuOpen = true;

        ChangMainButtonsInteractable(false);

        _lastMainButtonSelected = _lastButtonSelected;

        _exitMenu.SetActive(true);

        _defaultExitMenuButton.Select();
        _lastButtonSelected = _defaultExitMenuButton;
    }

    public void CloseExitMenu() {
        _exitMenuOpen = false;

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

        if (button == _standardButton || button == _expressButton) {
            Navigation navigation = _placeOrderButton.navigation;

            navigation.selectOnUp = button;

            _placeOrderButton.navigation = navigation;
        }
    }
}
