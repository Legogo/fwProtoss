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
    string scName = "loading";
    if (overrideLoadingScreenName.Length > 0) scName = overrideLoadingScreenName;

    if (!scName.StartsWith("screen")) scName = "screen-" + scName;

    if (!EngineLoader.isSceneAdded(scName))
    {
      SceneManager.LoadSceneAsync(scName, LoadSceneMode.Additive);
    }

    HalperGameObject.checkDestroyOnSoloMono(this);
  }
  
}
