using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonController : MonoBehaviour, ISelectHandler {
    [SerializeField] private MenuManager _menuManager;
    [SerializeField] private bool _isXButton;
    
    private MainMenuManager _mainMenuManager;
    private PauseMenuManager _pauseMenuManager;

    private Button _button;

    private void Start() {
        _button = GetComponent<Button>();
    }

    public void OnSelect(BaseEventData eventData) {
        if (_isXButton) {
            EventSystem.current.SetSelectedGameObject(null);
        }

        _menuManager.SetLastSelectedButton(_button);
    }
}
