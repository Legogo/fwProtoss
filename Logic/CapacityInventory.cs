using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapacityInventory : LogicCapacity {
  
  List<InventoryItem> items = new List<InventoryItem>();

  protected override void setup()
  {
    base.setup();

    //clear prev
    if(items != null && items.Count > 0)
    {
      for (int i = 0; i < items.Count; i++)
      {
        items[i] = null;
      }
      items.Clear();
    }
    
  }

  public bool hasSome(string uid)
  {
    InventoryItem ii = getItem(uid);
    if(ii != null)
    {
      return ii.getQuantity() > 0;
    }
    return false;
  }

  public InventoryItem addItem(InventoryItem newItem)
  {
    InventoryItem ii = getItem(newItem.getId());
    if(ii == null) items.Add(newItem);
    else
    {
      ii.add(newItem.getQuantity() - ii.getQuantity());
    }

    return ii;
  }

  public int countItems()
  {
    return items.Count;
  }

  public InventoryItem getItem(int idx)
  {
    if(items.Count > idx) return items[idx];
    return null;
  }

  public InventoryItem getItem(string id)
  {
    for (int i = 0; i < items.Count; i++)
    {
      if (items[i].isItem(id)) return items[i];
    }
    return null;
  }

  public override string toString()
  {
    string ct = base.toString();
    ct += "\n" + debug_display();
    return ct;
  }

  public string debug_display(int selectionIdx = -1)
  {
    string ct = "\n logic : "+name;
    for (int i = 0; i < items.Count; i++)
    {
      MarketInventoryItem item = items[i] as MarketInventoryItem;
      ct += "\n  "+ ((selectionIdx == i) ? ">" : " ") + item.symbol + " x " + item.getQuantity();
    }
    return ct;
  }
}
