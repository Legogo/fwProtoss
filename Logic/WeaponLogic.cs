﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A weapon must be carried by a CharacterLogic but can have it's own Capacities
/// </summary>

abstract public class WeaponLogic : LogicItem {

  protected Collider2D[] _colliders;
  protected CharacterLogic _weaponOwner;
  protected CapacityAttack _attack;

  protected override void build()
  {
    base.build();

    List<Collider2D> tmp = new List<Collider2D>();
    tmp.AddRange(GetComponents<Collider2D>());
    tmp.AddRange(GetComponentsInChildren<Collider2D>());
    _colliders = tmp.ToArray();

    _weaponOwner = GetComponentInParent<CharacterLogic>();
    _attack = _weaponOwner.GetComponent<CapacityAttack>();

    getCollider().enabled = false;
  }
  
  public int getOwnerDirection()
  {
    return _weaponOwner.Direction;
  }
  
  public bool isAttacking()
  {
    return _attack.isAttacking();
  }

  public Collider2D getCollider()
  {
    return _colliders[0];
  }
  
  /* helper to grab weapon */
  static public T getWeaponByType<T>(Transform parent) where T : WeaponLogic
  {
    WeaponLogic[] weapons = parent.GetComponentsInChildren<WeaponLogic>();
    foreach (WeaponLogic w in weapons)
    {
      if (w.GetType() == typeof(T)) return w as T;
    }
    return null;
  }

}
