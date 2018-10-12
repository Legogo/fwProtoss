using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Le bridge qui permet a un object d'avoir des <LogicCapacity>
/// </summary>

namespace fwp
{
  public class LogicItem : ArenaObject
  {
    protected List<LogicCapacity> capacities = new List<LogicCapacity>();

    [HideInInspector]
    public CapacityInput input;

    protected override void setupEarly()
    {
      base.setupEarly();
      input = GetComponent<CapacityInput>();
    }

    protected override void setup()
    {
      base.setup();

      //Debug.Log(GetType() + " , " + name + " , setup");

      setupCapacities();
    }

    public override void arena_round_restart()
    {
      base.arena_round_restart();
      for (int i = 0; i < capacities.Count; i++) capacities[i].restartCapacity();
    }

    /* after scenes load */
    protected void setupCapacities()
    {
      //Debug.Log(GetType() + " , "+ name+" , setup capacs !");
      for (int i = 0; i < capacities.Count; i++) capacities[i].earlySetupCapacity();
      for (int i = 0; i < capacities.Count; i++) capacities[i].setupCapacity();
    }

    virtual public void clean()
    {
      for (int i = 0; i < capacities.Count; i++) capacities[i].clean();
    }

    protected override void updateArenaLive(float timeStamp)
    {
      base.updateArenaLive(timeStamp);
      for (int i = 0; i < capacities.Count; i++)
      {
        if (capacities[i].isLocked()) continue;
        capacities[i].updateCapacity();
      }
    }

    protected override void updateArenaLiveLate(float timeStamp)
    {
      base.updateArenaLiveLate(timeStamp);
      for (int i = 0; i < capacities.Count; i++)
      {
        if (capacities[i].isLocked()) continue;
        capacities[i].updateCapacityLate();
      }
    }

    public void subscribeCapacity(LogicCapacity capa)
    {
      capacities.Add(capa);
    }

    public T getCapacity<T>() where T : LogicCapacity
    {
      return (T)capacities.FirstOrDefault(x => x != null && typeof(T).IsAssignableFrom(x.GetType()));
    }
    public void lockCapacity<T>(bool flag) where T : LogicCapacity
    {
      //LogicCapacity lc = capacities.FirstOrDefault(x => x != null && x.GetType().IsAssignableFrom(typeof(T)));
      LogicCapacity lc = getCapacity<T>();
      if (lc != null)
      {
        lc.lockCapacity(flag);
      }
      else
      {
        Debug.Log(typeof(T) + " NOT FOUND , " + flag + " , " + name);
      }

    }

    public void forceWithinBounds(Rect boundsClamp)
    {
      if (visibility == null) Debug.LogWarning("asking for bounds clamping but no visible module");

      Rect localRec = visibility.getWorldBounds();
      float gap = 0f;

      //Debug.Log(localRec);
      //Debug.Log(boundsClamp);

      gap = boundsClamp.xMax - localRec.xMax;
      if (gap < 0f) transform.position += Vector3.right * gap;

      gap = boundsClamp.xMin - localRec.xMin;
      //Debug.Log(boundsClamp.xMin + " - " + localRec.xMin + " = xmin " + gap);
      if (gap > 0f) transform.position += Vector3.right * gap;

      gap = boundsClamp.yMax - localRec.yMax;
      //Debug.Log(boundsClamp.yMax+" - "+localRec.yMax+" = ymax " + gap);
      if (gap < 0f) transform.position += Vector3.up * gap;

      gap = boundsClamp.yMin - localRec.yMin;
      //Debug.Log(boundsClamp.yMin + " - " + localRec.yMin + " = ymin " + gap);
      if (gap > 0f) transform.position += Vector3.up * gap;

    }

    /*
    public LogicCapacity getCapacity<T>() where T : LogicCapacity
    {
      for (int i = 0; i < capacities.Count; i++)
      {
        if (capacities[i].GetType() == typeof(T)) return capacities[i];
      }
      return null;
    }
    */
  }

}
