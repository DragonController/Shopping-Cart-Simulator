using UnityEngine;
using UnityEngine.AI;

public class AIController : MonoBehaviour {
    [SerializeField] private Rect floorRect;
    
    private NavMeshAgent navMeshAgent;
    private Rigidbody aiRigidbody;

    private Vector3 prevPosition;

    private void Start() {
        navMeshAgent = GetComponent<NavMeshAgent>();
        aiRigidbody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {
        NavMeshHit navMeshHit;

        if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) {
            Vector3 newDestination = new Vector3(Random.Range(floorRect.xMin, floorRect.xMax), 0.0f, Random.Range(floorRect.yMin, floorRect.yMax));

            NavMesh.SamplePosition(newDestination, out navMeshHit, navMeshAgent.height * 2.0f, NavMesh.AllAreas);

            // print(navMeshHit.position);
            newDestination = navMeshHit.position;

            navMeshAgent.SetDestination(navMeshHit.position);

            prevPosition = newDestination;
        }

        NavMesh.FindClosestEdge(navMeshAgent.nextPosition, out navMeshHit, NavMesh.AllAreas);
        print(Vector3.Distance(navMeshAgent.nextPosition, navMeshHit.position));
        print(Vector3.Distance(navMeshAgent.nextPosition, navMeshHit.position) < navMeshAgent.radius);

        // if () {
        //     aiRigidbody.AddRelativeForce(new Vector3(-navMeshAgent.acceleration, 0.0f, 0.0f), ForceMode.Acceleration);
        // }
    }
}
