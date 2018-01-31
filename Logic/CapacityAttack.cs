using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

abstract public class CapacityAttack : LogicCapacity
{
  public BoxCollider2D attackCollider; // collider use for overlap comparison
  
  protected WeaponLogic _weapon;
  
  protected Coroutine coProcessAttack;
  protected CapacityInput _input;
  protected CapacityMovement _move;

  protected CapacityHittable _hittable;
  private CapacityHittable[] _otherHittables;

  private float _attackTime;
  private float _interruptTime = 0f;
  private InputKeyTopDown inputDirection;

  public bool debug_autoHit = false;
  
  /* describe where and what type of weapon to find */
  virtual protected void fetchWeapon()
  {
    _weapon = WeaponLogic.getWeaponByType<WeaponLogic>(transform);
  }

  public override void setupCapacity()
  {
    inputDirection = _character.GetComponent<CapacityInput>().keys.get<InputKeyTopDown>();
    fetchWeapon();

    if (_weapon == null)
    {
      Debug.LogError("weapon is null, capacity attack is meant to use a weapon");
      return;
    }

    attackCollider = _weapon.getCollider() as BoxCollider2D;
    if(attackCollider == null)
    {
      Debug.LogError("no collider for weapon " + _weapon.name + " ?", gameObject);
      return;
    }

    attackCollider.enabled = false;

    _hittable = _owner.GetComponent<CapacityHittable>();
    refreshHittables();

    _interruptTime = getInterruptTime();
  }

  abstract public float getInterruptTime();

  protected void refreshHittables()
  {
    //filter own hittable from list
    List<CapacityHittable> tmp = new List<CapacityHittable>();
    _otherHittables = GameObject.FindObjectsOfType<CapacityHittable>();
    foreach(CapacityHittable hit in _otherHittables)
    {
      if (hit != _hittable) tmp.Add(hit);
    }
    _otherHittables = tmp.ToArray();
  }
  
  public override void updateLogic()
  {
    base.updateLogic();
    if (debug_autoHit) Attack();
  }
  
  internal void CancelAttack()
  {
    processEndOfAttach();
  }

  public void Attack()
  {
    if (coProcessAttack != null) return; // already attacking
    coProcessAttack = StartCoroutine(ProcessAttack());
  }

  IEnumerator ProcessAttack()
  {
    _attackTime = 0f;
    
    string animToPlay = "player_attack";

    _character.CaptureAnim();

    if (inputDirection.pressing_up())
    {
      animToPlay += "_up";
    }
    else if (inputDirection.pressing_down())
    {
      animToPlay += "_down";
    }
    else
    {
      animToPlay += "_stand";
    }

    //start animation
    _character.PlayAnimOfName(animToPlay);
    //Vector3 scale = _player.transform.localScale;

    //wait for anim to start in <animator>
    yield return null;
    
    // check if the anim is still playing and if the attack has not hit
    while (_character.isPlaying(animToPlay))
    {
      checkForAttackEvent();
      yield return null;
    }

    processEndOfAttach();
  }

  protected void processEndOfAttach()
  {
    if (coProcessAttack != null) StopCoroutine(coProcessAttack);

    _character.ReleaseAnim();

    coProcessAttack = null;
    _attackTime = 0f;
    attackCollider.enabled = false;
  }

  /* called each frame while attacking coroutine is running */
  private void checkForAttackEvent()
  {
    //if (overlappingColliders == null) overlappingColliders = new Collider2D[10];
    //attackCollider.OverlapCollider(new ContactFilter2D(), overlappingColliders);

    if (isFreezed()) Debug.LogWarning("check for attack event on a freezed logic ??");

    CapacityHittable capaTarget;

    for (int i = 0; i < _otherHittables.Length; i++)
    {
      if (!_otherHittables[i].Hittable()) continue; // deactivated

      //this should not happen, owner is filtered when populating other list
      //if (_otherHittables[i] == _hittable) continue; // me

      //check si je touche l'element
      capaTarget = _otherHittables[i].checkHitSomething(this);

      //touch !
      if (capaTarget != null)
      {
        capaTarget.HitBySomething(this);
      }

    }
    
  }

  public bool CanCancelAttack()
  {
    return AttackTime > _interruptTime;
  }

  /* must describe waht happens when hit */
  abstract public void HitBySomething(CapacityAttack attacker);
  
  protected void doRecoil(Vector2 recoilPower)
  {
    _move.addForce(new ForceInstant("recoil",
      _move.getHorizontalDirection() * recoilPower.x, recoilPower.y));

    CancelAttack();
  }
  public WeaponLogic getWeapon() { return _weapon; }
  
  public bool isAttacking()
  {
    return coProcessAttack != null;
  }

  public float AttackTime
  {
    get { return _attackTime; }
  }

  public bool Hittable()
  {
    if (getOwner().isFreezed()) return false;
    return attackCollider.enabled;
  }

  public int getAttackDirection()
  {
    if (!isAttacking()) return 0;
    return _character.Direction;
  }

  public PlayerLogic getPlayerLogic() { return _owner as PlayerLogic; }
  
  public override void clean()
  {
  }
}
