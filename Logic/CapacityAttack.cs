using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

abstract public class CapacityAttack : LogicCapacity
{
  protected WeaponLogic _weapon;
  
  protected Coroutine coProcessAttack;
  protected CapacityMovement _move;

  protected CapacityHittable _hittable;
  private CapacityHittable[] _otherHittables;

  private float _attackTime;
  private float _interruptTime = 0f;
  
  public bool debug_autoHit = false;
  
  /* describe where and what type of weapon to find */
  virtual protected void fetchWeapon()
  {
    _weapon = WeaponLogic.getWeaponByType<WeaponLogic>(transform);
  }

  public override void setupCapacity()
  {
    
    fetchWeapon();

    if (_weapon == null)
    {
      Debug.LogError("weapon is null, capacity attack is meant to use a weapon");
      return;
    }

    //_weapon.toggleCollider(false);

    _hittable = _owner.GetComponent<CapacityHittable>();
    refreshHittables();

    _move = _owner.GetComponent<CapacityMovement>();

    _interruptTime = getInterruptTime();
  }

  abstract public float getInterruptTime();

  abstract protected bool pressedAttack();
  abstract protected bool releasedAttack();

  abstract protected string getAttackDirectionName(); // up,down,stand
  
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
  
  public override void updateCapacity()
  {
    base.updateCapacity();

    if (debug_autoHit)
    {
      Attack();
      return;
    }

    //Debug.Log("attack ? "+inputAttack);
    if (pressedAttack()) Attack();
  }
  
  internal void CancelAttack()
  {
    endOfAttack();
  }

  public void Attack()
  {
    refreshHittables();
    if (coProcessAttack != null) return; // already attacking
    onAttackStart();
  }

  /// <summary>
  /// allow children to do more stuff when attack starts
  /// </summary>
  virtual protected void onAttackStart()
  {
    coProcessAttack = StartCoroutine(ProcessAttack());
  }

  /// <summary>
  /// process qui décrit le déroulement de l'attaque
  /// </summary>
  IEnumerator ProcessAttack()
  {
    _attackTime = 0f;
    
    string animToPlay = "player_attack_";

    animToPlay += getAttackDirectionName(); //composition du nom de l'anim
    
    _character.captureAnim(animToPlay); // tell character to play something else (attack anim)
    
    //wait for anim to start in <animator>
    yield return null;

    _move.lockHorizontal.addLock(gameObject);
    _move.addVelocity(getHorizontalSpeedOnLock(), 0f);

    //lock anim at frame 1 when keeping attack pressed
    //wait for release
    if (!releasedAttack())
    {
      _character.animController.pause(); // pause
        
      while (!releasedAttack())
      {
        //_character.stop
        yield return null;
      }
      
      _character.animController.resume();

    }

    //Debug.Log("start of animation attack "+animToPlay);

    //_character.animController.onAnimEnd += ownerAnimationDone;

    //this will be cancelled when character parent will finish it's animation
    while (_character.animController.isPlaying(animToPlay))
    {
      checksDuringAnimation();
      yield return null;
    }

    endOfAttack();
  }

  virtual protected float getHorizontalSpeedOnLock()
  {
    return 0f;
  }

  protected void ownerAnimationDone()
  {
    endOfAttack();
  }

  protected void endOfAttack() {

    StopAllCoroutines();

    _move.lockHorizontal.removeLock(gameObject);

    //_character.animController.onAnimEnd -= ownerAnimationDone;
    
    _character.releaseAnim();

    coProcessAttack = null;
    _attackTime = 0f;

    //_weapon.toggleCollider(false);
  }

  virtual protected void checksDuringAnimation()
  {
    checkForAttackEvent();
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
    //Debug.Log(name + " do recoil");
    _move.addVelocity(-_move.getHorizontalDirection() * recoilPower.x, recoilPower.y);
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
    return _weapon.getMainCollider().enabled;
  }

  public int getAttackDirection()
  {
    if (!isAttacking()) return 0;
    return _move.getHorizontalDirection();
  }

  public override void clean()
  {
  }
}
