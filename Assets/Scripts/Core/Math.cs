using UnityEngine;

static class Math
{
    public static float SqrDistanceBetweenTransform(Transform A, Transform B)
    {
        float result = Vector3.Magnitude(A.position - B.position);
        return result;
    }

    /// <summary>
    /// Gets the direction from A to B
    /// </summary>
    /// <param name="A">The source point.</param>
    /// <param name="B">The destination point.</param>
    /// <returns>A normalized vector or a zero vector if the points are too close.</returns>

    public static Vector3 DirectionTowards(Vector3 A, Vector3 B)
    {
        Vector3 result = Vector3.zero;

        Vector3 AToB = B - A;
        if(AToB.sqrMagnitude > Mathf.Epsilon)
        {
            result = Vector3.Normalize(B - A);
        }

        return result;
    }
}
