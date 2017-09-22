using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineObject : MonoBehaviour {

  //loading list
  static public List<EngineObject> eos = new List<EngineObject>();

  protected EngineManager _eManager;

  //static int loadingFrameCount = 10;
  
  protected bool _freeze = false;

  //constructor
  void Awake()
  {
    eos.Add(this);
    build();
  }
  
  //called by loader
  virtual public void onEngineSceneLoaded()
  {
    fetchData();
    EngineManager.checkForStartup();
  }
  
  virtual protected void build()
  {
    
  }
  
  virtual protected void fetchData()
  {

  }

  //must be called by a manager
  virtual public void update()
  {
    //...
  }

  private void OnDestroy()
  {
    destroy();
  }

  virtual protected void destroy()
  {
    if (eos.IndexOf(this) > -1) eos.Remove(this);
  }

  public bool isFreezed() { return _freeze; }
  public void setFreeze(bool flag) { _freeze = flag; }

}
