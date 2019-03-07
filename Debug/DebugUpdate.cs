using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class DebugUpdate : ArenaObject {
  
  public override void updateEngine()
  {
    base.updateEngine();

#if debug
    doStuff();
#endif

  }
  
  abstract public void doStuff();
}
