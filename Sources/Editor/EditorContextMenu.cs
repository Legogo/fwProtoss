using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class EditorContextMenu : MonoBehaviour {

  [MenuItem("Assets/readme")]
  static protected void openReadme()
  {
    //string fullPath = Path.Combine(Environment.CurrentDirectory, "/YourSubDirectory/yourprogram.exe");
    string fullPath = Environment.CurrentDirectory+"/README.md";

    //Debug.Log(fullPath);
    
    if (!File.Exists(fullPath))
    {
      File.Create(fullPath).Close();
    }

    HalperNatives.startCmd(fullPath);
  }
  
  [MenuItem("Tools/open persistant data path")]
  static public void osOpenDataPathFolder()
  {
    HalperNatives.os_openFolder(HalperNatives.getDataPath());
  }

}
