using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;
using System.IO;

public class EditorGitShortcuts : MonoBehaviour
{
  [MenuItem("Assets/git")]
  static protected void openGit()
  {
    //string fullPath = Path.Combine(Environment.CurrentDirectory, "/YourSubDirectory/yourprogram.exe");
    //string fullPath = getFolderPathContainingGit(Environment.CurrentDirectory);

    //start a git cmd within project context (../Assets)
    //Debug.Log("base root git folder : " + fullPath);

    HalperEditor.ClearConsole();

    string fullPath = Environment.CurrentDirectory;

    //Debug.Log("opening gits from "+ fullPath);
    
    if (folderHasGitFolder(fullPath))
    {
      Debug.Log("opening git bash for base git folder");
      HalperNatives.startCmd("git", "--cd=" + fullPath);
    }

    //Debug.Log("seaching for other gits");

    //searcg for protoss git folder and open it
    string path = getFolderPathContainingGit(fullPath, "protoss");
    if(path.Length > 0)
    {
      Debug.Log("opening git bash for found git at path : " + path);
      HalperNatives.startCmd("git", "--cd=" + path);
    }
    else
    {
      Debug.LogWarning("no other git folder");
    }

  }
  
  static public bool folderHasGitFolder(string basePath)
  {
    string[] dirs = Directory.GetDirectories(basePath);
    for (int i = 0; i < dirs.Length; i++)
    {
      string path = dirs[i];
      path = path.ToLower();
      if (path.Contains(".git")) return true;
    }
    return false;
  }

  static public string getFolderPathContainingGit(string basePath, string folderName)
  {
    //Debug.Log("path : " + basePath);

    folderName = folderName.ToLower();
    basePath = basePath.ToLower();

    if (folderHasGitFolder(basePath)) return basePath;

    //this returns FULL PATHs
    string[] dirs = Directory.GetDirectories(basePath);

    foreach (string dir in dirs)
    {
      string output = getFolderPathContainingGit(dir, folderName);
      if (output.Length > 0) return output;
    }
    
    return "";
  }
}
