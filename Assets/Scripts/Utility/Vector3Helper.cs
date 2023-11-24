using System.Globalization;
using System.Linq;
using UnityEngine;

public static class Vector3Helper
{
    public static Vector3 FromString(string s)
    {
        float[] components = s.Trim('(', ')').Split(',')
            .Select(x => float.Parse(x, CultureInfo.InvariantCulture)).ToArray();

        return new Vector3(components[0], components[1], components[2]);
    }
}
