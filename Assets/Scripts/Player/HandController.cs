using UnityEngine;
using System.Collections.Generic;

public class HandController : MonoBehaviour {
    private CartController _cartController;
    private Transform _cartTransform, _shoulderSwivelTransform, _shoulderTransform, _elbowTransform, _targetTransform, _maxTargetTransform;
    private Vector3 _shoulderRelativePosition, _elbowRelativePosition;

    private ArticulationBody _wristArticulationBody;
    private List<float> _driveTargets = new List<float>();
    private List<float> _driveTargetVelocities = new List<float>();

    private float _upperArmLength, _lowerArmLength;

    private Vector2 _look;

    private Vector3 _maxTargetLocalPos, _minTargetLocalPos;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        _wristArticulationBody = GetComponent<ArticulationBody>();

        _elbowTransform = transform.parent;
        _shoulderTransform = _elbowTransform.parent;
        _shoulderSwivelTransform = _shoulderTransform.parent;

        _cartTransform = _shoulderTransform.parent.parent;
        
        _shoulderRelativePosition = _shoulderTransform.gameObject.GetComponent<ArticulationBody>().anchorPosition;
        _elbowRelativePosition = _elbowTransform.gameObject.GetComponent<ArticulationBody>().anchorPosition;

        Vector3 shoulderPosition = _shoulderTransform.TransformPoint(_shoulderRelativePosition);
        Vector3 elbowPosition = _elbowTransform.TransformPoint(_elbowRelativePosition);
        Vector3 wristPosition = transform.TransformPoint(_wristArticulationBody.anchorPosition);

        _upperArmLength = Vector3.Distance(shoulderPosition, elbowPosition);
        _lowerArmLength = Vector3.Distance(elbowPosition, wristPosition);

        _cartController = _cartTransform.gameObject.GetComponent<CartController>();

        _targetTransform = _cartController.GetTargetTransform();
        _maxTargetTransform = _cartController.GetMaxTargetTransform();

        _wristArticulationBody.GetDriveTargets(_driveTargets);
        _wristArticulationBody.GetDriveTargetVelocities(_driveTargetVelocities);

        _minTargetLocalPos = _cartController.GetMinTargetTransform().localPosition;
        _maxTargetLocalPos = _maxTargetTransform.localPosition;
    }

    private void FixedUpdate() {
        _targetTransform.localPosition = Vector3.Lerp(_maxTargetLocalPos, _minTargetLocalPos, _cartController.GetRetractDistance());

        Vector3 targetPosition = _targetTransform.position;
        Vector3 maxTargetPosition = _maxTargetTransform.position;
        Vector3 shoulderPosition = _shoulderTransform.TransformPoint(_shoulderRelativePosition);
        Vector3 shoulderDifference = targetPosition - shoulderPosition;
        Vector3 shoulderMaxDifference = maxTargetPosition - shoulderPosition;
        float shoulderDistance = shoulderDifference.magnitude;
        Vector3 elbowPosition = _elbowTransform.TransformPoint(_elbowRelativePosition);
        Vector3 elbowDifference = targetPosition - elbowPosition;
        float elbowDistance = elbowDifference.magnitude;

        float oldShoulderAngle = 90.0f - _shoulderTransform.localEulerAngles.x;
        float oldElbowAngle = _elbowTransform.localEulerAngles.x;

        if (_shoulderTransform.localEulerAngles.y + _shoulderTransform.localEulerAngles.z > 180.0f) {
            oldShoulderAngle = _shoulderTransform.localEulerAngles.x - 90.0f;
        }
        if (_elbowTransform.localEulerAngles.y + _elbowTransform.localEulerAngles.z > 180.0f) {
            oldElbowAngle = 180.0f - _elbowTransform.localEulerAngles.x;
        }

        Vector2 newShoulderAngles = new Vector2(Mathf.Atan2(shoulderMaxDifference.x, shoulderMaxDifference.z) * Mathf.Rad2Deg - _cartTransform.eulerAngles.y, -Mathf.Asin(shoulderMaxDifference.y / shoulderMaxDifference.magnitude) * Mathf.Rad2Deg);

        if (_upperArmLength + _lowerArmLength > shoulderDistance) {
            newShoulderAngles.y -= Mathf.Acos((_upperArmLength * _upperArmLength + shoulderDistance * shoulderDistance - _lowerArmLength * _lowerArmLength) / (2.0f * _upperArmLength * shoulderDistance)) * Mathf.Rad2Deg;
        }

        float newElbowAngle = oldShoulderAngle - Mathf.Asin(elbowDifference.y / elbowDistance) * Mathf.Rad2Deg;
        float newWristAngle = oldShoulderAngle - oldElbowAngle;

        if (new Vector2(shoulderDifference.x, shoulderDifference.z).magnitude < Vector2.Distance(new Vector2(elbowPosition.x, elbowPosition.z), new Vector2(shoulderPosition.x, shoulderPosition.z))) {
            if (Mathf.Abs(oldShoulderAngle) < 90.0f) {
                newElbowAngle = oldShoulderAngle + Mathf.Asin(elbowDifference.y / elbowDistance) * Mathf.Rad2Deg + 180.0f;
            }
        }

        _driveTargets[6] += (180.0f - ((360.0f - ((newShoulderAngles.x - _driveTargets[6] * Mathf.Rad2Deg + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        _driveTargets[8] += (180.0f - ((360.0f - ((newShoulderAngles.y - _driveTargets[8] * Mathf.Rad2Deg + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        _driveTargets[10] = (180.0f - ((360.0f - ((newElbowAngle + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        _driveTargets[11] = (180.0f - ((360.0f - ((newWristAngle + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        
        _wristArticulationBody.SetDriveTargets(_driveTargets);
        
        float grab = _cartController.GetGrabVelocity() * Mathf.Deg2Rad;

        // _driveTargetVelocities[6] = (180.0f - ((360.0f - ((upperArmAngles.x - _shoulderSwivelTransform.localEulerAngles.y + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad / Time.fixedDeltaTime;

        _driveTargetVelocities[7] = _look.x;
        _driveTargetVelocities[9] = -_look.y;

        _look = Vector2.zero;

        _driveTargetVelocities[12] = grab;
        _driveTargetVelocities[13] = grab;

        _wristArticulationBody.SetDriveTargetVelocities(_driveTargetVelocities);
    }

    private void Update() {
        _look += _cartController.GetLook() * Time.deltaTime;
    }
}
