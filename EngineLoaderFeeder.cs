using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EngineLoaderFeeder : MonoBehaviour {

  protected List<string> scene_names;

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
