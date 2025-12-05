using UnityEngine;

public static class Ballistics
{
    public static bool SolveBallisticVelocity(out Vector3 velocity, Vector3 origin, Vector3 target, float speed, float gravity = -9.81f, bool useHighArc = true)
    {
        velocity = Vector3.zero;
        Vector3 displacement = target - origin;
        float dxz = new Vector3(displacement.x, 0f, displacement.z).magnitude;
        float dy = displacement.y;

        if (speed <= 0f || (Mathf.Approximately(dxz, 0f) && Mathf.Approximately(dy, 0f)))
            return false;

        float speed2 = speed * speed;
        float g = Mathf.Abs(gravity);

        float underSqrt = speed2 * speed2 - g * (g * dxz * dxz + 2f * dy * speed2);
        if (underSqrt < 0f)
            return false;

        float sqrt = Mathf.Sqrt(underSqrt);
        float angle = Mathf.Atan2(speed2 + (useHighArc ? sqrt : -sqrt), g * dxz);
        Vector3 dirXZ = new Vector3(displacement.x, 0f, displacement.z).normalized;
        Vector3 vel = dirXZ * (Mathf.Cos(angle) * speed) + Vector3.up * (Mathf.Sin(angle) * speed);

        velocity = vel;
        return true;
    }
}
