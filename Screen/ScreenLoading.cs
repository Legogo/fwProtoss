using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// to call loading screen before everything else put <EngineLoadingScreenFeeder> in startup scene
/// </summary>

public class ScreenLoading : ScreenObject {
  
  static protected ScreenLoading _instance;

  Camera cam;
  public Text txt;

  [RuntimeInitializeOnLoadMethod]
  static public void runetimeInit()
  {

    string filter = EngineLoader.isContextEngineCompatible();
    if (filter.Length > 0)
    {
      Debug.LogWarning("won't load engine here : scene starts with prefix " + filter);
      return;
    }

    string scName = "loading";
    //if (overrideLoadingScreenName.Length > 0) scName = overrideLoadingScreenName;

    if (!scName.StartsWith("screen")) scName = "screen-" + scName;

    if (!EngineLoader.isSceneAdded(scName) && EngineLoader.checkIfCanBeLoaded(scName))
    {
      SceneManager.LoadSceneAsync(scName, LoadSceneMode.Additive);
    }
    
    //HalperGameObject.checkDestroyOnSoloMono(this);
  }

  protected override void build()
  {
    base.build();

    _instance = this;

    txt.enabled = false;

    //loading must be sticky to not be closed by maanger open/close logic
    //of other screens
    if (!sticky) sticky = true;
    
    cam = GetComponent<Camera>();
    if (cam == null) cam = GetComponentInChildren<Camera>();

    //Debug.Log("hiding loading screen through static call");
    show();
  }

  static public void showLoadingScreen()
  {
    if (_instance == null) return;

    _instance.show();
  }

  static public void hideLoadingScreen()
  {
    Debug.Log("hiding loading screen through static call");

    if (_instance == null)
    {
      Debug.LogWarning("asking to hide loading but instance is null ?");
      return;
    }

    _instance.forceHide();
  }
  
}
