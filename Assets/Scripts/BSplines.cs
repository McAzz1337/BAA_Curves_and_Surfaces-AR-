using System.Collections.Generic;
using UnityEngine;
public static class BSplines
{

    public static float[] generateKnots(int degree, ControlPoints controlPoints)
    {
        int count = controlPoints.getTransforms().Length;
        int n = count - 1;
        int p = degree;

        int m = n + p + 1;
        float[] knots = new float[m + 1];

        for (int i = 0; i <= p; i++)
            knots[i] = 0;

        for (int i = p + 1; i <= n; i++)
            knots[i] = i - p;

        for (int i = n + 1; i <= m; i++)
            knots[i] = n - p + 1;

        return knots;
    }


    public static float basis(int i, int p, float t, int degree, float[] knots)
    {
        if (p == 0)
        {
            if (knots[i] <= t && t < knots[i + 1])
                return 1f;

            if (t == knots[knots.Length - 1] && i == knots.Length - degree - 2)
                return 1f;

            return 0f;
        }
        float left = 0;
        if (knots[i + p] != knots[i])
            left = (t - knots[i]) / (knots[i + p] - knots[i]);
        float right = 0;
        if (knots[i + p + 1] != knots[i + 1])
            right = (knots[i + p + 1] - t) / (knots[i + p + 1] - knots[i + 1]);
        return left * basis(i, p - 1, t, degree, knots) + right * basis(i + 1, p - 1, t, degree, knots);
    }

    public static Vector3 evaluate(float t, List<Vector3> positions, int degree, float[] knots)
    {
        if (positions.Count < degree + 1 || knots == null)
        {
            return Vector3.zero;
        }

        Vector3 result = Vector3.zero;
        for (int i = 0; i < positions.Count; i++)
        {
            float b = basis(i, degree, t, degree, knots);
            result += b * positions[i];
        }
        return result;
    }

}