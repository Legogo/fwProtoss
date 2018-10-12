using UnityEngine;
using System;
using fwp;

namespace fwp
{

  public class CapacityHittable : LogicCapacity
  {
    // arg: attacker
    public event Action<LogicCapacity> HitEvent;

    protected Collider2D[] _colliders;

    public float hitTimer = 0f;
    public float invincibleTimer = 0f;

    public Action<LogicItem, LogicItem> onHit;

    public override void setupCapacity()
    {
      _colliders = HalperPhysic.getColliders2D(transform);
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
      if(boxa.bounds.Intersects(boxb.bounds)) return true;
      //if (boxa.OverlapPoint(boxb.bounds.center)) return true;
      return false;
    }

    protected bool overlap(BoxCollider2D box, Vector3 bounds)
    {
      if (box.OverlapPoint(bounds)) return true;
      return false;
    }

    public virtual CapacityHittable checkHitSomething(CapacityAttack attackerCapa)
    {
      //faire le hit player APRES toutes les armes
      if (checkHitPlayer(attackerCapa))
      {
        return this;
      }

      return null;
    }
  
    private bool checkHitPlayer(CapacityAttack attackerCapa)
    {
      //si le collider de l'épée de l'attacker overlap pas avec mon collider (corps)
      if (!overlap(getCollider(), attackerCapa.getWeapon().getMainCollider())) return false;

      Debug.Log(attackerCapa.getOwner().name + " --ATTACK--> " + _owner.name);
    
      return true;
    }

    public bool Hittable()
    {
      if (this == null) return false;
      if (getOwner().isFreezed()) return false;
      if (invincibleTimer > 0f) return false;
      if (getCollider() != null && !getCollider().enabled) return false;
      return true;
    }

    public virtual void HitBySomething(CapacityAttack attacker)
    {
      RaiseHitEvent(attacker);
    }
  
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

    public Vector3 getBounds()
    {
      return getCollider().bounds.center;
    }

    public override void clean()
    {
    }

    protected virtual void RaiseHitEvent(LogicCapacity item)
    {
      Action<LogicCapacity> handler = HitEvent;
      if (handler != null)
        handler(item);
    }
  }
  
}