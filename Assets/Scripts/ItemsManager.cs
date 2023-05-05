using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ItemsManager : MonoBehaviour {
    [SerializeField] private string[] _itemNames;
    [SerializeField] private Transform _itemsParentTransform;
    [SerializeField] private GameObject _firstListLineGameObject, _winnerScreen;
    [SerializeField] private PauseMenuManager _pauseMenuManager;
    [SerializeField] private Button _defaultButton;
    [SerializeField] private TMP_Text _winnerText, _minutesAndSeconds, _centiseconds;
    [SerializeField] private TimerController _timerController;

    private Dictionary<GameObject, int> _items = new Dictionary<GameObject, int>();
    private List<int> _remainingItemTypeIndices = new List<int>();
    private TMP_Text[] _listLines;

    private void Start() {
        foreach (Transform itemTransform in _itemsParentTransform) {
            _items.Add(itemTransform.gameObject, itemTransform.gameObject.GetComponent<ItemController>().GetTypeIndex());
        }

        foreach (int itemTypeIndex in GameCreationParams.itemTypeIndices) {
            _remainingItemTypeIndices.Add(itemTypeIndex);
        }

        Transform parentTransform = _firstListLineGameObject.transform.parent;
        
        _listLines = new TMP_Text[GameCreationParams.itemCount];

        _listLines[0] = _firstListLineGameObject.GetComponent<TMP_Text>();
        _listLines[0].SetText("1x " + _itemNames[GameCreationParams.itemTypeIndices[0]]);

        for (int i = 1; i < GameCreationParams.itemCount; i++) {
            _listLines[i] = Instantiate(_firstListLineGameObject, parentTransform).GetComponent<TMP_Text>();
            _listLines[i].SetText("1x " + _itemNames[GameCreationParams.itemTypeIndices[i]]);
        }
    }

    public Dictionary<GameObject, int> GetItems() {
        return _items;
    }
    
    public List<int> GetRemainingItemTypeIndices() {
        return _remainingItemTypeIndices;
    }

    public void RemoveItem(GameObject item) {
        int itemIndex = _items[item];

        _listLines[GameCreationParams.itemTypeIndices.IndexOf(itemIndex)].fontStyle = FontStyles.Strikethrough;

        _remainingItemTypeIndices.RemoveAll(i => i == itemIndex);

        if (_remainingItemTypeIndices.Count == 0) {
            Time.timeScale = 0.0f;
            _winnerScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            _defaultButton.Select();
            _pauseMenuManager.SetLastSelectedButton(_defaultButton);

            OrderScore orderScore = new OrderScore();
            orderScore.itemCount = GameCreationParams.itemCount;
            orderScore.mode = GameCreationParams.mode;

            if (GameCreationParams.mode == 0) {
                orderScore.time = 0.0f;
                _winnerText.text = "Congradulations!\nYou completed the tutorial in " + _minutesAndSeconds.text + "." + _centiseconds.text;
            } else {
                orderScore.time = _timerController.GetRemainingTime();
                _winnerText.text = "Congradulations!\nYou completed your shopping list with " + _minutesAndSeconds.text + "." + _centiseconds.text + " to spare";
            }

            string json = JsonUtility.ToJson(orderScore);
            System.IO.File.WriteAllText(Application.persistentDataPath + "/OrderScores.json", json);
        }
    }
}
