using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArenaManager : EngineObject {

  public Text txt_score;
  public int score;

  public bool freezeArena = false;

  protected override void setup()
  {
    base.setup();
    addScore(-99999);
  }

  virtual public void restart()
  {
    Debug.Log("<b>RESTART</b> at "+Time.time);
    
    freezeArena = false;

    addScore(-9999);

    ArenaObject[] aobjs = GameObject.FindObjectsOfType<ArenaObject>();
    for (int i = 0; i < aobjs.Length; i++)
    {
      aobjs[i].restart();
    }
  }
  
  protected override void update()
  {
    base.update();
    
    if (Input.GetKeyUp(KeyCode.Space))
    {
      restart();
    }
    
  }

  protected void addScore(int step)
  {
    score += step;
    if (score < 0) score = 0;
    txt_score.text = "" + score;
  }

}
