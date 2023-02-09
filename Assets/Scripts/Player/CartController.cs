using UnityEngine;
using UnityEngine.InputSystem;

public class CartController : MonoBehaviour {
    [SerializeField] private float _halfMoveAcceleration, _lookSpeed, _grabSpeed;

    [SerializeField] private Transform _targetTransform;
    
    [SerializeField] private string _itemTag;

    private PlayerInput _playerInput;
    private InputAction _moveAction, _lookAction, _grabAction, _retractAction, _pauseAction;

    private bool _grab = false;

    private ArticulationBody _articulationBody;

    private FixedJoint _itemJoint;
    
    private void Start() {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        _lookAction = _playerInput.actions["Look"];
        _grabAction = _playerInput.actions["Grab"];
        _grabAction.performed += _ => _grab = true;
        _grabAction.canceled += _ => _grab = false;
        _retractAction = _playerInput.actions["Retract"];
        _pauseAction = _playerInput.actions["Pause"];

        _articulationBody = GetComponent<ArticulationBody>();
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

    public Transform GetTargetTransform() {
        return _targetTransform;
    }

    public Vector2 GetLook() {
        return _lookAction.ReadValue<Vector2>() * _lookSpeed;
    }

    public bool IsGrabbing() {
        return _grab;
    }

    public float GetGrab() {
        if (_grab) {
            return _grabSpeed;
        }

        return -_grabSpeed;
    }

    public FixedJoint GetItemJoint() {
        return _itemJoint;
    }

    public void SetItemJoint(FixedJoint itemJoint) {
        _itemJoint = itemJoint;
    }

    public string GetItemTag() {
        return _itemTag;
    }
}
