using UnityEngine;
using System.Collections.Generic;

public class GripperController : MonoBehaviour {
    private CartController _cartController;
    private Transform _itemsParentTransform;

    private List<Transform> _collidingItemTransforms = new List<Transform>();
    private List<Transform> _triggeringItemTransforms = new List<Transform>();

    private void Start() {
        _cartController = transform.parent.parent.parent.parent.parent.gameObject.GetComponent<CartController>();
        _itemsParentTransform = _cartController.GetItemsParentTransform();
    }

    private void FixedUpdate() {
        if (_cartController.IsGrab()) {
            if (!_cartController.IsGrabbingItem() && transform.childCount == 0) {
                foreach (Transform itemTransform in _collidingItemTransforms) {
                    if (_triggeringItemTransforms.Contains(itemTransform)) {
                        itemTransform.gameObject.GetComponent<ArticulationBody>().enabled = false;

                        itemTransform.parent = transform;

                        _cartController.SetGrabbingItem(true);
                    }
                }
            }

            return;
        }

        if (transform.childCount > 0) {
            Transform itemTransform = transform.GetChild(0);

            itemTransform.parent = _itemsParentTransform;

            itemTransform.gameObject.GetComponent<ArticulationBody>().enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == _cartController.GetItemTag()) {
            _collidingItemTransforms.Add(collision.transform);
        }
    }

    private void OnCollisionExit(Collision collision) {
        _collidingItemTransforms.RemoveAll(c => c == collision.transform);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.tag == _cartController.GetItemTag()) {
            _triggeringItemTransforms.Add(collider.transform);
        }
    }

    private void OnTriggerExit(Collider collider) {
        _triggeringItemTransforms.RemoveAll(c => c == collider.transform);
    }
}
