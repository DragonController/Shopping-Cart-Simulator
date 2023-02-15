using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ItemsManager : MonoBehaviour {
    [SerializeField] private string[] _itemNames;
    [SerializeField] private Transform _itemsParentTransform;
    [SerializeField] private GameObject _firstListLineGameObject;

    private Dictionary<GameObject, int> _items = new Dictionary<GameObject, int>();
    private TMP_Text[] _listLines;

    private void Start() {
        foreach (Transform itemTransform in _itemsParentTransform) {
            _items.Add(itemTransform.gameObject, itemTransform.gameObject.GetComponent<ItemController>().GetTypeIndex());
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

    private void Update() {
        foreach (int index in GameCreationParams.itemTypeIndices) {
            // print(index);
        }
    }

    public Dictionary<GameObject, int> GetItems() {
        return _items;
    }
}
