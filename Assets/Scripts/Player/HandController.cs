using UnityEngine;
using System.Collections.Generic;

public class HandController : MonoBehaviour {
    private CartController _cartController;
    private Transform _cartTransform, _shoulderTransform, _targetTransform;
    private Vector3 _shoulderRelativePosition;

    private ArticulationBody _wristArticulationBody;
    private List<float> _driveTargets = new List<float>();
    private List<float> _driveTargetVelocities = new List<float>();

    private float _upperArmLength, _lowerArmLength;

    private void Start() {
        _wristArticulationBody = GetComponent<ArticulationBody>();

        _shoulderTransform = transform.parent.parent;

        _cartTransform = _shoulderTransform.parent.parent;
        
        _shoulderRelativePosition = _shoulderTransform.gameObject.GetComponent<ArticulationBody>().anchorPosition;

        Vector3 _shoulderPosition = _shoulderTransform.TransformPoint(_shoulderRelativePosition);
        Vector3 _elbowPosition = transform.parent.TransformPoint(transform.parent.gameObject.GetComponent<ArticulationBody>().anchorPosition);
        Vector3 _wristPosition = transform.TransformPoint(_wristArticulationBody.anchorPosition);

        _upperArmLength = Vector3.Distance(_shoulderPosition, _elbowPosition);
        _lowerArmLength = Vector3.Distance(_elbowPosition, _wristPosition);

        _cartController = _cartTransform.gameObject.GetComponent<CartController>();
        _targetTransform = _cartController.GetTargetTransform();

        _wristArticulationBody.GetDriveTargets(_driveTargets);
        _wristArticulationBody.GetDriveTargetVelocities(_driveTargetVelocities);
    }

    private void FixedUpdate() {
        Vector3 posDifference = _targetTransform.position - _shoulderTransform.TransformPoint(_shoulderRelativePosition);
        float targetDistance = posDifference.magnitude;

        Vector2 upperArmAngles = new Vector2(Mathf.Atan2(posDifference.y, targetDistance) * Mathf.Rad2Deg, Mathf.Atan2(posDifference.x, posDifference.z) * Mathf.Rad2Deg - _cartTransform.eulerAngles.y);
        float elbowAngle = 0.0f;
        float wristAngle = Mathf.Atan2(posDifference.y, targetDistance) * Mathf.Rad2Deg;

        if (_upperArmLength + _lowerArmLength > targetDistance) {
            upperArmAngles = new Vector2((Mathf.Atan2(posDifference.y, targetDistance) + Mathf.Acos((_upperArmLength * _upperArmLength + targetDistance * targetDistance - _lowerArmLength * _lowerArmLength) / (2.0f * _upperArmLength * targetDistance))) * Mathf.Rad2Deg, Mathf.Atan2(posDifference.x, posDifference.z) * Mathf.Rad2Deg - _cartTransform.eulerAngles.y);
            elbowAngle = 180.0f - Mathf.Acos((_upperArmLength * _upperArmLength + _lowerArmLength * _lowerArmLength - targetDistance * targetDistance) / (2.0f * _upperArmLength * _lowerArmLength)) * Mathf.Rad2Deg;
            wristAngle = (-Mathf.Acos((_lowerArmLength * _lowerArmLength + targetDistance * targetDistance - _upperArmLength * _upperArmLength) / (2.0f * _lowerArmLength * targetDistance)) + Mathf.Atan2(posDifference.y, targetDistance)) * Mathf.Rad2Deg;
        }

        _driveTargets[6] += (180.0f - ((360.0f - ((upperArmAngles.y - _driveTargets[6] * Mathf.Rad2Deg + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        _driveTargets[8] += (180.0f - ((360.0f - ((-upperArmAngles.x - _driveTargets[8] * Mathf.Rad2Deg + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        _driveTargets[10] = (180.0f - ((360.0f - ((elbowAngle + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        _driveTargets[11] = (180.0f - ((360.0f - ((wristAngle + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        _wristArticulationBody.SetDriveTargets(_driveTargets);
        
        Vector2 look = _cartController.GetLook() * Time.fixedDeltaTime;

        _driveTargetVelocities[7] = look.x;
        _driveTargetVelocities[9] = -look.y;

        _wristArticulationBody.SetDriveTargetVelocities(_driveTargetVelocities);
    }
}
