using UnityEngine;
using System.Collections.Generic;

public class GripperController : MonoBehaviour {
    private CartController _cartController;

    private ArticulationBody _gripperArticulationBody;

    private List<GameObject> _collidingItems = new List<GameObject>();
    private List<GameObject> _triggeringItems = new List<GameObject>();

    private void Start() {
        _cartController = transform.parent.parent.parent.parent.parent.gameObject.GetComponent<CartController>();

        _gripperArticulationBody = GetComponent<ArticulationBody>();
    }

    private void FixedUpdate() {
        FixedJoint _itemJoint = _cartController.GetItemJoint();

        if (_cartController.IsGrabbing()) {
            if (_itemJoint == null) {
                foreach (GameObject triggeringItem in  _triggeringItems) {
                    if (_collidingItems.Contains(triggeringItem)) {
                        _itemJoint = triggeringItem.AddComponent<FixedJoint>() as FixedJoint;

                        _itemJoint.connectedArticulationBody = _gripperArticulationBody;

                        _cartController.SetItemJoint(_itemJoint);
                    }
                }
            }

            return;
        }

        if (_itemJoint != null) {
            Destroy(_itemJoint);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == _cartController.GetItemTag()) {
            _collidingItems.Add(collision.gameObject);
        }
    }

    private void OnCollisionExit(Collision collision) {
        _collidingItems.Remove(collision.gameObject);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.tag == _cartController.GetItemTag()) {
            _triggeringItems.Add(collider.gameObject);
        }
    }

    private void OnTriggerExit(Collider collider) {
        _triggeringItems.Remove(collider.gameObject);
    }
}
