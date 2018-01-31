using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Common base class for all InputKeyManager bridge logic
/// 
/// This need fwWaffleInputController library
/// 
/// </summary>

abstract public class InputKey {

  protected int _playerIndex;
  protected Controller360 _xboxController;
  protected XinputController _xinput;
  
  virtual public void assignIndex(int newIndex)
  {
    _playerIndex = newIndex;
    _xboxController = ControllerSelector.getInputManager().getControllerById(_playerIndex) as Controller360;
    _xinput = ControllerSelector.getInputManager().getControllerById(_playerIndex) as XinputController;
  }

  public Controller360 getController() { return _xboxController; }
  public XinputController getXinput() { return _xinput; }
}
