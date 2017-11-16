using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum CoroutineLayerId { DEFAULT };
public enum CoroutineState { NONE, START, SKIP, INTERRUPT, DONE, ERROR };

public class CoroutineLayer
{
  public CoroutineLayerId id;
  public List<CoroutineInfo> infos;

  public CoroutineLayer(CoroutineLayerId id)
  {
    this.id = id;
    infos = new List<CoroutineInfo>();
  }
}

public class CoroutineInfo
{
  public CoroutineLayer layer;
  public MonoBehaviour caller;
  public Coroutine handle;
  public IEnumerator function;
  public CoroutineState state;

  public string context = ""; // for debug

  public Action onEnd;

  public bool paused = false; // loop tant qu'on enlève pas la pause
  public bool skipped = false; // va au bout en 1 frame et balance ses callbacks
  public bool stopped = false; // s'interrompt au milieu de ce qu'il est en train de faire et balance ses callbacks
  public bool killed = false; // s'interrompt au milieu et ne balance pas la suite (si callback)

  public CoroutineInfo(IEnumerator fct, MonoBehaviour caller, CoroutineLayer layer)
  {
    this.layer = layer;
    layer.infos.Add(this);

    this.caller = caller;
    this.function = fct;
    state = CoroutineState.NONE;
  }

  public CoroutineInfo setup(Action cbOnEnd) {
    this.onEnd = cbOnEnd;
    return this;
  }

  public CoroutineInfo kill() { killed = true; return this; }
  public CoroutineInfo stop() { stopped = true; return this; }
  public CoroutineInfo skip() { skipped = true; return this; }

  public bool isDone() { return state >= CoroutineState.DONE; }

  public CoroutineInfo removeFromLayer() {
    layer.infos.Remove(this);
    return this;
  }

  public string toString()
  {
    string ct = ""+state;
    
    string fctName = "null";
    if (function != null)
    {
      string[] split = function.ToString().Split('<');
      split = split[1].Split('>');
      fctName = split[0];
    }

    if(caller != null) ct += "  " + caller;
    ct += "  " + fctName+"()";

    if (context.Length > 0) ct += " " + context;
    return ct;
  }
}

public class CoroutineManager
{
  public List<CoroutineInfo> history;
  public CoroutineLayer[] layers;

  protected CoroutineManagerCarrier carrier; // l'objet qui permet d'appel les StartCoroutine()
  
  public CoroutineManager() {
    history = new List<CoroutineInfo>();

    GameObject obj = GameObject.Find("[coroutine]");
    if (obj == null) obj = GameObject.Find("(coroutine)");
    if (obj == null) obj = new GameObject("(coroutine)", typeof(CoroutineManagerCarrier));

    carrier = obj.GetComponent<CoroutineManagerCarrier>();
    if (carrier == null) Debug.LogError("{CoroutineManager} No carrier ?");

    GameObject.DontDestroyOnLoad(carrier);

    layers = new CoroutineLayer[Enum.GetNames(typeof(CoroutineLayerId)).Length];
    for (int i = 0; i < layers.Length; i++) { layers[i] = new CoroutineLayer((CoroutineLayerId)i); }
    //Debug.Log("setup");
  }

  public CoroutineInfo start(CoroutineLayerId layerId, IEnumerator scriptFunction, MonoBehaviour caller)
  {
    CoroutineInfo info = new CoroutineInfo(scriptFunction, caller, layers[(int)layerId]);
    
    carrier.StartCoroutine(checker(info));
    return info;
  }

  protected IEnumerator checker(CoroutineInfo info)
  {
    info.state = CoroutineState.START;
    
    #if UNITY_EDITOR
    if(info.caller != null) {
      Debug.Log("{{coroutine}} starting coroutine for caller : " + info.caller.name, info.caller);
    }
    else {
      Debug.Log("{{coroutine}} starting coroutine (no caller)");
    }
    #endif

    while (!info.stopped && info.function.MoveNext())
    {
      if (info.killed)
      {
        info.stopped = true;
        yield return null;
      }
      
      if(info.stopped) {
        if(info.state != CoroutineState.INTERRUPT) info.state = CoroutineState.INTERRUPT;
        yield return null;
      }

      while (info.paused)
      {
        if (info.stopped) info.paused = false;
        yield return null;
      }

      if (info.skipped)
      {
        info.state = CoroutineState.SKIP;
        while (info.function.MoveNext()) { }; // no yield, skipping in the same frame
      }

      yield return null;
    }

    if(info.caller) {
      Debug.Log("{{coroutine}} coroutine for caller " + info.caller.name + " is done (killed?" + info.killed + ", stopped?" + info.stopped + ", skipped?" + info.skipped + ")", info.caller);
    }

    info.state = CoroutineState.DONE;

    info.removeFromLayer();
    history.Add(info);

    if(!info.killed) {
      if (info.onEnd != null) info.onEnd();
    }
    
  }
  
  public string toString() {
    string ct = "[CoroutineManager](layers count "+layers.Length+")";
    //CoroutineInfo[] infos = get().inf
    for (int i = 0; i < layers.Length; i++)
    {
      ct += "\n <color=red>("+layers[i].id.ToString()+")</color>";
      for (int j = 0; j < layers[i].infos.Count; j++)
      {
        ct += "\n  " + layers[i].infos[j].toString();
      }
    }
    return ct;
  }

  static protected CoroutineManager manager;
  static public CoroutineManager get() {
    if (manager == null) manager = new CoroutineManager();
    return manager;
  }

  static public CoroutineInfo launch(IEnumerator scriptFunction) { return launch(CoroutineLayerId.DEFAULT, scriptFunction, null); }
  static public CoroutineInfo launch(IEnumerator scriptFunction, MonoBehaviour caller) { return launch(CoroutineLayerId.DEFAULT, scriptFunction, caller); }
  static public CoroutineInfo launch(CoroutineLayerId layer, IEnumerator scriptFunction, MonoBehaviour caller) { return get().start(layer, scriptFunction, caller); }

}
