using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Le bridge qui permet a un object d'avoir des <LogicCapacity>
/// </summary>

public class LogicItem : ArenaObject
{
  protected List<LogicCapacity> capacities = new List<LogicCapacity>();
  
  protected override void fetchGlobal()
  {
    base.fetchGlobal();
    setupCapacities();
  }
  
  /* after scenes load */
  protected void setupCapacities()
  {
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
    for (int i = 0; i < capacities.Count; i++) capacities[i].updateLogic();
  }

  public override void updateEngineLate()
  {
    base.updateEngineLate();
    for (int i = 0; i < capacities.Count; i++) capacities[i].updateLogicLate();
  }

  public void subscribeCapacity(LogicCapacity capa)
  {
    capacities.Add(capa);
  }
}
