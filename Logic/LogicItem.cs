using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// Le bridge qui permet a un object d'avoir des <LogicCapacity>
/// </summary>

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

  public override void restart()
  {
    base.restart();
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

  public override void updateEngine()
  {
    base.updateEngine();
    
    //Debug.Log(GetType()+" , "+ name, gameObject);

    for (int i = 0; i < capacities.Count; i++)
    {
      if (capacities[i].isLocked()) continue;
      capacities[i].updateCapacity();
    }
  }

  public override void updateEngineLate()
  {
    base.updateEngineLate();
    for (int i = 0; i < capacities.Count; i++) capacities[i].updateCapacityLate();
  }

  public void subscribeCapacity(LogicCapacity capa)
  {
    capacities.Add(capa);
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
