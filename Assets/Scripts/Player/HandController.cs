using UnityEngine;
using System.Collections.Generic;

public class HandController : MonoBehaviour {
    private Transform _cartTransform, _shoulderTransform, _targetTransform;
    private Vector3 _shoulderRelativePosition;

    private ArticulationBody _articulationBody;
    private List<float> _driveTargets = new List<float>();

    private float _upperArmLength, _lowerArmLength;

    private void Start() {
        _articulationBody = GetComponent<ArticulationBody>();

        _shoulderTransform = transform.parent.parent;
        _cartTransform = _shoulderTransform.parent;
        
        _shoulderRelativePosition = _shoulderTransform.gameObject.GetComponent<ArticulationBody>().anchorPosition;

        Vector3 _shoulderPosition = _shoulderTransform.TransformPoint(_shoulderRelativePosition);
        Vector3 _elbowPosition = transform.parent.TransformPoint(transform.parent.gameObject.GetComponent<ArticulationBody>().anchorPosition);
        Vector3 _wristPosition = transform.TransformPoint(_articulationBody.anchorPosition);

        _upperArmLength = Vector3.Distance(_shoulderPosition, _elbowPosition);
        _lowerArmLength = Vector3.Distance(_elbowPosition, _wristPosition);

        _targetTransform = _cartTransform.gameObject.GetComponent<CartController>().GetTargetTransform();
    }

    private void FixedUpdate() {
        float targetDistance = Vector3.Distance(_targetTransform.position, _shoulderTransform.TransformPoint(_shoulderRelativePosition));

        _driveTargets.Clear();
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);

        Quaternion rotation = Quaternion.Inverse(Quaternion.LookRotation(_targetTransform.position - _shoulderTransform.TransformPoint(_shoulderRelativePosition))) * _cartTransform.rotation;

        float s = (_upperArmLength + _lowerArmLength + targetDistance) * 0.5f;

        float relativeElbowY = 2.0f * Mathf.Sqrt(s * (s - _upperArmLength) * (s - _lowerArmLength) * (s - targetDistance)) / targetDistance;
        float relativeElbowZ = Mathf.Sqrt(_upperArmLength * _upperArmLength - relativeElbowY * relativeElbowY);

        // print(relativeElbowY);
        // print(relativeElbowZ);

        if (_upperArmLength + _lowerArmLength > targetDistance) {
            rotation = Quaternion.LookRotation((rotation * Vector3.forward).normalized * relativeElbowZ + (rotation * Vector3.up).normalized * -relativeElbowY, rotation * Vector3.up);
            print(rotation.eulerAngles);
        }

        // print((rotation * Vector3.forward).normalized * relativeElbowZ);
        // print((rotation * Vector3.up).normalized * relativeElbowY);
        // print((rotation * Vector3.forward).normalized * relativeElbowZ + (rotation * Vector3.up).normalized * relativeElbowY);

        // print(Quaternion.LookRotation((rotation * Vector3.forward).normalized * relativeElbowZ + (rotation * Vector3.up).normalized * relativeElbowY, rotation * Vector3.up));

        _driveTargets.Add(-(180.0f - ((360.0f - ((rotation.eulerAngles.z + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad);
        _driveTargets.Add((180.0f - ((360.0f - ((rotation.eulerAngles.x + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad);
        _driveTargets.Add((180.0f - ((360.0f - ((rotation.eulerAngles.y + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _articulationBody.SetDriveTargets(_driveTargets);
    }
}
