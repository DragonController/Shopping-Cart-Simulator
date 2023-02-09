using UnityEngine;

public class CameraController : MonoBehaviour {
    [SerializeField] private float lookSpeed;

    private CartController _cartController;
    private ArticulationBody _cameraArticulationBody;

    private void Start() {
        _cartController = transform.parent.gameObject.GetComponent<CartController>();

        _cameraArticulationBody = GetComponent<ArticulationBody>();
    }

    private void FixedUpdate() {
        Vector2 look = _cartController.GetLook() * lookSpeed * Time.fixedDeltaTime;
        
        Vector3 currentEulerAngles = _cameraArticulationBody.anchorRotation.eulerAngles;

        _cameraArticulationBody.anchorRotation = Quaternion.Euler(currentEulerAngles.x + look.y, 0.0f, currentEulerAngles.z + look.x);
    }
}
