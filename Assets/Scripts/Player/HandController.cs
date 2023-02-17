using UnityEngine;
using System.Collections.Generic;

public class HandController : MonoBehaviour {
    private CartController _cartController;
    private Transform _cartTransform, _shoulderSwivelTransform, _shoulderTransform, _elbowTransform, _targetTransform, _maxTargetTransform;
    private Vector3 _shoulderRelativePosition, _elbowRelativePosition;

    private ArticulationBody _wristArticulationBody;
    private int _shoulderSwivelIndex, _shoulderIndex, _elbowIndex, _wristIndex, _lookSwivelIndex, _lookIndex, _firstGrabberIndex, _secondGrabberIndex;
    private List<float> _driveTargets = new List<float>();
    private List<float> _driveTargetVelocities = new List<float>();

    private float _upperArmLength, _lowerArmLength;

    private Vector2 _look = Vector2.zero;

    private Vector3 _maxTargetLocalPos, _minTargetLocalPos;

    private void Start() {
        Cursor.lockState = CursorLockMode.Locked;

        _elbowTransform = transform.parent;
        _shoulderTransform = _elbowTransform.parent;
        _shoulderSwivelTransform = _shoulderTransform.parent;

        _cartTransform = _shoulderSwivelTransform.parent;

        _cartController = _cartTransform.gameObject.GetComponent<CartController>();

        _wristArticulationBody = GetComponent<ArticulationBody>();
        ArticulationBody shoulderArticulationBody = _shoulderTransform.gameObject.GetComponent<ArticulationBody>();
        ArticulationBody elbowArticulationBody = _elbowTransform.gameObject.GetComponent<ArticulationBody>();
        
        _shoulderRelativePosition = shoulderArticulationBody.anchorPosition;
        _elbowRelativePosition = elbowArticulationBody.anchorPosition;

        Vector3 shoulderPosition = _shoulderTransform.TransformPoint(_shoulderRelativePosition);
        Vector3 elbowPosition = _elbowTransform.TransformPoint(_elbowRelativePosition);
        Vector3 wristPosition = transform.TransformPoint(_wristArticulationBody.anchorPosition);

        _upperArmLength = Vector3.Distance(shoulderPosition, elbowPosition);
        _lowerArmLength = Vector3.Distance(elbowPosition, wristPosition);

        _targetTransform = _cartController.GetTargetTransform();
        _maxTargetTransform = _cartController.GetMaxTargetTransform();

        _wristArticulationBody.GetDriveTargets(_driveTargets);
        _wristArticulationBody.GetDriveTargetVelocities(_driveTargetVelocities);

        _minTargetLocalPos = _cartController.GetMinTargetTransform().localPosition;
        _maxTargetLocalPos = _maxTargetTransform.localPosition;
        
        _shoulderSwivelIndex = _shoulderSwivelTransform.gameObject.GetComponent<ArticulationBody>().index - 1;
        _shoulderIndex = shoulderArticulationBody.index - 1;
        _elbowIndex = elbowArticulationBody.index - 1;
        _wristIndex = _wristArticulationBody.index - 1;
        _lookSwivelIndex = _cartController.GetCameraBaseSwivelArticulationBody().index - 1;
        _lookIndex = _cartController.GetCameraBaseArticulationBody().index - 1;
        _firstGrabberIndex = transform.GetChild(0).gameObject.GetComponent<ArticulationBody>().index - 1;
        _secondGrabberIndex = transform.GetChild(1).gameObject.GetComponent<ArticulationBody>().index - 1;
    }

    private void FixedUpdate() {
        _targetTransform.localPosition = Vector3.Lerp(_maxTargetLocalPos, _minTargetLocalPos, _cartController.GetRetractDistance());

        Vector3 targetPosition = _targetTransform.position;
        targetPosition.y = Mathf.Max(targetPosition.y, _cartController.GetMinHandY());
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
            if ((360.0f - ((oldShoulderAngle + 90.0f) % 360.0f)) % 360.0f > 180.0f) {
                newElbowAngle = oldShoulderAngle + Mathf.Asin(elbowDifference.y / elbowDistance) * Mathf.Rad2Deg + 180.0f;
            }
        }

        _driveTargets[_shoulderSwivelIndex] += (180.0f - ((360.0f - ((newShoulderAngles.x - _driveTargets[_shoulderSwivelIndex] * Mathf.Rad2Deg + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        _driveTargets[_shoulderIndex] += (180.0f - ((360.0f - ((newShoulderAngles.y - _driveTargets[_shoulderIndex] * Mathf.Rad2Deg + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        _driveTargets[_elbowIndex] = (180.0f - ((360.0f - ((newElbowAngle + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        _driveTargets[_wristIndex] = (180.0f - ((360.0f - ((newWristAngle + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad;
        
        _wristArticulationBody.SetDriveTargets(_driveTargets);
        
        float grab = _cartController.GetGrabVelocity() * Mathf.Deg2Rad;

        // _driveTargetVelocities[_shoulderSwivelIndex] = (180.0f - ((360.0f - ((upperArmAngles.x - _shoulderSwivelTransform.localEulerAngles.y + 180.0f) % 360.0f)) % 360.0f)) * Mathf.Deg2Rad / Time.fixedDeltaTime;

        _driveTargetVelocities[_lookSwivelIndex] = _look.x;
        _driveTargetVelocities[_lookIndex] = -_look.y;

        _look = Vector2.zero;

        _driveTargetVelocities[_firstGrabberIndex] = grab;
        _driveTargetVelocities[_secondGrabberIndex] = grab;

        _wristArticulationBody.SetDriveTargetVelocities(_driveTargetVelocities);
    }

    private void Update() {
        _look += _cartController.GetLook() * Time.deltaTime;
    }
}
