using UnityEngine;
using System.Collections.Generic;

public class CoverTriggerController : MonoBehaviour {
    [SerializeField] private CartController _cartController;

    private List<GameObject> _coverTriggers = new List<GameObject>();

    private void OnTriggerEnter(Collider collider) {
        _coverTriggers.Add(collider.gameObject);
    }

    private void OnTriggerExit(Collider collider) {
        _coverTriggers.RemoveAll(c => c == collider.gameObject);

        _cartController.CheckGameObject(collider.gameObject);
    }

    public List<GameObject> GetCoverTriggers() {
        return _coverTriggers;
    }
}
