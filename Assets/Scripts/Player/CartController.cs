using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class CartController : MonoBehaviour {
    [SerializeField] private ItemsManager _itemsManager;
    [SerializeField] private CartTriggerController _cartTriggerController;
    [SerializeField] private CoverTriggerController _coverTriggerController;

    [SerializeField] private float _halfMoveAcceleration, _lookSpeed, _grabSpeed, _retractSpeed;
    [SerializeField] private float _minHandY;

    [SerializeField] private Transform _backLeftWheelTransform, _backRightWheelTransform, _minTargetTransform, _targetTransform, _maxTargetTransform, _itemsParentTransform;
    
    [SerializeField] private string _itemTag, _collectedItemLayer;
    [SerializeField] private string _keyboardControlScheme, _gamepadControlScheme;

    [SerializeField] private float _rejectObjectsForce;

    private PlayerInput _playerInput;
    private InputAction _moveAction, _lookAction, _grabAction, _retractAction;

    private bool _grab = false;
    private bool _grabbingItem = false;
    private GameObject _grabbedItem;

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

        _articulationBody = GetComponent<ArticulationBody>();

        _retractSpeedMultiplier = 1.0f / Vector3.Distance(_minTargetTransform.localPosition, _maxTargetTransform.localPosition);
    }

    private void FixedUpdate() {
        Vector2 move = _moveAction.ReadValue<Vector2>() * Time.fixedDeltaTime;

        _articulationBody.AddForceAtPosition(transform.TransformVector(0.0f, 0.0f, (move.x + move.y) * _halfMoveAcceleration * _articulationBody.mass), _backLeftWheelTransform.position);
        _articulationBody.AddForceAtPosition(transform.TransformVector(0.0f, 0.0f, (-move.x + move.y) * _halfMoveAcceleration * _articulationBody.mass), _backRightWheelTransform.position);

        // print(move.x + move.y);
        // print(-move.x + move.y);
        
        // Vector2 look = _moveAction.ReadValue<Vector2>();

        // _articulationBody.AddRelativeForce(Vector3.forward * move.y * moveForce * Time.fixedDeltaTime);

        List<Collider> cartTriggerColliders = _cartTriggerController.GetCartTriggerColliders();

        for (int i = 0; i < cartTriggerColliders.Count; i++) {
            Collider collider = cartTriggerColliders[i];

            for (int j = 0; j < i; j++) {
                if (collider == cartTriggerColliders[j].gameObject) {
                    continue;
                }
            }

            if (collider.tag == "Player" || collider.gameObject == _grabbedItem || collider.gameObject.layer == LayerMask.NameToLayer(_collectedItemLayer)) {
                continue;
            }

            if (_itemsManager.GetItems().ContainsKey(collider.gameObject) && _itemsManager.GetRemainingItemTypeIndices().Contains(_itemsManager.GetItems()[collider.gameObject])) {
                if (!_coverTriggerController.GetCoverTriggers().Contains(collider.gameObject)) {
                    collider.gameObject.layer = LayerMask.NameToLayer(_collectedItemLayer);
                    collider.tag = "Untagged";

                    _itemsManager.RemoveItem(collider.gameObject);
                }

                continue;
            }
            
            if (collider.attachedArticulationBody != null) {
                collider.attachedArticulationBody.AddForce(new Vector3(0.0f, _rejectObjectsForce, 0.0f));
            }
        }
    }

    private void Update() {
        if (_playerInput.currentControlScheme == _keyboardControlScheme && !Mathf.Approximately(_retractAction.ReadValue<float>(), 0.0f)) {
            _retractDistance = Mathf.Clamp(_retractDistance - Mathf.Sign(_retractAction.ReadValue<float>()) * _retractSpeed * _retractSpeedMultiplier * Time.deltaTime, 0.0f, 1.0f);
        }

        if (_playerInput.currentControlScheme == _gamepadControlScheme) {
            _retractDistance = _retractAction.ReadValue<float>();
        }
    }

    public Transform GetMinTargetTransform() {
        return _minTargetTransform;
    }

    public Transform GetTargetTransform() {
        return _targetTransform;
    }

    public Transform GetMaxTargetTransform() {
        return _maxTargetTransform;
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
            
            _grabbedItem = null;
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

    public void SetGrabbedItem(GameObject grabbedItem) {
        _grabbedItem = grabbedItem;
        
        _grabbingItem = true;
    }

    public Transform GetItemsParentTransform() {
        return _itemsParentTransform;
    }

    public string GetItemTag() {
        return _itemTag;
    }

    public float GetMinHandY() {
        return _minHandY;
    }
}
