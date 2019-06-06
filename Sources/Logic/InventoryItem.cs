using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// objet abstrait qui représente le fait qu'un perso possède des objets de ce type
/// l'item possède la quantité total
/// </summary>

namespace fwp
{
  [System.Serializable]
  public class InventoryItem
  {
    public ItemHeaderData data; // id & qty in data
    public Vector2 range = new Vector2(0f, -1f); // store capacity

    //this won't init as 0,-1 but always 0,0 by default ...
    //Vector2 range;
    
    public InventoryItem(string uid)
    {
      if (data == null) data = new ItemHeaderData();

      data.uid = uid;

      setQuantity(0);
    }

    public InventoryItem setupRange(int min, int max)
    {
      range.x = min;
      range.y = max;
      return this;
    }

    public bool add(int step)
    {
      data.qty += step;

      //Debug.Log(uid + " x " + qty);

      if (range.y > -1)
      {
        data.qty = Mathf.Min(data.qty, (int)range.y);
        return true;
      }

      return false;
    }

    /// <summary>
    /// returns false if can't remove (not enought quantity of this item)
    /// </summary>
    /// <param name="step"></param>
    /// <returns></returns>
    public bool remove(int step)
    {
      if(step > data.qty)
      {
        return false;
      }

      data.qty -= step;

      return true;
    }
    
    public void setQuantity(int newQty) { data.qty = newQty; }
    public int getQuantity() { return data.qty; }
    public string getId() { return data.uid; }

    public bool isItem(string nm)
    {
      if (data.uid.Contains(nm)) return true;
      return false;
    }

    public ItemHeaderData split(int splitQty)
    {
      if (remove(splitQty))
      {
        ItemHeaderData ii = copy();
        ii.qty = splitQty;
        return ii;
      }

      return null;
    }

    public ItemHeaderData copy()
    {
      //Debug.Log(data.uid +" , "+range.y);
      ItemHeaderData newData = new ItemHeaderData();
      newData.uid = getId();
      newData.qty = getQuantity();
      return newData;
    }
  }

}


[System.Serializable]
public class ItemHeaderData
{
  public string uid;
  public int qty;
}