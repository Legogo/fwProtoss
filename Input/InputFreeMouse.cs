using UnityEngine;
using System.Collections;

public class InputFreeMouse
{

	static public void lockMouse(bool lockState)
	{
    bool changed = false;

    if (!Cursor.visible && !lockState) {
			Cursor.lockState = CursorLockMode.None;
      Cursor.visible = true;
      changed = true;
      //Debug.Log("<InputFreeMouse>\tswitched to none");
		} else if(Cursor.visible && lockState){
      //Cursor.lockState = CursorLockMode.Confined;
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
      changed = true;
      //Debug.Log("<InputFreeMouse>\tswitched to locked");
    }

    if(changed) {
      Debug.Log("<InputFreeMouse> is now locked ? " + lockState + " (cursor.lockState = " + Cursor.lockState + ", Cursor.visible = " + Cursor.visible + ")");
    }
  }

}
