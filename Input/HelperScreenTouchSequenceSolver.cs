﻿using UnityEngine;
using System;

/// <summary>
/// 2018-10-05
/// this is meant to simplify making a sequence of click on screen
/// </summary>

namespace fwp.input
{
  public class HelperScreenTouchSequenceSolver
  {

    Transform parent;

    Rect[] zones;
    int step = 0;

    bool _proportional = false;
    bool _state = false;

    public Action onToggle; // callback where to subscribe to react to sequence solving

    public HelperScreenTouchSequenceSolver(Transform owner, Rect[] screenZones, bool useProportional = false)
    {
      parent = owner;
      zones = screenZones;

      _proportional = useProportional;

      InputTouchBridge.get().onTouch += onBridgeInput;
    }

    protected void onBridgeInput(InputTouchFinger finger)
    {
      onInput(_proportional ? finger.screenProportional : (Vector2)finger.screenPosition);
    }

    protected void onInput(Vector2 screenPosition)
    {
      if (!parent.gameObject.activeSelf) return;

      Rect z = zones[step];

      if (screenPosition.x > z.x && screenPosition.x < z.x + z.width)
      {
        if (screenPosition.y > z.y && screenPosition.y < z.y + z.height)
        {
          step++;

          //Debug.Log(step);

          if (step >= zones.Length)
          {
            toggle();
          }
          return;
        }
      }

      step = 0;
    }

    protected void toggle()
    {
      _state = !_state;
      step = 0;

      if (onToggle != null) onToggle();

      //Debug.Log(parent.name + " toggle");
    }

    public bool isToggled()
    {
      return _state;
    }
  }
}