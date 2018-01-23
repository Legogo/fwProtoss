using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// USed to declare specific additionnal scene to load on startup
/// </summary>

public class EngineLoaderFeeder : MonoBehaviour {

  protected List<string> scene_names;

  public string[] resource_names;
  public string[] screen_names;
  public string[] other_names;

  virtual public string[] feed()
  {
    if (scene_names == null) scene_names = new List<string>();

    addWithPrefix("resource", resource_names);
    addWithPrefix("screen", screen_names);
    addWithPrefix("", other_names);

    return scene_names.ToArray();
  }

  protected void addWithPrefix(string prefix, string[] names)
  {
    if (names.Length <= 0) return;

    //Debug.Log(prefix + " count ? " + names.Length);

    for (int i = 0; i < names.Length; i++)
    {
      scene_names.Add(prefix + "-"+ names[i]);
    }
  }

  public string[] getNames()
  {
    return scene_names.ToArray();
  }

  static public EngineLoaderFeeder get()
  {
    return GameObject.FindObjectOfType<EngineLoaderFeeder>();
  }

}
