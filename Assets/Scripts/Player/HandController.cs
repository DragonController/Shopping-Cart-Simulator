using UnityEngine;
using System.Collections.Generic;

public class HandController : MonoBehaviour {
    private Transform _shoulderTransform, _targetTransform;

    private ArticulationBody articulationBody;
    private List<float> _driveTargets = new List<float>();

    private void Start() {
        _shoulderTransform = transform.parent.parent;

        _targetTransform = transform.parent.parent.parent.gameObject.GetComponent<CartController>().GetTargetTransform();

        articulationBody = GetComponent<ArticulationBody>();
    }

    private void FixedUpdate() {
        _driveTargets.Clear();
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(((Quaternion.LookRotation(_targetTransform.position - _shoulderTransform.position).eulerAngles.x + 90.0f) % 360.0f) * Mathf.Deg2Rad);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        articulationBody.SetDriveTargets(_driveTargets);
        print(((Quaternion.LookRotation(_targetTransform.position - _shoulderTransform.position).eulerAngles.x + 90.0f) % 360.0f));
    }
}
