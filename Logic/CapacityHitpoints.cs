﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapacityHitpoints : LogicCapacity {

  public float startingHealth = 1f; // [0,1]
  public float health = 1f;
  
  public override void setupCapacity()
  {
    health = startingHealth;
  }
  public override void updateLogic()
  {
  }

  public float getHealth()
  {
    return health;
  }

  public void takeHit(float pwr)
  {
    health -= pwr;
    if (health < 0f) health = 0f;

    Debug.Log(_owner.name + " took <b>"+pwr+" dmg</b> and now has <b>" + health + " hp</b>");
    
  }

  public override void clean()
  {
  }
}
