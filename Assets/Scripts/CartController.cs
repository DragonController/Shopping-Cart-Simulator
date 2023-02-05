using UnityEngine;
using UnityEngine.InputSystem;

public class CartController : MonoBehaviour {
    [SerializeField] private float _halfMoveForce;

    private PlayerInput _playerInput;
    private InputAction _moveAction, _lookAction, _grabAction, _retractAction, _pauseAction;

    private ArticulationBody _articulationBody;
    
    private void Start() {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];
        _lookAction = _playerInput.actions["Look"];
        _grabAction = _playerInput.actions["Grab"];
        _retractAction = _playerInput.actions["Retract"];
        _pauseAction = _playerInput.actions["Pause"];

        _articulationBody = GetComponent<ArticulationBody>();
    }

    private void FixedUpdate() {
        Vector2 move = _moveAction.ReadValue<Vector2>();

        _articulationBody.AddForceAtPosition(transform.TransformVector(0.0f, 0.0f, (move.x + move.y) * _halfMoveForce * Time.fixedDeltaTime), transform.TransformPoint(-0.5f, 0.0f, -0.5f));
        _articulationBody.AddForceAtPosition(transform.TransformVector(0.0f, 0.0f, (-move.x + move.y) * _halfMoveForce * Time.fixedDeltaTime), transform.TransformPoint(0.5f, 0.0f, -0.5f));
        
        // Vector2 look = _moveAction.ReadValue<Vector2>();

        // _articulationBody.AddRelativeForce(Vector3.forward * move.y * moveForce * Time.fixedDeltaTime);
    }
}
