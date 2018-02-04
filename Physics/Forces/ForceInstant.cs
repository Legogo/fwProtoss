using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceInstant : ForceBase {
  
  public ForceInstant(string forceName, Vector2 strength):base(forceName, true)
  {
    _force = strength;
  }
  public ForceInstant(string forceName, float strengthX, float strengthY):base(forceName, true)
  {
    _force.x = strengthX;
    _force.y = strengthY;

    //Debug.Log("ForceInstant "+forceName + " -> x " + _force.x + " , y " + _force.y);
  }

  protected override void compute()
  {
    //...
  }
}
