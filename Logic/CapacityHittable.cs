using UnityEngine;
using System;

abstract public class CapacityHittable : LogicCapacity
{
  protected Collider2D[] _colliders;

  public float hitTimer = 0f;
  public float invincibleTimer = 0f;
  
  protected CapacityAttack _attack;
  protected CapacityMovement _move;
  protected CapacityHitpoints _hp;

  public Action<LogicItem, LogicItem> onHit;

  public override void setupCapacity()
  {
    _colliders = UnityHelpers.getColliders2D(transform);
    
    _move = _owner.GetComponent<CapacityMovement>();

    _attack = _owner.GetComponent<CapacityAttack>();
    
    _hp = _owner.GetComponent<CapacityHitpoints>();
  }

  public override void updateCapacity()
  {
    
    // transform.localScale = new Vector3(1f, hitTimer > 0f ? 0.5f : 1f);

    if(hitTimer > 0f)
    {
      hitTimer -= Time.deltaTime;
    }

    if (invincibleTimer > 0f)
    {
      invincibleTimer -= Time.deltaTime;
    }

  }

  protected bool overlap(BoxCollider2D boxa, BoxCollider2D boxb)
  {
    if(boxa.OverlapPoint(boxb.bounds.center)) return true;
    //if (boxa.OverlapPoint(boxb.bounds.center)) return true;
    return false;
  }

  public CapacityHittable checkHitSomething(CapacityAttack attackerCapa)
  {
    if (checkHitSword(attackerCapa)) return null;

    //faire le hit player APRES toutes les armes
    if (checkHitPlayer(attackerCapa)) return this;

    return null;
  }
  
  private bool checkHitPlayer(CapacityAttack attackerCapa)
  {
    //si le collider de l'épée de l'attacker overlap pas avec mon collider (corps)
    if (!overlap(getCollider(), attackerCapa.getWeapon().getMainCollider())) return false;

    Debug.Log(attackerCapa.getOwner().name + " --ATTACK--> " + _owner.name);
    
    return true;
  }

  private bool checkHitSword(CapacityAttack attackerCapa)
  {
    //si le collider de l'épée de l'adversaire overlap pas avec la mienne
    if (!overlap(_attack.getWeapon().getMainCollider(), attackerCapa.getWeapon().getMainCollider())) return false;
    
    Debug.Log(_owner.name + " <--WEAPONS--> " + attackerCapa.getOwner().name);

    //balance l'event sur l'attacker qu'il a tapé une épée
    attackerCapa.HitBySomething(_attack);
    _attack.HitBySomething(attackerCapa);
    return true;
  }

  public void doKnockBack(Vector2 knockbackPower, float hitDirection)
  {
    //Debug.Log(name + " knockback");
    _move.addVelocity(hitDirection * knockbackPower.x, knockbackPower.y);
  }

  public bool Hittable()
  {
    if (getOwner().isFreezed()) return false;
    if (invincibleTimer > 0f) return false;
    if (!getCollider().enabled) return false;
    return true;
  }

  abstract public void HitBySomething(CapacityAttack attacker);
  
  public bool Stuned
  {
    get
    {
      return hitTimer > 0f;
    }
  }
  
  /* main collider if multiple ones */
  public BoxCollider2D getCollider()
  {
    return _colliders[0] as BoxCollider2D;
  }

  public override void clean()
  {
  }
}
