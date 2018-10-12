using System.Collections;
using System;
using UnityEngine;

namespace fwp.input
{
  public class InputTouchPinch
  {

    float delta; // last move spread delta
    float magnitude; // spread done by user (relative to original spread)

    float total;

    float rawSpread; // actual spread on screen btw the 2 fingers (absolute)
    float originalMagnitude; // local var to solve magnitude

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
      bool pinching = isPinching();

      if (finger == _fingerA) _fingerA = null;
      if (finger == _fingerB) _fingerB = null;

      if (pinching && !isPinching())
      {
        total = getMagnitude();

        reset();
      }
    }

    public void onPress(InputTouchFinger finger)
    {
      if (_fingerA == null)
      {
        _fingerA = finger;
        return;
      }

      if (_fingerB == null)
      {
        _fingerB = finger;
      }

      rawSpread = getDistanceBetweenFingers();
      originalMagnitude = rawSpread;
      magnitude = 0f;
      delta = 0f;
    }

    public void reset()
    {
      magnitude = 0f;
      delta = 0f;
      rawSpread = 0f;
    }

    public void updateDesktop()
    {
      delta = Input.mouseScrollDelta.y * iBridge.mouseScrollMulFactor;
      magnitude += delta;

      magnitude = Mathf.Clamp(magnitude, iBridge.scrollClampMagnitude.x, iBridge.scrollClampMagnitude.y);

      if (onScroll != null) onScroll(delta, magnitude);
    }

    void updateMobile()
    {
      if (!isPinching()) return;

      float newSpread = getDistanceBetweenFingers();
      delta = newSpread - rawSpread;

      magnitude = rawSpread - originalMagnitude;
      rawSpread = newSpread;

      if (onScroll != null) onScroll(delta, getMagnitude());
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
      float output = total + magnitude;

      Vector2 clamp = iBridge.scrollClampMagnitude;
      if (clamp.sqrMagnitude != 0f)
      {
        output = Mathf.Clamp(output, clamp.x, clamp.y);
      }

      if (!InputTouchBridge.isMobile()) output = Math.Max(output, 0f);

      return output;
    }

    public bool isPinching()
    {
      if (_fingerA == null || _fingerB == null) return false;
      return true;
    }

    public string toString()
    {
      string ct = "[PINCH]";

      ct += "\n A : " + ((_fingerA != null) ? _fingerA.fingerId.ToString() : "none");
      ct += "\n B : " + ((_fingerB != null) ? _fingerB.fingerId.ToString() : "none");

      ct += "\n dlta   : " + delta;
      ct += "\n magn   : " + magnitude;
      ct += "\n tota   : " + total;
      ct += "\n orig   : " + originalMagnitude;
      return ct;
    }
  }
}