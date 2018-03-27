using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItem {

  protected string uid = "";
  protected Vector2 range = new Vector2(0,-1);
  protected int qty = 0;
  
  public InventoryItem(string uid, int min = 0, int max = -1)
  {
    this.uid = uid;
    range.x = min;
    range.y = max;
  }
  
  public bool add(int step)
  {
    qty += step;

    //Debug.Log(uid + " x " + qty);

    if (range.y > -1 && qty > range.y)
    {
      qty = (int)range.y;
      return true;
    }

    return false;
  }

  public bool remove(int step)
  {
    qty -= step;
    if(qty < range.x)
    {
      qty = (int)range.x;
      return true;
    }
    return false;
  }

  public void setQuantity(int newQty){ qty = newQty; }
  public int getQuantity() { return qty; }
  public string getId() { return uid; }

  public bool isItem(string nm)
  {
    if (uid.Contains(nm)) return true;
    return false;
  }
}
