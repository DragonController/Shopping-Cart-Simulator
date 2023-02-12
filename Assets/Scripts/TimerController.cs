using UnityEngine;
using TMPro;

public class TimerController : MonoBehaviour {
    [SerializeField] private TMP_Text _minutesAndSeconds, _centiseconds;

    private float _timer;

    private void Start() {
        if (GameCreationParams.isStandardMode) {
            _timer = 120.0f;
        } else {
            _timer = 60.0f;
        }
    }

    private void Update() {
        _timer = Mathf.Max(0.0f, _timer - Time.deltaTime);

        if (_timer >= 60.0f) {
            _minutesAndSeconds.text = Mathf.Floor(_timer / 60.0f) + ":" + Mathf.Floor(_timer % 60.0f).ToString("00");
        } else if (_timer >= 10.0f) {
            _minutesAndSeconds.text = Mathf.Floor(_timer % 60.0f).ToString("00");
        } else {
            _minutesAndSeconds.text = Mathf.Floor(_timer % 60.0f).ToString("0");
        }

        _centiseconds.text = Mathf.Floor((_timer % 1.0f) * 100.0f).ToString("00");
    }
}
