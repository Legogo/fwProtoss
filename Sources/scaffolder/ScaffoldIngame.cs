using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scaffolder
{
  public class ScaffoldIngame : ScaffGround
  {
    static public ScaffoldIngame mgr;
    List<iScaffIngame> scaffers = new List<iScaffIngame>();

    public enum IngameState { OFF, PRIMED, LIVE, END };
    IngameState state;

    protected override void build()
    {
      base.build();
      mgr = this;
    }

    public void sub(iScaffIngame elmt)
    {
      if (scaffers.IndexOf(elmt) < 0) scaffers.Add(elmt);
    }
    public void unsub(iScaffIngame elmt)
    {
      scaffers.Remove(elmt);
    }

    void Update()
    {
      if (state != IngameState.LIVE) return;

      for (int i = 0; i < scaffers.Count; i++)
      {
        scaffers[i].ingameUpdate();
      }
      for (int i = 0; i < scaffers.Count; i++)
      {
        scaffers[i].ingameUpdateLate();
      }
    }

    public void roundSetup()
    {
      for (int i = 0; i < scaffers.Count; i++)
      {
        scaffers[i].ingameSetup();
      }

      state = IngameState.PRIMED;
    }

    public void roundRestart()
    {
      for (int i = 0; i < scaffers.Count; i++)
      {
        scaffers[i].ingameRestart();
      }

      state = IngameState.LIVE;
    }

    public void roundEnd()
    {
      state = IngameState.END;

      for (int i = 0; i < scaffers.Count; i++)
      {
        scaffers[i].ingameEnd();
      }
    }
  }

}
