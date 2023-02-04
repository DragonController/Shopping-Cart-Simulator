using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfController : MonoBehaviour {
    // [SerializeField] private float _breakDegrees;
    private float _breakDegrees = 22.5f;

    private HingeJoint _hingeJoint;

    private void Start() {
        _hingeJoint = GetComponent<HingeJoint>();
    }

    private void FixedUpdate() {
        if (transform.localEulerAngles.x < 360.0f - _breakDegrees && transform.localEulerAngles.x >= 180.0f) {
            Destroy(_hingeJoint);
        }
    }
}
