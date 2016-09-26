using UnityEngine;
using System.Collections;

public static class MathHelper 
{
    public static Vector3 Forward(this Quaternion rh)
    {
        return new Vector3(2 * (rh.x * rh.z + rh.w * rh.y),
                           2 * (rh.y * rh.z - rh.w * rh.x),
                           1 - 2 * (rh.x * rh.x + rh.y * rh.y));
    }

    public static float AngleAroundAxis(Vector3 dirA, Vector3 dirB, Vector3 axis)
    {
        // Project A and B onto the plane orthogonal target axis
        dirA = dirA - Vector3.Project(dirA, axis);
        dirB = dirB - Vector3.Project(dirB, axis);

        // Find (positive) angle between A and B
        float angle = Vector3.Angle(dirA, dirB);

        // Return angle multiplied with 1 or -1
        return angle * (Vector3.Dot(axis, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1);
    }

}
