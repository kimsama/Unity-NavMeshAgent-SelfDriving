using UnityEngine;
using System.Collections;

public class NavAgentController : MonoBehaviour
{

    public Vector3 Destination { get; set; }

    private RigidbodyController rigidbodyController = null;
    private NavMeshAgent navMeshAgent = null;

    void Awake()
    {
        rigidbodyController = GetComponent<RigidbodyController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
        {
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
        }
    }

	void Start ()
    {
	
	}
	
	void Update ()
    {
        if (rigidbodyController == null || navMeshAgent == null)
            return;




        // move rigidbody

        navMeshAgent.nextPosition = transform.position;

	}

    bool firstPathSet = false;
    bool hasArrived = false;
    Vector3 agentDestination = Vector3.zero;

    void SetDestination(Vector3 dest)
    {
        if (hasArrived && agentDestination == dest)
            return;

        hasArrived = false;

        agentDestination = dest;

        if (!navMeshAgent.pathPending)
        {
            navMeshAgent.updatePosition = false;
            navMeshAgent.updateRotation = false;
            navMeshAgent.ResetPath();
            navMeshAgent.SetDestination(agentDestination);

            firstPathSet = true;
        }
    }
}
