
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace fwp
{

  abstract public class CapacityInventory : LogicCapacity
  {
    [SerializeField]
    protected List<InventoryItem> items;
    
    public Action<InventoryItem> onItemAdded;

    protected override void build()
    {
      base.build();

      if (items == null) items = new List<InventoryItem>();
    }
    
    public bool hasSome(string uid)
    {
      InventoryItem ii = getItem(uid);
      if (ii != null)
      {
        return ii.getQuantity() > 0;
      }
      return false;
    }

    public InventoryItem addItem(string uid, int qty = 1)
    {
      if (uid.Length <= 0)
      {
        Debug.LogWarning("no uid given ?");
        return null;
      }

      InventoryItem ii = getItem(uid);
      if (ii == null)
      {
        ii = new InventoryItem(uid);
        ii.setQuantity(qty);
        addItem(ii);
      }
      return ii;
    }


    public InventoryItem addItem(ItemHeaderData data)
    {
      InventoryItem ii = new InventoryItem(data.uid);
      ii.setQuantity(data.qty);
      return addItem(ii);
    }

    /// <summary>
    /// add new item to inventory
    /// </summary>
    /// <param name="newItem"></param>
    /// <returns></returns>
    public InventoryItem addItem(InventoryItem newItem)
    {
      if (newItem == null) Debug.LogWarning("no item given ?");

      //check if another item of same id is already in inventory
      InventoryItem ii = getItem(newItem.getId());

      if (ii == null)
      {
        ii = newItem;
        items.Add(ii);
        if (onItemAdded != null) onItemAdded(ii);
      }
      else
      {
        //Debug.Log("adding " + newItem.getQuantity() + " to stack");

        //item already exist in inventory, adding quantity to existing item
        ii.add(newItem.getQuantity());
      }
      
      return ii;
    }

    public int countItems()
    {
      return items.Count;
    }

    public ItemHeaderData[] getHeaders()
    {
      InventoryItem[] items = getItems();
      List<ItemHeaderData> headers = new List<ItemHeaderData>();
      for (int i = 0; i < items.Length; i++)
      {
        headers.Add(items[i].data);
      }
      return headers.ToArray();
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
      if (items.Count > idx) return items[idx];
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

    public void removeAllItems()
    {
      items.Clear();
    }

    public void addItemsByHeaders(ItemHeaderData[] headers)
    {
      for (int i = 0; i < headers.Length; i++)
      {
        addItem(headers[i]);
      }
    }

    public void addItems(InventoryItem[] newItems, bool makeCopies = false)
    {

      for (int i = 0; i < newItems.Length; i++)
      {
        if (makeCopies) addItem(newItems[i].copy());
        else addItem(newItems[i]);
      }

    }

    public override string toString()
    {
      string ct = base.toString();
      ct += "\n" + debug_display();
      return ct;
    }

    public string debug_display(int selectionIdx = -1)
    {
      string ct = "\n logic : " + name;
      for (int i = 0; i < items.Count; i++)
      {
        InventoryItem item = items[i];
        ct += "\n  " + ((selectionIdx == i) ? ">" : " ") + item.getId() + " x " + item.getQuantity();
      }
      return ct;
    }
  }

}
