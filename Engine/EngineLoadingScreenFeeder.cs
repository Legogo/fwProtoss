using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// this script is meant to load and display loading screen asap
/// </summary>

public class EngineLoadingScreenFeeder : MonoBehaviour {

  [Header("default is 'loading'")]
  public string overrideLoadingScreenName = "";

  private void Awake()
  {
    Scene? foundScene = null;

    string scName = "loading";
    if (overrideLoadingScreenName.Length > 0) scName = overrideLoadingScreenName;

    if(!scName.StartsWith("screen")) scName = "screen-" + scName;

    for (int i = 0; i < SceneManager.sceneCount; i++)
    {
      if (foundScene != null) continue;

      Scene sc = SceneManager.GetSceneAt(i);
      if (sc.IsValid()) // check exists
      {
        if (sc.name.Contains(scName))
        {
          //Debug.Log("found " + sc.name);
          foundScene = sc;
        }
      }

    }

    if (foundScene != null)
    {
      Debug.Log("found " + foundScene.Value.name);
      SceneManager.LoadSceneAsync(foundScene.Value.name, LoadSceneMode.Additive);
    }
    else
    {
      Debug.LogWarning("no loading screen setup ?");
    }
    
    DestroyImmediate(gameObject);
  }
  
}
