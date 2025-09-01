using UnityEngine;

public class FacingDirectionGizmo : MonoBehaviour
{
    public float lineLength = 1f;
    public Color gizmoColor = Color.red;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Vector3 startPosition = transform.position;
        Vector3 direction = transform.forward * lineLength;
        Gizmos.DrawLine(startPosition, startPosition + direction);
        Gizmos.DrawSphere(startPosition + direction, 0.1f); // Optional: adds a small sphere at the tip
    }
}
