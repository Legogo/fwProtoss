using UnityEngine.SceneManagement;
using UnityEngine;

public class EngineLoadingFeeder : MonoBehaviour {

  public string loadingScreenSceneName = "screen-loading";

  private void Awake()
  {
    bool found = false;
    for (int i = 0; i < SceneManager.sceneCount; i++)
    {
      if (SceneManager.GetSceneAt(i).name.Contains(loadingScreenSceneName))
      {
        found = true;
      }
    }

    if (!found)
    {
      SceneManager.LoadSceneAsync(loadingScreenSceneName, LoadSceneMode.Additive);
    }
    
    DestroyImmediate(gameObject);
  }
  
}
