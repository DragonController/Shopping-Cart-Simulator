using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour {
    private TMP_Text _text;
 
    private void Start() {
        _text = GetComponent<TMP_Text>();
    }
 
    private void Update() {
        _text.text = Mathf.Round(1.0f / Time.unscaledDeltaTime) + " FPS";
    }
}
