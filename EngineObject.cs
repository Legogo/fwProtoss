using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineObject : MonoBehaviour {

  //loading list
  static public List<EngineObject> eos = new List<EngineObject>();

  protected EngineManager _eManager;

  //static int loadingFrameCount = 10;
  
  protected bool _freeze = false;

  void Awake()
  {
    eos.Add(this);

    build();
  }

  IEnumerator Start()
  {
    yield return null;

    setup();

    yield return null;

    EngineManager.checkForStartup();
  }

  virtual protected void build()
  {
    
  }
  
  //quand les scène sont add
  virtual protected void setup() {
    
  }
  
  //must be called by a manager
  virtual public void update()
  {
    //...
  }

  private void OnDestroy()
  {
      if(eos.IndexOf(this) > -1) eos.Remove(this);
  }

  public bool isFreezed() { return _freeze; }
  public void setFreeze(bool flag) { _freeze = flag; }

}
