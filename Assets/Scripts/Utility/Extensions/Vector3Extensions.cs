using UnityEngine;

public static class Vector3Extensions
{
    public static float[] ToArray(this Vector3 vector3) =>
        new float[] { vector3.x, vector3.y, vector3.z };
}
