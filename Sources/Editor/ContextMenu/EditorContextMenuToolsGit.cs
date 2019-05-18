using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class EditorContextMenuToolsGit : MonoBehaviour
{
  //[MenuItem("Assets/git !#&%g")]
  [MenuItem("Assets/git &%g")]
  static protected void openGit()
  {
    //string fullPath = Path.Combine(Environment.CurrentDirectory, "/YourSubDirectory/yourprogram.exe");
    //string fullPath = getFolderPathContainingGit(Environment.CurrentDirectory);

    //start a git cmd within project context (../Assets)
    //Debug.Log("base root git folder : " + fullPath);

    HalperEditor.ClearConsole();

    string fullPath = Environment.CurrentDirectory;

    //Debug.Log("opening gits from "+ fullPath);
    
    if (pathHasGitFolder(fullPath))
    {
      Debug.Log("current path has git folder : opening");
      HalperNatives.startCmd("git", "--cd=" + fullPath);
    }

    //Debug.Log("seaching for other gits");
    openGitFolderByName("protoss");
  }
  
  static public void openGitFolderByName(string containsFolderName)
  {
    string fullPath = Environment.CurrentDirectory;

    //searcg for protoss git folder and open it
    string path = getFolderPathContainingGit(fullPath, containsFolderName);
    
    //Debug.Log("protoss : " + path);

    if (path.Length > 0)
    {
      Debug.Log("git with name "+containsFolderName+" found and opened");
      HalperNatives.startCmd("git", "--cd=" + path);
    }
    else
    {
      Debug.Log("no git folder with name : "+ containsFolderName);
    }

  }
  
  static public bool pathHasGitFolder(string basePath)
  {
    //Debug.Log(basePath);

    string path;

    //search for a /.git/ folder
    string[] dirs = Directory.GetDirectories(basePath);
    for (int i = 0; i < dirs.Length; i++)
    {
      path = dirs[i].ToLower();
      if (path.Contains(".git")) return true;
    }

    //search for .git file (submodules)
    string[] files = Directory.GetFiles(basePath);
    for (int i = 0; i < files.Length; i++)
    {
      path = files[i].ToLower();
      if (path.Contains(".git")) return true;
    }

    return false;
  }
  
  /// <summary>
  /// recurcively search for git folder
  /// </summary>
  /// <param name="basePath"></param>
  /// <param name="folderName"></param>
  /// <returns></returns>
  static private string getFolderPathContainingGit(string basePath, string folderName)
  {
    //Debug.Log("path : " + basePath);

    folderName = folderName.ToLower();
    basePath = basePath.ToLower();

    
    if(HalperIO.isLastFolderInPath(basePath, folderName))
    {
      //Debug.Log(basePath);

      if (pathHasGitFolder(basePath)) return basePath;
    }
    
    //this returns FULL PATHs
    string[] dirs = Directory.GetDirectories(basePath);

    foreach (string dir in dirs)
    {
      if (HalperIO.isLastFolderDotFolder(dir)) continue;

      string output = getFolderPathContainingGit(dir, folderName);
      if (output.Length > 0) return output;
    }
    
    return "";
  }
}
