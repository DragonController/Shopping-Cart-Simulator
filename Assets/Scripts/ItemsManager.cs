using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ItemsManager : MonoBehaviour {
    [SerializeField] private string[] _itemNames;
    [SerializeField] private GameObject _firstListLineGameObject;

    private TMP_Text[] _listLines;

    private void Start() {
        Transform parentTransform = _firstListLineGameObject.transform.parent;
        
        _listLines = new TMP_Text[GameCreationParams.itemCount];

        _listLines[0] = _firstListLineGameObject.GetComponent<TMP_Text>();
        _listLines[0].SetText("1x " + _itemNames[GameCreationParams.itemIndices[0]]);

        for (int i = 1; i < GameCreationParams.itemCount; i++) {
            _listLines[i] = Instantiate(_firstListLineGameObject, parentTransform).GetComponent<TMP_Text>();
            _listLines[i].SetText("1x " + _itemNames[GameCreationParams.itemIndices[i]]);
        }
    }

    private void Update() {
        foreach (int index in GameCreationParams.itemIndices) {
            // print(index);
        }
    }
}
