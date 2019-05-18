using System.Collections.Generic;
using System.IO;
using System;

public class HalperIO
{
  static public FileStream getStream(string path)
  {
    if (!File.Exists(path)) File.Create(path);
    return File.OpenWrite(path);
  }
    
  static public List<string> getLinesOfFile(string path)
  {
    throw new NotImplementedException("todo");

    /*
    FileStream fs = getStream(path);
    //fs.
    //string[] splitted = patternFile.text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    return null;
    */
  }

  static public string getFolderPathByName(string basePath, string folderName)
  {
    //this returns FULL PATHs
    string[] dirs = Directory.GetDirectories(basePath);
    
    foreach (string dir in dirs)
    {
      if (dir.ToLower().EndsWith(folderName.ToLower())) return dir;
      string output = getFolderPathByName(dir, folderName);
      if (output.Length > 0) return output;
    }

    return "";
  }




  /// <summary>
  /// returns true if last folder in path is the same as folderName param
  /// </summary>
  static public bool isLastFolderInPath(string path, string folderName)
  {
    path = cleanPathFromFile(path);
    return path.EndsWith(folderName);
  }

  /// <summary>
  /// is the last folder in path is a folder starting with a '.' char (ex : git related)
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  static public bool isLastFolderDotFolder(string path)
  {
    path = path.Replace("\\", "/"); // make path with all /

    // remove all last /
    while (path[path.Length - 1] == '/')
    {
      path.Substring(0, path.Length - 1);
    }

    string[] split = path.Split('/');
    return split[split.Length - 1].StartsWith(".");
  }

  /// <summary>
  /// removes file name and extension from given path
  /// also replace \ by /
  /// </summary>
  /// <param name="path"></param>
  /// <returns></returns>
  static protected string cleanPathFromFile(string path)
  {
    path = path.Replace("\\", "/");

    int extIndex = path.LastIndexOf('.'); // some folder have .
    int lastSlash = path.LastIndexOf('/');

    if (extIndex > 0)
    {
      path = path.Substring(0, lastSlash); // remove any file name
    }

    if (path[path.Length - 1] == '/') path.Substring(0, path.Length - 1); // remove last /

    return path;
  }

}
