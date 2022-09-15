using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class ArenaMgr
{

}

public interface iArenaCandidate
{
  void setup();
  void restart();
  void update();
  void end();
}
