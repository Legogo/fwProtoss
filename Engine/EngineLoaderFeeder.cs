using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// USed to declare specific additionnal scene to load on startup
/// </summary>

public class EngineLoaderFeeder : MonoBehaviour {

  protected List<string> scene_names;

  [Header("prefix resource-")]
  public string[] resource_names;

  [Header("prefix screen-")]
  public string[] screen_names;

  [Header("prefix graphics-")]
  public string[] graphics_names;

  [Header("no prefix")]
  public string[] other_names;
  
  virtual public string[] feed()
  {
    if (scene_names == null) scene_names = new List<string>();

    addWithPrefix("resource-", resource_names);
    addWithPrefix("screen-", screen_names);
    addWithPrefix("graphics-", graphics_names);

    addWithPrefix("", other_names);

    GameObject.DestroyImmediate(this);

    return scene_names.ToArray();
  }

  protected void addWithPrefix(string prefix, string[] names)
  {
    if (names.Length <= 0) return;

    //Debug.Log(prefix + " count ? " + names.Length);

    for (int i = 0; i < names.Length; i++)
    {
      scene_names.Add(prefix + names[i]);
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
