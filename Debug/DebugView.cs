using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fwp.input;

public class DebugView : DebugDisplayContent {

  HelperScreenTouchSequenceSolver solver;

  public Rect[] zones;

  protected override void setup()
  {
    base.setup();

    solver = new HelperScreenTouchSequenceSolver(zones);

    solver.onToggle += delegate()
    {
      enabled = !enabled;
    };
  }
  
}
