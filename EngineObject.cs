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

  protected Transform _tr;
  protected bool _freeze = false;
  protected bool _ready = false;

  //constructor
  void Awake()
  {
    _tr = transform;

    _ready = false;
    eos.Add(this);

    //Debug.Log(name);
    //if(name.Contains("ui_")) Debug.Log(name+" <b>added to eos</b>");

    build();

    if (name.Contains("timer_"))
    {
      Debug.Log("<b>" + name + "." + GetType() + "</b> awake :: EngineManager loading ? "+ EngineManager.isLoading());
    }

    //Debug.Log(GetType() + " (on "+name+") awake() | count ? "+eos.Count);

    if (!EngineManager.isLoading()) onEngineSceneLoaded();
  }
  
  //called by loader
  virtual public void onEngineSceneLoaded()
  {
    fetchData();
    _ready = true;
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
    //Debug.Log(name + " OnDestroy ", gameObject);
    destroy();
  }

  virtual protected void destroy()
  {
    //Debug.Log(name + " destroy() ", gameObject);
    if (eos.IndexOf(this) > -1) eos.Remove(this);
  }

  public bool isFreezed() { return _freeze; }
  public void setFreeze(bool flag) {
    //Debug.Log("<b>" + name + "</b> , "+GetType()+" , setFreeze " + flag);
    _freeze = flag;
  }
  public bool isReady() { return _ready; }

  virtual public string toString()
  {
    return name + " freeze ? " + isFreezed()+" canUpdate("+canUpdate()+")";
  }

  public string toStringDebug()
  {
    return toString();
  }
}
