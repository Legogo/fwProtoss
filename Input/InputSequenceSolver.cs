using UnityEngine;
using System;

public class InputSequenceSolver {

  Transform parent;

  Rect[] zones;
  int step = 0;

  bool _state = false;

  public Action onToggle;

  public InputSequenceSolver(Transform owner, Rect[] screenZones)
  {
    parent = owner;
    zones = screenZones;
  }

  public void onInput(Vector2 screenPosition)
  {
    if (!parent.gameObject.activeSelf) return;

    Rect z = zones[step];

    if (screenPosition.x > z.x && screenPosition.x < z.x + z.width)
    {
      if (screenPosition.y > z.y && screenPosition.y < z.y + z.height)
      {
        step++;

        //Debug.Log(step);

        if(step >= zones.Length)
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

    if(onToggle != null) onToggle();

    //Debug.Log(parent.name + " toggle");
  }

  public bool isToggled()
  {
    return _state;
  }
}
