using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// to be able to store huge list of positions
/// </summary>

[CreateAssetMenu(menuName = "protoss/create DataPositions", order = 100)]
public class DataPositions : ScriptableObject {
  
  public Vector3[] positions;

}

#if UNITY_EDITOR


[CustomEditor(typeof(DataPositions))]
public class DataPositionsEditor : Editor
{

  override public void OnInspectorGUI()
  {

    DataPositions obj = (DataPositions)target;

    EditorGUILayout.LabelField(obj.positions.Length + " positions");

    int qty = Math.Min(20, obj.positions.Length);
    for (int i = 0; i < qty; i++)
    {
      EditorGUILayout.Vector3Field("#" + i, obj.positions[i]);
    }

    EditorGUILayout.LabelField("...");

  }
}

#endif