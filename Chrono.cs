using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chrono : Timer {

  protected Canvas _parent;
  public Text txt_min;
  public Text txt_sec;

  public bool addZeros = true;

  protected override void build()
  {
    base.build();

    _parent = gameObject.GetComponentInParent<Canvas>();
  }
  
  public override void startTimer()
  {
    base.startTimer();

    _parent.enabled = true;
    updateVisual();
  }

  protected override void updateArenaLive(float timeStamp)
  {
    base.updateArenaLive(timeStamp);
    
    updateVisual();
  }

  protected void updateVisual()
  {
    float time = getTimeRemaining();

    //Debug.Log(time);

    int min = Mathf.FloorToInt(time / 60f);
    int sec = Mathf.FloorToInt(time - (min * 60f));

    //Debug.Log(time);
    if (addZeros && min < 10) txt_min.text = "0" + min;
    else txt_min.text = "" + min;

    if (addZeros && sec < 10) txt_sec.text = "0" + sec;
    else txt_sec.text = "" + sec;

  }

  public override void kill()
  {
    base.kill();
    
    _parent.enabled = false;
  }
}
