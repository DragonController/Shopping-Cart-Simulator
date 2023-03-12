using UnityEngine;

public class TutorialMenuController : MonoBehaviour {
    [SerializeField] GameObject _tutorialMenu, _keyboardTextGameObject, _controllerTextGameObject;
    [SerializeField] string _keyboardControlScheme, _controllerControlScheme;

    private bool _tutorialMenuOpen = false;

    private void Start() {
        if (GameCreationParams.mode != 0) {
            return;
        }

        print(GameCreationParams.currentControlScheme);
        
        _keyboardTextGameObject.SetActive(false);
        _controllerTextGameObject.SetActive(false);

        // if (GameCreationParams.currentControlScheme == _keyboardControlScheme) {
        //     _keyboardTextGameObject.SetActive(true);
        // }
        if (GameCreationParams.currentControlScheme == _controllerControlScheme) {
            _controllerTextGameObject.SetActive(true);
        } else {
            _keyboardTextGameObject.SetActive(true);
        }

        _tutorialMenu.SetActive(true);
        _tutorialMenuOpen = true;

        Time.timeScale = 0.0f;
    }

    public void CloseTutorialMenu() {
        Cursor.lockState = CursorLockMode.Locked;
        _tutorialMenu.SetActive(false);
        _tutorialMenuOpen = false;

        Time.timeScale = 1.0f;
    }

    public bool IsTutorialMenuOpen() {
        return _tutorialMenuOpen;
    }
}
