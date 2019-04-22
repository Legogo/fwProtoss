using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fwp;

public class CapacityInventoryStartup : LogicCapacity
{
  public ItemHeaderData[] startup_items;

  CapacityInventory inv;
  
  public override void setupCapacity()
  {
    base.setupCapacity();
    inv = _owner.getCapacity<CapacityInventory>();
  }

  public override void restartCapacity()
  {
    base.restartCapacity();

    inv.removeAllItems();
    inv.addItemsByHeaders(startup_items);
  }

}
