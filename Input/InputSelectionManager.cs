using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fwp.input;

namespace fwp.input
{
  public class InputSelectionManager
  {
    static public InputSelectionManager manager;

    List<HelperInputObject> selection = new List<HelperInputObject>();
    
    public InputSelectionManager()
    {
      manager = this;
    }

    public void add(EngineObject obj)
    {
      HelperInputObject io = obj.getIO();
      if (io == null)
      {
        Debug.LogWarning("trying to add object input of : '" + obj.name + "' but it doesn't have IO");
        return;
      }

      selection.Add(io);

      //Debug.Log("added : " + io.owner.name);
    }

    public void remove(EngineObject obj)
    {
      HelperInputObject io = obj.getIO();
      if (io == null) return;

      if (selection.IndexOf(io) < 0) return;
     
      selection.Remove(io);
      //Debug.Log("removed : " + io.owner.name);
    }

    public EngineObject getSelection()
    {
      if (selection.Count <= 0) return null;
      if (selection[0] == null) return null;

      return selection[0].owner;
    }

    public bool hasSelection()
    {
      return selection.Count > 0;
    }
    
    public string toString()
    {
      string ct = "[SELECTION]";
      for (int i = 0; i < selection.Count; i++)
      {
        ct += "\n   L #" + i + " -> " + selection[i].owner.name;
      }
      return ct;
    }
  }

}
