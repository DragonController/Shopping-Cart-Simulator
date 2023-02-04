using UnityEngine;
using UnityEngine.InputSystem;

public class CartController : MonoBehaviour {
    [SerializeField] private float moveForce;

    private PlayerInput _playerInput;
    private InputAction _moveAction;

    private ArticulationBody _articulationBody;
    
    private void Start() {
        _playerInput = GetComponent<PlayerInput>();
        _moveAction = _playerInput.actions["Move"];

        _articulationBody = GetComponent<ArticulationBody>();
    }

    private void Update() {
        Vector2 move = _moveAction.ReadValue<Vector2>();

        print(move);

        _articulationBody.AddRelativeForce(Vector3.forward * move.y * moveForce);
    }
}
