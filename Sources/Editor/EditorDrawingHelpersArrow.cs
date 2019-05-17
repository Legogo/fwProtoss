using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class EditorDrawingHelpersArrow
{
  public static void handleArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
  {
    Handles.DrawLine(pos, direction);

    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Handles.DrawLine(pos + direction, right * arrowHeadLength);
    Handles.DrawLine(pos + direction, left * arrowHeadLength);
  }

  public static void handleArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
  {
    Handles.color = color;
    Handles.DrawLine(pos, direction);

    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Handles.DrawLine(pos + direction, right * arrowHeadLength);
    Handles.DrawLine(pos + direction, left * arrowHeadLength);
  }


  public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
  {
    Gizmos.DrawRay(pos, direction);

    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
    Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
  }

  public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
  {
    Gizmos.color = color;
    Gizmos.DrawRay(pos, direction);

    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
    Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
  }

  public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
  {
    Debug.DrawRay(pos, direction);

    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Debug.DrawRay(pos + direction, right * arrowHeadLength);
    Debug.DrawRay(pos + direction, left * arrowHeadLength);
  }
  public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
  {
    Debug.DrawRay(pos, direction, color);

    Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
    Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
    Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
  }
}