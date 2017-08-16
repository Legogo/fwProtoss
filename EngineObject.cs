using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineObject : MonoBehaviour {

  static List<EngineObject> eos = new List<EngineObject>();
  //static int loadingFrameCount = 10;

  public bool freeze = false;

  void Awake()
  {
    build();
    
    if (!EngineManager.isLive())
    {
      eos.Add(this);
    }
  }

  IEnumerator Start()
  {
    //tt le monde a fait son build
    setup();

    //already loaded
    if (eos.IndexOf(this) > -1)
    {
      
      yield return new WaitForSeconds(1f);

      eos.Remove(this);

      //Debug.Log(eos.Count);

      if (!EngineManager.isLive() && eos.Count <= 0)
      {
        Debug.Log(name + " called <b>end of loading</b>", gameObject);
        EngineManager.manager.game_loading_done();
      }
    }

  }

  virtual protected void build()
  {

  }

  virtual protected void setup()
  {

  }

  void Update()
  {
    if (freeze) return;

    if (!EngineManager.isLive()) return;

    update();
  }

  virtual protected void update()
  {

  }
}
