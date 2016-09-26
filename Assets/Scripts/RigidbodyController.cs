using UnityEngine;
using System.Collections;

/// <summary>
/// 
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class RigidbodyController : MonoBehaviour 
{
    public float RotationSpeed = 15f;
    private Transform tm;

    void Awake()
    {
        tm = GetComponent<Transform>();
    }

    public void UpdateTransform(Vector3 movement, Quaternion rotation)
    {
        tm.position = tm.position + movement;
        tm.rotation = Quaternion.Slerp(tm.rotation, tm.rotation * rotation, Time.deltaTime * RotationSpeed);
    }
}
