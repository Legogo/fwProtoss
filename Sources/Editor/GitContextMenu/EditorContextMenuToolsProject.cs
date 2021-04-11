using UnityEngine;
using UnityEditor;
using System;
using System.IO;

/// <summary>
/// various context menus related to project management
/// </summary>

public class EditorContextMenuToolsProject
{

  [MenuItem("Assets/readme")]
  static protected void openReadme()
  {
    //string fullPath = Path.Combine(Environment.CurrentDirectory, "/YourSubDirectory/yourprogram.exe");
    string fullPath = Environment.CurrentDirectory + "/README.md";

    //Debug.Log(fullPath);

    if (!File.Exists(fullPath))
    {
      File.Create(fullPath).Close();
    }

    HalperNatives.startCmd(fullPath);
  }

}
