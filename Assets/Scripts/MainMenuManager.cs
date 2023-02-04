using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenuManager : MonoBehaviour {
    [SerializeField] private GameObject[] _menus;
    [SerializeField] private Button _subtractButton, _addButton;
    [SerializeField] private TMP_Text _itemCountText, _totalText;
    [SerializeField] private int _maxItems;

    private int _activeMenuIndex;
    private int _itemCount = 1;

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
        _itemCount = Mathf.Clamp(_itemCount + change, 1, _maxItems);

        _subtractButton.interactable = (_itemCount > 1);
        _addButton.interactable = (_itemCount < _maxItems);

        _itemCountText.SetText(_itemCount + "x");

        UpdateTotal();
    }

    private void UpdateTotal() {
        _totalText.SetText("Total: $" + (_itemCount - 1) + ".99");
    }
}
