using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] private GameObject[] _menus;
    [SerializeField] private Button _subtractButton, _addButton;
    [SerializeField] private TMP_Text _itemCountText, _totalText;
    [SerializeField] private int _maxItems;

    [SerializeField] private Image _loadingScreenImage;

    private int _activeMenuIndex;

    private void Start() {
        for (int i = 0; i < _menus.Length; i++) {
            if (_menus[i].activeSelf) {
                _activeMenuIndex = i;

                return;
            }
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
}
