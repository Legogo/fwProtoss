using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterLoading : EngineObject {
  public override void onEngineSceneLoaded()
  {
    base.onEngineSceneLoaded();

    GameObject.DestroyImmediate(gameObject);
  }
}
