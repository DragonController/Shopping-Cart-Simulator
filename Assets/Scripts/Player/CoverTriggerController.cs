using UnityEngine;
using System.Collections.Generic;

public class CoverTriggerController : MonoBehaviour {
    private List<GameObject> _coverTriggers = new List<GameObject>();

    private void OnTriggerEnter(Collider collider) {
        if (!_coverTriggers.Contains(collider.gameObject)) {
            _coverTriggers.Add(collider.gameObject);
        }
    }

    private void OnTriggerExit(Collider collider) {
        _coverTriggers.RemoveAll(c => c == collider.gameObject);
    }

    public List<GameObject> GetCoverTriggers() {
        return _coverTriggers;
    }
}
