using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Le bridge qui permet a un object d'avoir des <LogicCapacity>
/// </summary>

public class LogicItem : ArenaObject
{
  protected List<LogicCapacity> capacities;

  protected override void build()
  {
    base.build();

    capacities = new List<LogicCapacity>();

    //on doit aller chercher les capa existantes du LD
    LogicCapacity[] temp = transform.GetComponentsInChildren<LogicCapacity>();
    for (int i = 0; i < temp.Length; i++)
    {
      //Debug.Log("  "+name+" :: "+temp[i].GetType());
      capacities.Add(temp[i]);
    }
    
  }

  public override void onEngineSceneLoaded()
  {
    base.onEngineSceneLoaded();
    setupCapacities();
  }

  /* after scenes load */
  protected void setupCapacities()
  {
    for (int i = 0; i < capacities.Count; i++)
    {
      capacities[i].setupCapacity();
    }
  }

  public override void updateEngine()
  {
    base.updateEngine();

    //Debug.Log(name + " update logic", gameObject);

    for (int i = 0; i < capacities.Count; i++)
    {
      capacities[i].updateLogic();
    }
  }

  public T addCapacity<T>() where T : LogicCapacity
  {
    T comp = gameObject.GetComponent<T>();
    if (comp == null) comp = gameObject.AddComponent<T>();
    if (capacities.IndexOf(comp) < 0) capacities.Add(comp);
    return comp;
  }

  public T getCapacity<T>() where T : LogicCapacity
  {
    for (int i = 0; i < capacities.Count; i++)
    {
       if (capacities[i].GetType() == typeof(T)) return (T)capacities[i];
    }
    //return addCapacity<T>();
    return default(T);
  }
  
}
