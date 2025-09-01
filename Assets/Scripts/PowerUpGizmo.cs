using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PowerUpGizmo : MonoBehaviour
{
    public Color gizmoColor = Color.yellow;
    public float gizmoSize = 0.3f;

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, gizmoSize);
    }
}
