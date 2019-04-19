
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace fwp
{

  abstract public class CapacityInventory : LogicCapacity {
  
    List<InventoryItem> items = new List<InventoryItem>();

    public Action<InventoryItem> onItemAdded;

    [Serializable]
    public class ItemStartupData
    {
      public string uid;
      public int qty;
    }

    public ItemStartupData[] startup_items;

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

      for (int i = 0; i < startup_items.Length; i++)
      {
        addItem(startup_items[i].uid, startup_items[i].qty);
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

    public InventoryItem addItem(string uid, int qty = 1)
    {
      if(uid.Length <= 0)
      {
        Debug.LogWarning("no uid given ?");
        return null;
      }

      InventoryItem ii = getItem(uid);
      if (ii == null) {
        ii = new InventoryItem(uid);
        ii.setQuantity(qty);
        addItem(ii);
      }
      return ii;
    }
  
    public InventoryItem addItem(InventoryItem newItem)
    {
      if (newItem == null) Debug.LogWarning("no item given ?");

      InventoryItem ii = getItem(newItem.getId());
      if (ii == null)
      {
        items.Add(newItem);
        if (onItemAdded != null) onItemAdded(ii);
      }

      ii.setQuantity(newItem.getQuantity());

      return ii;
    }

    public int countItems()
    {
      return items.Count;
    }

    public InventoryItem[] getItems(string filterUid = "")
    {
      if (filterUid.Length <= 0) return items.ToArray();

      List<InventoryItem> output = new List<InventoryItem>();
      for (int i = 0; i < items.Count; i++)
      {
        if (!items[i].isItem(filterUid)) output.Add(items[i]);
      }
      return output.ToArray();
    }

    public InventoryItem getItem(int idx)
    {
      if (idx < 0)
      {
        Debug.LogWarning("asking for item " + idx + " ?");
        return null;
      }
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
        InventoryItem item = items[i];
        ct += "\n  "+ ((selectionIdx == i) ? ">" : " ") + item.getId() + " x " + item.getQuantity();
      }
      return ct;
    }
  }

}