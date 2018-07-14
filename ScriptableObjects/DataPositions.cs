using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "data/new DataPositions", order = 100)]
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

    for (int i = 0; i < 20; i++)
    {
      EditorGUILayout.Vector3Field("#" + i, obj.positions[i]);
    }

    EditorGUILayout.LabelField("...");

  }
}

#endif