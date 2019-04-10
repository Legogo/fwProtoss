using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
}
