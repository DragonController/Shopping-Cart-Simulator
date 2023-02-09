using UnityEngine;
using System.Collections.Generic;

public class GripperController : MonoBehaviour {
    private CartController _cartController;

    private ArticulationBody _gripperArticulationBody;

    private List<GameObject> _triggeringItemGameObjects = new List<GameObject>();

    private void Start() {
        _cartController = transform.parent.parent.parent.parent.parent.gameObject.GetComponent<CartController>();

        _gripperArticulationBody = GetComponent<ArticulationBody>();
    }

    private void FixedUpdate() {
        ConfigurableJoint _itemJoint = _cartController.GetItemJoint();

        if (_cartController.IsGrabbing()) {
            if (_itemJoint == null && _triggeringItemGameObjects.Count > 0) {
                _itemJoint = _triggeringItemGameObjects[0].AddComponent<ConfigurableJoint>() as ConfigurableJoint;

                _itemJoint.connectedArticulationBody = _gripperArticulationBody;
                _itemJoint.xMotion = ConfigurableJointMotion.Locked;
                _itemJoint.yMotion = ConfigurableJointMotion.Locked;
                _itemJoint.zMotion = ConfigurableJointMotion.Locked;
                _itemJoint.enableCollision = true;

                _cartController.SetItemJoint(_itemJoint);
            }

            return;
        }

        if (_itemJoint != null) {
            Destroy(_itemJoint);
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.tag == _cartController.GetItemTag()) {
            _triggeringItemGameObjects.Add(collider.gameObject);
        }
    }

    private void OnTriggerExit(Collider collider) {
        _triggeringItemGameObjects.Remove(collider.gameObject);
    }
}
