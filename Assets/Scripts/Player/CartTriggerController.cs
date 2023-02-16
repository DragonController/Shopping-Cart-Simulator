using UnityEngine;
using System.Collections.Generic;

public class CartTriggerController : MonoBehaviour {
    private List<Collider> _cartTriggerColliders = new List<Collider>();

    private void OnTriggerEnter(Collider collider) {
        if (!_cartTriggerColliders.Contains(collider)) {
            _cartTriggerColliders.Add(collider);
        }
    }

    private void OnTriggerExit(Collider collider) {
        print(collider);
        _cartTriggerColliders.RemoveAll(c => c == collider);
    }

    public List<Collider> GetCartTriggerColliders() {
        return _cartTriggerColliders;
    }
}
