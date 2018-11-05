using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDeviceInfo : DebugDisplayContent {

  protected override void setupFont()
  {
    base.setupFont();
    style.normal.textColor = Color.red;
  }

  protected override void process()
  {
    ct = Screen.width + " x " + Screen.height;
  }

}
