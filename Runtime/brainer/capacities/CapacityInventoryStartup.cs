using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using inventeer;
using brainer;
using brainer.capacity;

public class CapacityInventoryStartup : brainer.BrainerLogicCapacity
{
  public ItemHeaderData[] startup_items;

  CapacityInventory inv;

  public override void setupCapacity()
  {
    base.setupCapacity();

    inv = brain.getCapacity<CapacityInventory>();
    if (inv == null) Debug.LogError("need inventory");
  }

  public override void restartCapacity()
  {
    base.restartCapacity();

    inv.removeAllItems();
    inv.addItemsByHeaders(startup_items);
  }

}
