using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

abstract public class ArenaManager : EngineObject {

  public bool freezeArena = false;

  public float time = 0f;

  public Text txt_score;
  
  protected override void setup()
  {
    base.setup();
    setScore(0, txt_score);
  }

  virtual public void restart()
  {
    Debug.Log("<b>RESTART</b> at "+Time.time);
    
    freezeArena = false;

    setScore(0, txt_score);

    ArenaObject[] aobjs = GameObject.FindObjectsOfType<ArenaObject>();
    for (int i = 0; i < aobjs.Length; i++)
    {
      aobjs[i].restart();
    }
  }

  virtual public void event_score(int step)
  {
    addScore(step, txt_score);
  }
  
  protected override void update()
  {
    base.update();
    
    if (Input.GetKeyUp(KeyCode.Space))
    {
      restart();
    }

    time += Time.deltaTime;
  }

  protected void setScore(int newScore, Text txt)
  {
    if (newScore < 0) newScore = 0;
    txt.text = "" + newScore;
  }

  protected void addScore(int step, Text txt)
  {
    setScore(int.Parse(txt.text) + step, txt);
  }

  public float getElapsedTime()
  {
    return time;
  }

  static protected ArenaManager _manager;
  static public ArenaManager get()
  {
    if (_manager == null) _manager = GameObject.FindObjectOfType<ArenaManager>();
    return _manager;
  }
}
