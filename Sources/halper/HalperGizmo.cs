using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HalperGizmo
{
  // DEBUG DRAW

  static public void gizmoDrawCircle(Vector3 position, float radius, Color color, float alpha = 1f, string label = "")
  {
#if UNITY_EDITOR
    color.a = alpha;

    Gizmos.DrawLine(position, position + Vector3.up * (radius + 0.05f));

    Gizmos.color = color;
    Gizmos.DrawSphere(position, radius);

    if (label.Length > 0) Handles.Label(position + (Vector3.right * 0.01f) + Vector3.up * (radius + 0.05f), label);
#endif
  }

  static public void gizmoDrawCross(Transform pivot, float size, Color color, string label = "")
  {
#if UNITY_EDITOR
    Gizmos.color = color;
    Gizmos.DrawLine(pivot.position + Vector3.up * size, pivot.position + Vector3.down * size);
    Gizmos.DrawLine(pivot.position + Vector3.left * size, pivot.position + Vector3.right * size);

    if (label.Length > 0) Handles.Label(pivot.position, pivot.name);
#endif
  }
}
