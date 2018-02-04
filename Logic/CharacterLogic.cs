using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Classe générique pour gérer le déplacement et l'animation de persos
/// </summary>

public class CharacterLogic : LogicItem {

  public enum LogicGameType { PLATFORMER, TOP_VIEW };

  [Tooltip("to know how to compute orientation (top view or side view)")]
  public LogicGameType gameType;

  protected Animator _anim;

  protected CapacityCollision _collision;
  protected CapacityMovement _move;
  
  private Transform _spawnReference;

  protected CapacityHitpoints _hp;

  protected string overrideNameAnimation = "";
  
  protected override void build()
  {
    base.build();

    _collision = GetComponent<CapacityCollision>();
    //on peut vouloir faire des trucs sans collision
    //if (_collision == null) Debug.LogError("no collision capacity for : " + name, gameObject);

    _move = GetComponent<CapacityMovement>();
    if (_move == null) Debug.LogError(name + " has no <b>move module</b> for character logic ?", gameObject);

  }

  protected override void setup()
  {
    base.setup();

    //Debug.Log(name + " <b>fetch</b> global", gameObject);

    _anim = gameObject.GetComponentInChildren<Animator>();
    
    _hp = gameObject.GetComponent<CapacityHitpoints>();
  }

  virtual public void setupBalancing(){}

  /* anim won't play something else */
  public void captureAnim(string newAnimationName)
  {
    overrideNameAnimation = newAnimationName;
    //_anim.Play(overrideNameAnimation);
  }

  public void releaseAnim()
  {
    overrideNameAnimation = "";
  }

  public override void updateEngine()
  {
    base.updateEngine();

    update_animation();

    ComputeOrientation(_move.getHorizontalDirection());
  }

  virtual protected void update_animation()
  {
    if(overrideNameAnimation.Length > 0)
    {
      PlayAnimOfName(overrideNameAnimation);
      return;
    }

    if (!_move.isGrounded())
    {
      PlayAnimOfName("fall");
    }
    else
    {
      if (_move.hasMoved())
      {
        PlayAnimOfName("walk");
      }
      else
      {
        PlayAnimOfName("idle");
      }
    }

  }

  public void PlayAnimOfName(string animName)
  {
    if (_anim == null) return;
    //Debug.Log(animName);
    _anim.Play(animName);
  }

  public bool isPlaying(string animName)
  {
    if (_anim == null) return false;
    AnimatorStateInfo state = _anim.GetCurrentAnimatorStateInfo(0);
    if (state.IsName(animName)) return true;
    return false;
  }

  virtual public void Respawn(Transform spawn = null)
  {
    if (_hp != null) _hp.setupCapacity();

    if(_move != null) _move.clean();
    
    if (spawn != null) _spawnReference = spawn;

    if(_spawnReference != null)
    {
      transform.position = _spawnReference.transform.position;
    }
    
    show();
  }

  public float getHp() { if (_hp == null) return 1f; return _hp.getHealth(); }

  public void show()
  {
    visibility.show();
    if (_collision == null) Debug.LogError(name + " has no collision ?", gameObject);
    _collision.enabled = true;
  }

  public void hide()
  {
    visibility.hide();
    _collision.enabled = false;
  }

  public Sprite getSprite()
  {
    return (visibility as HelperVisibleSprite).getSprite();
  }

  protected void ComputeOrientation(int hDirection)
  {
    //why invert ?
    //int sign = (hDirection >= 0) ? -1 : 1;
    
    switch(gameType)
    {
      case LogicGameType.TOP_VIEW:
        float angle = Vector2.Angle(Vector2.right * hDirection, Vector2.up) * (hDirection * -1);
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        break;
      case LogicGameType.PLATFORMER:
        //GetComponentInChildren<SpriteRenderer>().flipX = sign == 1f;
        visibility.flipHorizontal(hDirection);
        break;
    }

    
  }
  
  /* it's interesting to have a hub that avoid to get the Movement capa every time */
  public int getDirection()
  {
    return _move.getHorizontalDirection();
  }

  public override string toString()
  {
    string ct = base.toString();
    ct += "\n~CharacterLogic~";
    ct += "\n  direction ? " + getDirection();
    return ct;
  }
}