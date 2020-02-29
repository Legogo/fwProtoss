using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalperBounds
{
  // Pass in a game object and a Vector3[8], and the corners of the mesh.bounds in 
  //   in world space are returned in the passed array;
  public static void GetBoundsPointsNoAlloc(GameObject go, Vector3[] points)
  {
    if (points == null || points.Length < 8)
    {
      Debug.Log("Bad Array");
      return;
    }
    MeshFilter mf = go.GetComponent<MeshFilter>();
    if (mf == null)
    {
      Debug.Log("No MeshFilter on object");
      for (int i = 0; i < points.Length; i++)
        points[i] = go.transform.position;
      return;
    }

    Transform tr = go.transform;

    Vector3 v3Center = mf.sharedMesh.bounds.center;
    Vector3 v3ext = mf.sharedMesh.bounds.extents;

    points[0] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y + v3ext.y, v3Center.z - v3ext.z));  // Front top left corner
    points[1] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y + v3ext.y, v3Center.z - v3ext.z));  // Front top right corner
    points[2] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y - v3ext.y, v3Center.z - v3ext.z));  // Front bottom left corner
    points[3] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y - v3ext.y, v3Center.z - v3ext.z));  // Front bottom right corner
    points[4] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y + v3ext.y, v3Center.z + v3ext.z));  // Back top left corner
    points[5] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y + v3ext.y, v3Center.z + v3ext.z));  // Back top right corner
    points[6] = tr.TransformPoint(new Vector3(v3Center.x - v3ext.x, v3Center.y - v3ext.y, v3Center.z + v3ext.z));  // Back bottom left corner
    points[7] = tr.TransformPoint(new Vector3(v3Center.x + v3ext.x, v3Center.y - v3ext.y, v3Center.z + v3ext.z));  // Back bottom right corner
  }
}
