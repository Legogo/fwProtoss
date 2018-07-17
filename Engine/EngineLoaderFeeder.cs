using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
//using UnityEditor.SceneManagement;
using UnityEditor;
#endif
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

#if UNITY_EDITOR
  [ContextMenu("fetch screens")]
  protected void fetchScreensRefs()
  {
    List<string> screens = new List<string>();
    for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
    {
      EditorBuildSettingsScene data = EditorBuildSettings.scenes[i];
      if (data.path.Contains("screen-"))
      {
        string[] split = data.path.Split('-'); // screen-xxx
        split = split[split.Length - 1].Split('.'); // remove .asset
        Debug.Log("adding " + split[0]);
        screens.Add(split[0]);
      }
    }

    this.screen_names = screens.ToArray();

    UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
  }
#endif
}
