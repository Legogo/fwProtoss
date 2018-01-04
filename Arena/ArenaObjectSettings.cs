using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaObjectSettings {

  protected Transform tr;
  protected Vector3 _init_position;
  protected Quaternion _init_rotation;

  public ArenaObjectSettings(Transform owner)
  {
    tr = owner;

    //learn where the object is at start
    recRespawn();
  }

  protected void recRespawn()
  {  
    _init_position = tr.localPosition;
    _init_rotation = tr.localRotation;
  }

  public void applyRespawn()
  {
    tr.localPosition = _init_position;
    tr.localRotation = _init_rotation;
  }

}
