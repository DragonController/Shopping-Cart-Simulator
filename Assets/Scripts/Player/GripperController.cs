using UnityEngine;
using System.Collections.Generic;

public class GripperController : MonoBehaviour {
    private CartController _cartController;
    private Transform _itemsParentTransform;

    private List<ArticulationBody> _collidingItemArticulationBodies = new List<ArticulationBody>();
    private List<ArticulationBody> _triggeringItemArticulationBodies = new List<ArticulationBody>();
    private ArticulationBody childArticulationBody;

    private void Start() {
        _cartController = transform.parent.parent.parent.parent.parent.gameObject.GetComponent<CartController>();
        _itemsParentTransform = _cartController.GetItemsParentTransform();
    }

    private void FixedUpdate() {
        if (_cartController.IsGrab()) {
            if (!_cartController.IsGrabbingItem() && transform.childCount == 0) {
                foreach (ArticulationBody itemArticulationBody in _collidingItemArticulationBodies) {
                    if (_triggeringItemArticulationBodies.Contains(itemArticulationBody)) {
                        itemArticulationBody.enabled = false;

                        itemArticulationBody.transform.parent = transform;

                        childArticulationBody = itemArticulationBody;

                        _cartController.SetGrabbingItem(true);
                    }
                }
            }

            return;
        }

        if (transform.childCount > 0) {
            childArticulationBody.transform.parent = _itemsParentTransform;

            childArticulationBody.enabled = true;

            childArticulationBody = null;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == _cartController.GetItemTag()) {
            _collidingItemArticulationBodies.Add(collision.articulationBody);
        }
    }

    private void OnCollisionExit(Collision collision) {
        _collidingItemArticulationBodies.RemoveAll(c => c == collision.articulationBody);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.tag == _cartController.GetItemTag()) {
            _triggeringItemArticulationBodies.Add(collider.attachedArticulationBody);
        }
    }

    private void OnTriggerExit(Collider collider) {
        _triggeringItemArticulationBodies.RemoveAll(c => c == collider.attachedArticulationBody);
    }
}
