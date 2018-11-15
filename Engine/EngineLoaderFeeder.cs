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

  [Header("prefix ui-")]
  public string[] ui_names; // ui element (overlay)

  [Header("prefix graphics-")]
  public string[] graphics_names; // objects (3d, sprite) ingame

  [Header("prefix screen-")]
  public string[] screens_names;

  [Header("no prefix")]
  public string[] other_names;
  
  /// <summary>
  /// generate list of scenes with exact names
  /// </summary>
  /// <returns></returns>
  virtual public string[] feed()
  {
    if (scene_names == null) scene_names = new List<string>();

    addWithPrefix("resource-", resource_names);
    addWithPrefix("ui-", ui_names);
    addWithPrefix("graphics-", graphics_names);
    addWithPrefix("screen-", screens_names);

    addWithPrefix("", other_names);

    GameObject.DestroyImmediate(this);

    return scene_names.ToArray();
  }

  protected void addWithPrefix(string prefix, string[] names)
  {
    if (names == null)
    {
      Debug.LogWarning("names is null for prefix " + prefix);
      return;
    }

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
  
#if UNITY_EDITOR

  protected string[] fetchScenesRefs(string type)
  {
    UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();

    List<string> screens = new List<string>();
    for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
    {
      EditorBuildSettingsScene data = EditorBuildSettings.scenes[i];
      if (data.path.Contains(type+"-"))
      {
        string[] split = data.path.Split('-'); // screen-xxx
        split = split[split.Length - 1].Split('.'); // remove .asset
        Debug.Log("adding " + split[0]);
        screens.Add(split[0]);
      }
    }

    return screens.ToArray();
  }

  [ContextMenu("fetch graphics")]
  protected void fetchGraphics()
  {
    this.graphics_names = fetchScenesRefs("graphics");
  }

  [ContextMenu("fetch uis")]
  protected void fetchUis()
  {
    this.ui_names = fetchScenesRefs("ui");
  }

  [ContextMenu("fetch screens")]
  protected void fetchScreens()
  {
    this.screens_names = fetchScenesRefs("screen");
  }

#endif
}
