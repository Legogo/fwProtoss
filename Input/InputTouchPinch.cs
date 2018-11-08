using System.Collections;
using System;
using UnityEngine;

namespace fwp.input
{
  public class InputTouchPinch
  {

    float delta; // last move spread delta
    float magnitude; // spread done by user (relative to original spread)
    
    float lastRawSpread; // actual spread on screen btw the 2 fingers (absolute)
    float onPressSpread; // local var to solve magnitude

    InputTouchFinger _fingerA;
    InputTouchFinger _fingerB;

    public Action<float, float> onScroll;

    InputTouchBridge iBridge;

    public InputTouchPinch(InputTouchBridge itb)
    {
      iBridge = itb;

      if (InputTouchBridge.isMobile())
      {
        itb.onTouch += onPress;
        itb.onRelease += onRelease;
        itb.onTouching += updateMobile;
      }

      reset();
    }

    public void onRelease(InputTouchFinger finger)
    {
      
      if (finger == _fingerA) _fingerA = null;
      if (finger == _fingerB) _fingerB = null;

      /*
      bool pinching = isPinching();

      if (pinching && !isPinching())
      {
        reset();
      }
      */
    }

    public void onPress(InputTouchFinger finger)
    {
      if (_fingerA == null)
      {
        _fingerA = finger;
        return;
      }

      if (finger == _fingerA) return;

      if (_fingerB == null)
      {
        _fingerB = finger;
      }
      
      lastRawSpread = getDistanceBetweenFingers(); //current spread is neutral
    }
    
    public void reset()
    {
      lastRawSpread = 0f;
      delta = 0f;
    }

    protected float getDelta(BehaviorTargetPlatform platform)
    {
      float output = 0f;

      if (!isPinching()) return output;

      if (platform == BehaviorTargetPlatform.DESKTOP)
      {
        output = Input.mouseScrollDelta.y * iBridge.mouseScrollMulFactor;
      }
      else
      {
        float newSpread = getDistanceBetweenFingers();
        output = newSpread - lastRawSpread;
        lastRawSpread = newSpread;
      }
      
      return output;
    }

    public void updateMobile() { update(BehaviorTargetPlatform.MOBILE); }

    public void update(BehaviorTargetPlatform platform)
    {
      if (!isPinching()) return;

      delta = getDelta(platform);

      //Debug.Log(delta);

      if (delta == 0f) return;

      magnitude += delta;
      magnitude = Mathf.Clamp(magnitude, iBridge.scrollClampMagnitude.x, iBridge.scrollClampMagnitude.y);

      if (onScroll != null) onScroll(delta, magnitude);
    }
    
    /// <summary>
    /// distance btw the 2 fingers (screen positions)
    /// </summary>
    /// <returns></returns>
    public float getDistanceBetweenFingers()
    {
      return Vector2.Distance(_fingerB.screenPosition, _fingerA.screenPosition);
    }

    public bool atMaxMagnitude()
    {
      if (iBridge.scrollClampMagnitude.sqrMagnitude == 0f) return false;
      return magnitude >= iBridge.scrollClampMagnitude.y;
    }

    public float getDelta()
    {
      return delta;
    }

    public float getMagnitude()
    {
      return magnitude;
    }

    public bool isPinching()
    {
      if (!InputTouchBridge.isMobile()) return true;

      if (_fingerA == null || _fingerB == null) return false;
      return true;
    }

    public string toString()
    {
      string ct = "[PINCH]";

      ct += "\n A : " + ((_fingerA != null) ? _fingerA.fingerId.ToString() : "none");
      ct += "\n B : " + ((_fingerB != null) ? _fingerB.fingerId.ToString() : "none");
      
      ct += "\n dlta      : " + delta;
      ct += "\n magn      : " + magnitude;
      ct += "\n getMagn() : " + getMagnitude();
      return ct;
    }
  }
}