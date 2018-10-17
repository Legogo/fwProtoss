using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class EditorContextMenu : MonoBehaviour {

  [MenuItem("Assets/clear console")]
  static protected void clear_console() {
    HalperEditorContextMenu.ClearConsole();
  }
  
  [MenuItem("Assets/git")]
  static protected void openGit()
  {
    //string fullPath = Path.Combine(Environment.CurrentDirectory, "/YourSubDirectory/yourprogram.exe");
    string fullPath = "git";

    startCmd("git", "--cd=" + Environment.CurrentDirectory);
    
    fullPath = Path.Combine(Environment.CurrentDirectory, "Assets/Lib/fwProtoss");
    if (Directory.Exists(fullPath))
    {
      startCmd("git", "--cd=" + fullPath);
    }
    
  }
  
  static protected void startCmd(string fullPath, string args)
  {
    ProcessStartInfo startInfo = new ProcessStartInfo(fullPath);
    startInfo.WindowStyle = ProcessWindowStyle.Normal;
    startInfo.Arguments = args;

    //Debug.Log(Environment.CurrentDirectory);

    Process.Start(startInfo);

  }

}
