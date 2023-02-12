using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour {
    private TMP_Text _text;

    private float _timer;

    private void Start() {
        _text = GetComponent<TMP_Text>();

        if (GameCreationParams.isStandardMode) {
            _timer = 120.0f;
        } else {
            _timer = 60.0f;
        }
    }

    private void Update() {
        _timer -= Time.deltaTime;

        if (_timer >= 60.0f) {
            _text.text = Mathf.Floor(_timer / 60.0f) + ":" + (_timer % 60.0f).ToString("00.00");
        } else if (_timer >= 10.0f) {
            _text.text = (_timer % 60.0f).ToString("00.00");
        } else {
            _text.text = (_timer % 60.0f).ToString("0.00");
        }
    }
}
