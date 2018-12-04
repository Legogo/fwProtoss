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

}
