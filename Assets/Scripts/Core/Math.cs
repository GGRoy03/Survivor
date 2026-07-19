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

    /// <summary>
    /// Computes the interpolated rotation between a source and a target vector.
    /// </summary>
    /// <param name="current">The current direction we are looking at.</param>
    /// <param name="direction">The target direction we want to look at.</param>
    /// <param name="radiansPerStep">How many radians per seconds can we rotate.</param>
    /// <returns>A quaternion containing the rotation between the two vectors.</returns>

    public static Quaternion LookTowards(Vector3 current, Vector3 direction, float radiansPerStep)
    {
        // float      step    = radiansPerStep * Time.deltaTime;
        Vector3    looking = Vector3.RotateTowards(current, direction, radiansPerStep, 0.0f);
        Quaternion result  = Quaternion.LookRotation(looking);

        return result;
    }
}
