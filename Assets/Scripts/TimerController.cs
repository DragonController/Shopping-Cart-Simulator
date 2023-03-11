using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TimerController : MonoBehaviour {
    [SerializeField] private TMP_Text _minutesAndSeconds, _centiseconds;
    [SerializeField] private GameObject _timedOutScreen;
    [SerializeField] private PauseMenuManager _pauseMenuManager;
    [SerializeField] private Button _defaultButton;

    private float _timer;

    private bool _timedOut = false;

    private void Start() {
        switch (GameCreationParams.mode) {
            case 0:
                _timer = 0.0f;
                break;
            case 1:
                _timer = 120.0f;
                break;
            case 2:
                _timer = 60.0f;
                break;
        }
    }

    private void Update() {
        if (GameCreationParams.mode == 0) {
            _timer += Time.deltaTime;
        
            UpdateDisplay();

            return;
        }

        _timer = Mathf.Max(0.0f, _timer - Time.deltaTime);
        
        UpdateDisplay();

        if (_timer <= 0.0f && !_timedOut) {
            _timedOut = true;

            Time.timeScale = 0.0f;
            _timedOutScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;

            _defaultButton.Select();
            _pauseMenuManager.SetLastSelectedButton(_defaultButton);
        }
    }

    private void UpdateDisplay() {
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
