using UnityEngine;
using UnityEngine.InputSystem;

public class CartController : MonoBehaviour {
    [SerializeField] private float _halfMoveAcceleration, _lookSpeed, _grabSpeed, _retractSpeed;

    [SerializeField] private Transform _targetTransform, _itemsParentTransform;
    
    [SerializeField] private string _itemTag;
    [SerializeField] private string _keyboardControlScheme, _gamepadControlScheme;

    private PlayerInput _playerInput;
    private InputAction _moveAction, _lookAction, _grabAction, _retractAction, _pauseAction;

    private bool _grab = false;
    private bool _grabbingItem = false;

    private float _retractDistance = 0.0f;
    private float _retractSpeedMultiplier;

    private ArticulationBody _articulationBody;
    
    private void Start() {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        _lookAction = _playerInput.actions["Look"];
        _grabAction = _playerInput.actions["Grab"];
        _grabAction.performed += _ => SetGrab(true);
        _grabAction.canceled += _ => SetGrab(false);
        _retractAction = _playerInput.actions["Retract"];
        _pauseAction = _playerInput.actions["Pause"];

        _articulationBody = GetComponent<ArticulationBody>();

        _retractSpeedMultiplier = 2.0f / _targetTransform.localPosition.magnitude;
    }

    private void FixedUpdate() {
        Vector2 move = _moveAction.ReadValue<Vector2>() * Time.fixedDeltaTime;

        _articulationBody.AddForceAtPosition(transform.TransformVector(0.0f, 0.0f, (move.x + move.y) * _halfMoveAcceleration * _articulationBody.mass), transform.TransformPoint(-0.2794f, 0.0f, -0.5207f));
        _articulationBody.AddForceAtPosition(transform.TransformVector(0.0f, 0.0f, (-move.x + move.y) * _halfMoveAcceleration * _articulationBody.mass), transform.TransformPoint(0.2794f, 0.0f, -0.5207f));

        // print(move.x + move.y);
        // print(-move.x + move.y);
        
        // Vector2 look = _moveAction.ReadValue<Vector2>();

        // _articulationBody.AddRelativeForce(Vector3.forward * move.y * moveForce * Time.fixedDeltaTime);
    }

    private void Update() {
        if (_playerInput.currentControlScheme == _keyboardControlScheme && !Mathf.Approximately(_retractAction.ReadValue<float>(), 0.0f)) {
            _retractDistance = Mathf.Clamp(_retractDistance - Mathf.Sign(_retractAction.ReadValue<float>()) * _retractSpeed * _retractSpeedMultiplier, 0.0f, 1.0f);
        }

        if (_playerInput.currentControlScheme == _gamepadControlScheme) {
            _retractDistance = _retractAction.ReadValue<float>();
        }

        print(_retractDistance);
    }

    public Transform GetTargetTransform() {
        return _targetTransform;
    }

    public Vector2 GetLook() {
        return _lookAction.ReadValue<Vector2>() * _lookSpeed;
    }

    public bool IsGrab() {
        return _grab;
    }

    private void SetGrab(bool grab) {
        _grab = grab;

        if (!grab) {
            _grabbingItem = false;
        }
    }

    public float GetGrabVelocity() {
        if (_grab) {
            return _grabSpeed;
        }

        return -_grabSpeed;
    }

    public bool IsGrabbingItem() {
        return _grabbingItem;
    }

    public float GetRetractDistance() {
        return _retractDistance;
    }

    public void SetGrabbingItem(bool grabbingItem) {
        _grabbingItem = grabbingItem;
    }

    public Transform GetItemsParentTransform() {
        return _itemsParentTransform;
    }

    public string GetItemTag() {
        return _itemTag;
    }
}
