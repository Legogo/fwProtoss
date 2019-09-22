using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineLoaderFeederBase : MonoBehaviour
{
  protected List<string> scene_names;

  virtual public string[] feed()
  {
    if (scene_names == null) scene_names = new List<string>();
    return scene_names.ToArray();
  }

  protected void addWithPrefix(string prefix, string nm)
  {
    addWithPrefix(prefix, new string[] { nm });
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

}
