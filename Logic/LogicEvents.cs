using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// where to plug logic/capacity to react to specific external (idealy only global system related) events (ex : plug / unplug controller)
/// </summary>

static public class LogicEvents {
  
  static public Dictionary<string, LogicEvent> events = new Dictionary<string, LogicEvent>();

  /* must be called once the manager exists */
  [RuntimeInitializeOnLoadMethod]
  static public void build()
  {
    EngineLoader el = EngineLoader.get();
    
    el.onLoadingDone += delegate () {
      LogicEventController e_controller = createEvent("controller", new LogicEventController()) as LogicEventController;
      ControllerManager cm = ControllerSelector.getInputManager();
      cm.subscribeToEvents(e_controller.onControllerConnected, e_controller.onControllerDisconnected);
    };
  }
  
  /* basic declaration */
  static public T createEvent<T>(string name) where T : LogicEvent
  {
    if (!events.ContainsKey(name)) events.Add(name, default(T));
    return events[name] as T;
  }
  /* if params are T on event creation */
  static public LogicEvent createEvent(string name, LogicEvent newEvent){
    if (!events.ContainsKey(name)) events.Add(name, newEvent);
    return events[name];
  }

  static public void subscribeController(Action<int> onNewControllerConnected, Action<int> onNewControllerDisconnected)
  {
    if (!events.ContainsKey("controller")) events.Add("controller", new LogicEventController());
    LogicEventController lec = events["controller"] as LogicEventController;
    lec.onControllerConnected += onNewControllerConnected;
    lec.onControllerDisconnected += onNewControllerDisconnected;
  }
  
  public class LogicEvent{
    public string name = "";
  }

  public class LogicEventController : LogicEvent
  {
    public Action<int> onControllerConnected;
    public Action<int> onControllerDisconnected;
  }
}

