using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EngineObject : MonoBehaviour, Interfaces.IDebugSelection
{
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

    if (!EngineManager.isLoading()) onEngineSceneLoaded();
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
  virtual public void updateEngine()
  {
    //...
  }

  virtual public void updateEngineLate()
  {

  }

  virtual public bool canUpdate()
  {
    if (isFreezed()) return false;
    return true;
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
  public void setFreeze(bool flag) {
    //Debug.Log("<b>" + name + "</b> , "+GetType()+" , setFreeze " + flag);
    _freeze = flag;
  }

  virtual public string toString()
  {
    return name + " freeze ? " + isFreezed();
  }

  public string toStringDebug()
  {
    return toString();
  }
}
