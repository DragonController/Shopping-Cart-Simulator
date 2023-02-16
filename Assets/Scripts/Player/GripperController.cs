using UnityEngine;
using System.Collections.Generic;

public class GripperController : MonoBehaviour {
    private CartController _cartController;
    private Transform _itemsParentTransform;

    private List<ArticulationBody> _collidingItemArticulationBodies = new List<ArticulationBody>();
    private List<ArticulationBody> _triggeringItemArticulationBodies = new List<ArticulationBody>();
    private ArticulationBody _articulationBody, _childArticulationBody;

    private Vector3 _defaultCenterOfMass;
    private float _defaultMass;

    private void Start() {
        _articulationBody = GetComponent<ArticulationBody>();

        _defaultCenterOfMass = _articulationBody.centerOfMass;
        _defaultMass = _articulationBody.mass;

        _cartController = transform.parent.parent.parent.parent.parent.gameObject.GetComponent<CartController>();
        _itemsParentTransform = _cartController.GetItemsParentTransform();
    }

    private void FixedUpdate() {
        if (_cartController.IsGrab()) {
            if (!_cartController.IsGrabbingItem() && transform.childCount == 0) {
                foreach (ArticulationBody itemArticulationBody in _collidingItemArticulationBodies) {
                    if (_triggeringItemArticulationBodies.Contains(itemArticulationBody)) {
                        _articulationBody.centerOfMass = Vector3.Lerp(_articulationBody.centerOfMass, transform.InverseTransformPoint(itemArticulationBody.worldCenterOfMass), itemArticulationBody.mass / (_articulationBody.mass + itemArticulationBody.mass));
                        _articulationBody.mass += itemArticulationBody.mass;

                        itemArticulationBody.enabled = false;

                        itemArticulationBody.transform.parent = transform;

                        _childArticulationBody = itemArticulationBody;

                        _cartController.SetGrabbedItem(itemArticulationBody.gameObject);
                    }
                }
            }

            return;
        }

        if (transform.childCount > 0) {
            _childArticulationBody.transform.parent = _itemsParentTransform;

            _childArticulationBody.enabled = true;

            _childArticulationBody = null;

            _articulationBody.centerOfMass = _defaultCenterOfMass;
            _articulationBody.mass = _defaultMass;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == _cartController.GetItemTag() && !_collidingItemArticulationBodies.Contains(collision.articulationBody)) {
            _collidingItemArticulationBodies.Add(collision.articulationBody);
        }
    }

    private void OnCollisionExit(Collision collision) {
        _collidingItemArticulationBodies.RemoveAll(c => c == collision.articulationBody);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.tag == _cartController.GetItemTag() && !_triggeringItemArticulationBodies.Contains(collider.attachedArticulationBody)) {
            _triggeringItemArticulationBodies.Add(collider.attachedArticulationBody);
        }
    }

    private void OnTriggerExit(Collider collider) {
        _triggeringItemArticulationBodies.RemoveAll(c => c == collider.attachedArticulationBody);
    }
}
