using UnityEngine;

public static class Vector3Helper
{
    public static Vector3 FromArray(float[] array) =>
        new Vector3(array[0], array[1], array[2]);
}
