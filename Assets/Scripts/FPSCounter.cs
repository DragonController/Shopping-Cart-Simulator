using UnityEngine;
using TMPro;

public class FPSCounter : MonoBehaviour {
 
    public float timer, refresh, avgFramerate;
    private TMP_Text _text;
 
    private void Start() {
        _text = GetComponent<TMP_Text>();
    }
 
    private void Update() {
        float timelapse = Time.smoothDeltaTime;
        timer = timer <= 0 ? refresh : timer -= timelapse;
 
        if(timer <= 0) avgFramerate = (int) (1f / timelapse);
        _text.text = avgFramerate + " FPS";
    }
}