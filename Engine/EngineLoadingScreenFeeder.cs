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
    bool found = false;

    string scName = "loading";
    if (overrideLoadingScreenName.Length > 0) scName = overrideLoadingScreenName;

    if(!scName.StartsWith("screen")) scName = "screen-" + scName;

    for (int i = 0; i < SceneManager.sceneCount; i++)
    {
      if (SceneManager.GetSceneAt(i).name.Contains(scName))
      {
        found = true;
      }
    }

    if (!found)
    {
      SceneManager.LoadSceneAsync(scName, LoadSceneMode.Additive);
    }
    
    DestroyImmediate(gameObject);
  }
  
}
