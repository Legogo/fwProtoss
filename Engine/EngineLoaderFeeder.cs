using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// USed to declare specific additionnal scene to load on startup
/// </summary>

public class EngineLoaderFeeder : MonoBehaviour {

  public List<string> scene_names;

  virtual public string[] feed()
  {
    if (scene_names == null) scene_names = new List<string>();
    return scene_names.ToArray();
  }

  static public EngineLoaderFeeder get()
  {
    return GameObject.FindObjectOfType<EngineLoaderFeeder>();
  }

}
