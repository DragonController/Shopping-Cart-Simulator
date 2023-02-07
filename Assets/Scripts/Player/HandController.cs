using UnityEngine;
using System.Collections.Generic;

public class HandController : MonoBehaviour {
    private Transform _cartTransform, _shoulderTransform, _targetTransform;
    private Vector3 _shoulderRelativePosition;

    private ArticulationBody _wristArticulationBody, _shoulderArticulationBody;
    private List<float> _driveTargets = new List<float>();

    private float _upperArmLength, _lowerArmLength;

    private void Start() {
        _wristArticulationBody = GetComponent<ArticulationBody>();

        _shoulderTransform = transform.parent.parent;
        _shoulderArticulationBody = _shoulderTransform.gameObject.GetComponent<ArticulationBody>();
        _cartTransform = _shoulderTransform.parent;
        
        _shoulderRelativePosition = _shoulderTransform.gameObject.GetComponent<ArticulationBody>().anchorPosition;

        Vector3 _shoulderPosition = _shoulderTransform.TransformPoint(_shoulderRelativePosition);
        Vector3 _elbowPosition = transform.parent.TransformPoint(transform.parent.gameObject.GetComponent<ArticulationBody>().anchorPosition);
        Vector3 _wristPosition = transform.TransformPoint(_wristArticulationBody.anchorPosition);

        _upperArmLength = Vector3.Distance(_shoulderPosition, _elbowPosition);
        _lowerArmLength = Vector3.Distance(_elbowPosition, _wristPosition);

        _targetTransform = _cartTransform.gameObject.GetComponent<CartController>().GetTargetTransform();
    }

    private void FixedUpdate() {
        Vector3 posDifference = _targetTransform.position - _shoulderTransform.TransformPoint(_shoulderRelativePosition);
        float targetDistance = posDifference.magnitude;

        _driveTargets.Clear();
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);

        // Quaternion rotation = Quaternion.Inverse(Quaternion.LookRotation(_targetTransform.position - _shoulderTransform.TransformPoint(_shoulderRelativePosition))) * _cartTransform.rotation;
        Quaternion rotation = Quaternion.LookRotation(_targetTransform.position - _shoulderTransform.TransformPoint(_shoulderRelativePosition));
        float elbowAngle = 0.0f;
        float wristAngle = 0.0f;

        float s = (_upperArmLength + _lowerArmLength + targetDistance) * 0.5f;

        float relativeElbowY = 2.0f * Mathf.Sqrt(s * (s - _upperArmLength) * (s - _lowerArmLength) * (s - targetDistance)) / targetDistance;
        float relativeElbowZ = Mathf.Sqrt(_upperArmLength * _upperArmLength - relativeElbowY * relativeElbowY);

        // print((rotation * Vector3.forward).normalized);
        // print((rotation * Vector3.up).normalized);
        // print((rotation * Vector3.forward).normalized * relativeElbowZ + (rotation * Vector3.up).normalized * relativeElbowY);

        if (_upperArmLength + _lowerArmLength > targetDistance) {
            rotation = Quaternion.LookRotation((rotation * Vector3.forward).normalized * relativeElbowZ + (rotation * Vector3.up).normalized * relativeElbowY, rotation * Vector3.up);
            elbowAngle = 180.0f - Mathf.Acos((_upperArmLength * _upperArmLength + _lowerArmLength * _lowerArmLength - targetDistance * targetDistance) / (2.0f * _upperArmLength * _lowerArmLength)) * Mathf.Rad2Deg;
            wristAngle = -Mathf.Acos((_lowerArmLength * _lowerArmLength + targetDistance * targetDistance - _upperArmLength * _upperArmLength) / (2.0f * _lowerArmLength * targetDistance)) * Mathf.Rad2Deg;
        }

        rotation = Quaternion.Inverse(rotation) * _cartTransform.rotation;

        // _driveTargets.Add(-(180.0f - ((360.0f - ((rotation.eulerAngles.x + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad);
        // _driveTargets.Add((180.0f - ((360.0f - ((rotation.eulerAngles.y + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad);
        // print((180.0f - ((360.0f - ((rotation.eulerAngles.z + 180.0f) % 360.0f)) % 360.0f)));

        // print(Quaternion.Euler(123.0f, 34.0f, 65.76f).eulerAngles);
        // print(new Vector3(180.0f - 123.0f, 180.0f + 34.0f, 180.0f + 65.76f));

        // print(rotation.eulerAngles);

        // print(posDifference.y);
        // print(targetDistance);

        rotation = Quaternion.Euler(Mathf.Atan2(posDifference.y, targetDistance) * Mathf.Rad2Deg, 0.0f, Mathf.Atan2(posDifference.x, posDifference.z) * Mathf.Rad2Deg - _cartTransform.eulerAngles.y);

        _shoulderArticulationBody.anchorRotation = rotation;

        // _driveTargets.Add(0.0f);
        // _driveTargets.Add(0.0f);
        // _driveTargets.Add(0.0f);
        // _driveTargets.Add(-Mathf.Sqrt(Mathf.Abs(180.0f - ((360.0f - ((rotation.eulerAngles.y + 180.0f) % 360.0f)) % 360.0f))) * Mathf.Deg2Rad);
        // _driveTargets.Add(-(180.0f - ((360.0f - ((rotation.eulerAngles.z + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad);
        _driveTargets.Add((180.0f - ((360.0f - ((elbowAngle + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad);
        _driveTargets.Add((180.0f - ((360.0f - ((wristAngle + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad);
        _driveTargets.Add(0.0f);
        _driveTargets.Add(0.0f);
        _wristArticulationBody.SetDriveTargets(_driveTargets);
    }
}
