using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fwp;

public class CapacityInventoryStartup : LogicCapacity
{
  public ItemHeaderData[] startup_items;

  CapacityInventory inv;

  protected override void setup()
  {
    base.setup();
    inv = _owner.getCapacity<CapacityInventory>();
    if (inv == null) Debug.LogError("need inventory");
  }

  public override void restartCapacity()
  {
    base.restartCapacity();

    inv.removeAllItems();
    inv.addItemsByHeaders(startup_items);
  }

}
