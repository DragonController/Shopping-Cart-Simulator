using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfController : MonoBehaviour {
    // [SerializeField] private float _breakDegrees;
    
    private bool _connectedToBackboard = true;

    private float _breakDegrees = 22.5f;

    private void FixedUpdate() {
        if (_connectedToBackboard && transform.localEulerAngles.x < 360.0f - _breakDegrees && transform.localEulerAngles.x >= 180.0f) {
            transform.SetParent(transform.parent.parent, true);

            _connectedToBackboard = false;
        }
    }
}
