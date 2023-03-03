using UnityEngine;

public class PowerButtonController : MonoBehaviour {
    [SerializeField] MainMenuManager _mainMenuManager;

    public void OnMouseDown(){
        _mainMenuManager.OpenExitMenu();
    }
}
