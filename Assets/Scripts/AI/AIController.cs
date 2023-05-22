using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {
    [SerializeField] private Rect _floorRect;
    
    private NavMeshAgent _navMeshAgent;
    private Rigidbody _aiRigidbody;

    private void Start() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _aiRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        NavMeshHit navMeshHit;

        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance) {
            Vector3 newDestination = new Vector3(Random.Range(_floorRect.xMin, _floorRect.xMax), 0.0f, Random.Range(_floorRect.yMin, _floorRect.yMax));

            NavMesh.SamplePosition(newDestination, out navMeshHit, Vector2.Distance(_floorRect.min, _floorRect.max) * 2.0f, NavMesh.AllAreas);

            newDestination = navMeshHit.position;

            _navMeshAgent.SetDestination(newDestination);
        }
    }
}
