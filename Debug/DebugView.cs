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

    solver = new HelperScreenTouchSequenceSolver(transform, zones, true);

    solver.onToggle += toggle;
  }

  protected void toggle()
  {
    enabled = !enabled;
  }
}
