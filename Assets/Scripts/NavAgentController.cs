using UnityEngine;
using System.Collections;

/// <summary>
/// Move with NavMeshAgent but handles movement and rotation by ourself.
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(RigidbodyController))]
public class NavAgentController : MonoBehaviour
{
    /// <summary>
    /// Place where to navigate.
    /// </summary>
    public Vector3 Destination { get; set; }

    /// <summary>
    /// Agent speed.
    /// </summary>
    public float MovementSpeed = 1f;

    /// <summary>
    /// FIXME: less than 1.2f causes abnormal jerkness of the object.
    /// </summary>
    public float StopDistance = 1.2f;
    public float PathHeight = 0.05f;

    private RigidbodyController rigidbodyController = null;
    private NavMeshAgent navMeshAgent = null;
    private Animator animator = null;

    private bool hasValidPath = false;
    private bool hasArrived = false;

    private Vector3 wayPoint = Vector3.zero;
    private Vector3 agentDestination = Vector3.zero;

    private bool isPathSet = false;
    
    #region RootMotion
    private Vector3 rootMotionMovement = Vector3.zero;
    private Quaternion rootMotionRotation = Quaternion.identity;
    #endregion

    void Awake()
    {
        rigidbodyController = GetComponent<RigidbodyController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

	void Start ()
    {
        agentDestination = transform.position;
        Destination = transform.position;

        // Handle position and rotation by ourself not by NavMeshAgent.
        navMeshAgent.updatePosition = false;
        navMeshAgent.updateRotation = false;

        navMeshAgent.stoppingDistance = StopDistance;
        navMeshAgent.speed = 1f;
        navMeshAgent.angularSpeed = 120f;
	}

	private bool HasValidPath()
    {
        return (isPathSet && navMeshAgent.hasPath && !navMeshAgent.pathPending);
    }

    float TargetDistance() 
    {
        Vector3 targetVector = agentDestination - transform.position;
        return targetVector.magnitude;
    }

	void Update ()
    {
        if (rigidbodyController == null || navMeshAgent == null)
            return;

        // Set destination if it is updated.
        if (agentDestination != Destination)
        {
            hasArrived = false;
            SetDestination(Destination);
        }

        // Stop if the agent is within stop distance.
        if (TargetDistance() < StopDistance)
        {
            navMeshAgent.Stop();

            hasArrived = true;
            isPathSet = false;
        }

        // Steering to the target destination.
        if (!hasArrived && HasValidPath())
        {
            // 'hasPath' will be false when the agent has finished pathing.
            if (navMeshAgent.hasPath && !navMeshAgent.pathPending)
            {
                wayPoint = navMeshAgent.steeringTarget;
            }

            Vector3 movement = Vector3.zero;
            Quaternion rotation = Quaternion.identity;
            CalcMove(wayPoint, ref movement, ref rotation);

            // Move rigidbody. Actual update position and rotation is done at here.
            rigidbodyController.UpdateTransform(movement, rotation);

            // set the agent stay with the rigidbody gameobject.
            navMeshAgent.nextPosition = transform.position;
        }
	}

    void CalcMove(Vector3 waypoint, ref Vector3 refMovement, ref Quaternion refRotation)
    {
        Vector3 dir = waypoint - transform.position;
        dir.y = dir.y - PathHeight;
        dir.Normalize();

        Vector3 vDirection = Vector3.Project(dir, transform.up);
        Vector3 lDirection = dir - vDirection;

        // Get rotation based on yaw angle.
        float yaw = MathHelper.AngleAroundAxis(transform.forward, lDirection, transform.up);
        refRotation = Quaternion.AngleAxis(yaw, transform.up);

        float moveSpeed = rootMotionMovement.magnitude / Time.deltaTime;
        if (moveSpeed == 0f) 
            moveSpeed = MovementSpeed;

        // Set the final velocity based on the rotation we'll look at.
        Quaternion toRotation = transform.rotation * refRotation;
        refMovement = toRotation.Forward() * (moveSpeed * Time.deltaTime);
    }

    protected virtual void OnAnimatorMove()
    {
        if (animator != null)
        {
            // Convert the movement to relative the current rotation
            rootMotionMovement = Quaternion.Inverse(transform.rotation) * (animator.deltaPosition);

            // Store the rotation as a velocity per second.
            rootMotionRotation = animator.deltaRotation;
        }
    }

    void SetDestination(Vector3 dest)
    {
        agentDestination = dest;

        if (!navMeshAgent.pathPending)
        {
            navMeshAgent.ResetPath();

            // Setting the destination will immediately calculate a path on the same frame.
            navMeshAgent.SetDestination(agentDestination);

            isPathSet = true;
        }
    }

}
