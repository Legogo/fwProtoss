using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// layer to be able to store huge list of positions
/// custom gui
/// </summary>

[CreateAssetMenu(menuName = "protoss/create ScriptablePositions", order = 100)]
public class ScriptablePositions : ScriptableObject {
  
  public Vector3[] positions;

}





#if UNITY_EDITOR
[CustomEditor(typeof(ScriptablePositions))]
public class DataPositionsEditor : Editor
{

  override public void OnInspectorGUI()
  {

    ScriptablePositions obj = (ScriptablePositions)target;

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