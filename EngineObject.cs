using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineObject : MonoBehaviour {

  //loading list
  static List<EngineObject> eos = new List<EngineObject>();

  //static int loadingFrameCount = 10;

  protected bool _freeze = false;

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
        EngineManager.get().game_loading_done();
      }
    }

  }

  virtual protected void build()
  {

  }

  virtual protected void setup()
  {

  }

  private void Update()
  {
    if (_freeze) return;
    update();
  }

  //must be called by a manager
  virtual public void update()
  {
    //...
  }
  
  public bool isFreezed() { return _freeze; }
  public void setFreeze(bool flag) { _freeze = flag; }
}
