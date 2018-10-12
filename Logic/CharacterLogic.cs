using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Classe générique pour gérer le déplacement et l'animation de persos
/// </summary>

namespace fwp
{
  public class CharacterLogic : LogicItem
  {

    public enum LogicGameType { PLATFORMER, TOP_VIEW };

    [Tooltip("to know how to compute orientation (top view or side view)")]
    public LogicGameType gameType;

    //protected Animator _animator;
    public AnimatorPlayer animController;

    protected CapacityCollision _collision;
    protected CapacityMovement _move;

    private Transform _spawnReference;

    protected CapacityHitpoints _hitPoints;

    protected string overrideNameAnimation = "";

    protected override void build()
    {
      base.build();

      _collision = GetComponent<CapacityCollision>();
      //on peut vouloir faire des trucs sans collision
      //if (_collision == null) Debug.LogError("no collision capacity for : " + name, gameObject);

      _move = GetComponent<CapacityMovement>();
      //if (_move == null) Debug.LogError(name + " has no <b>move module</b> for character logic ?", gameObject);

      animController = new AnimatorPlayer(transform);
      animController.onAnimEnd += onAnimationDone;
    }

    protected override void setup()
    {
      base.setup();

      //Debug.Log(name + " <b>fetch</b> global", gameObject);

      _hitPoints = gameObject.GetComponent<CapacityHitpoints>();
    }

    virtual public void setupBalancing() { }

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

    protected override void updateArenaLive(float timeStamp)
    {
      base.updateArenaLive(timeStamp);

      update_animation();

      if (_move != null) ComputeOrientation(_move.getHorizontalDirection());
    }

    virtual protected void update_animation()
    {
      animController.update_check(); // to activate event of progression
                                     //_controller.update(); // make animation go foward

      string animToPlay = "idle";

      if (overrideNameAnimation.Length > 0)
      {
        animToPlay = overrideNameAnimation;
      }
      else if (_move != null)
      {

        if (!_move.isGrounded())
        {
          animToPlay = "fall";
        }
        else
        {
          if (_move.hasMoved())
          {
            animToPlay = "walk";
          }
        }

      }

      PlayAnimOfName(animToPlay);
    }

    protected void onAnimationDone()
    {
      //Debug.Log("end of character animation");
      if (overrideNameAnimation.Length > 0) overrideNameAnimation = "";
    }

    protected void PlayAnimOfName(string stateName)
    {
      //already playing this anim ?
      if (animController.isPlaying(stateName))
      {
        //Debug.LogWarning("already playing : " + animName);
        return;
      }

      //Debug.Log("playing : " + animName);
      animController.launch(stateName);
    }

    virtual public void Respawn(Transform spawn = null)
    {
      if (_hitPoints != null) _hitPoints.setupCapacity();

      if (_move != null) _move.clean();

      if (spawn != null) _spawnReference = spawn;

      if (_spawnReference != null)
      {
        transform.position = _spawnReference.transform.position;
      }

      show();
    }

    public float getHp() { if (_hitPoints == null) return 1f; return _hitPoints.getHealth(); }

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

      switch (gameType)
      {
        case LogicGameType.TOP_VIEW:
          float angle = Vector2.Angle(Vector2.right * hDirection, Vector2.up) * (hDirection * -1);
          transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
          break;
        case LogicGameType.PLATFORMER:
          //GetComponentInChildren<SpriteRenderer>().flipX = sign == 1f;
          visibility.flipHorizontal(hDirection);
          //Debug.Log(hDirection);
          break;
      }

    }

    /* it's interesting to have a hub that avoid to get the Movement capa every time */
    public int getDirection()
    {
      if (_move == null) return 0;
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
}
