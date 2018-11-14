using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDeviceInfo : DebugDisplayContent {

  protected override void setupFont()
  {
    base.setupFont();
    style.normal.textColor = Color.red;
  }

  protected override void processGui()
  {
    ct = Screen.width + " x " + Screen.height;
  }

}
