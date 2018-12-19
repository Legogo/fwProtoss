using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UiChrono : ArenaObject {

  TimeObject time; // generic object
  Timer timer;
  Countdown countdown;

  public bool isCountdown = false;

  [Header("to refs")]
  public Text txt_min;
  public Text txt_sec;
  public Text txt_ms;
  
  [Header("formating")]
  public bool mm_addZeros = true;
  public bool ss_addZeros = true;
  public bool ms_addZeros = true;

  protected override void build()
  {
    base.build();

    if (isCountdown)
    {
      countdown = new Countdown();
      time = countdown;
    }
    else
    {
      timer = new Timer();
      time = timer;
    }
  }
  
  [ContextMenu("play")]
  public void play() { time.play(); }
  [ContextMenu("reset")]
  public void reset() { time.reset(); }
  [ContextMenu("stop")]
  public void stop() { time.stop(); }
  [ContextMenu("pause")]
  public void pause() { time.pause(); }

  public TimeObject getTimeObject() { return time; }

  public float getTime(bool remaining = false) {
    if (remaining) return time.getTimeRemaining();
    else return time.getTime();
  }

  public void subscribeToTimeout(Action callback)
  {
    countdown.onTimeout += callback;
  }

  public void play(float timeout)
  {
    if (countdown == null) Debug.LogError("can't setup timeout if chrono is not setup as countdown (bool in editor)");
    countdown.timeoutTime = timeout;
    countdown.play(true);
  }

  public void toggleVisibility(bool flag)
  {
    if (txt_min != null) txt_min.enabled = flag;
    if (txt_sec != null) txt_sec.enabled = flag;
    if (txt_ms != null) txt_ms.enabled = flag;
  }

  protected override void updateArenaLive(float timeStamp)
  {
    base.updateArenaLive(timeStamp);

    time.update();

    updateVisual();
  }

  protected void updateVisual()
  {
    float chronoTime = time.getTime();
    if (countdown != null) chronoTime = countdown.getTimeRemaining();
    
    int min, sec, ms;

    min = 0;
    if(txt_min != null) min = Mathf.FloorToInt(chronoTime / 60f);

    sec = Mathf.FloorToInt(chronoTime - (min * 60f));
    
    ms = Mathf.FloorToInt((chronoTime - (min * 60f) - sec) * 1000f);
    
    if (txt_min != null)
    {
      if (mm_addZeros && min < 10) txt_min.text = "0" + min;
      else txt_min.text = "" + min;
    }

    if (txt_sec != null)
    {
      if (ss_addZeros && sec < 10) txt_sec.text = "0" + sec;
      else txt_sec.text = "" + sec;
    }

    if(txt_ms != null)
    {
      if (!ms_addZeros || ms > 100) txt_ms.text = ms.ToString();
      else
      {
        if (ms < 10) txt_ms.text = "00" + ms;
        else txt_ms.text = "0" + ms;
      }
    }
    
    //Debug.Log(time+"="+min+":"+sec+":"+ms);
  }

  public override void kill()
  {
    base.kill();
    clean();
  }

  public void clean()
  {
    toggleVisibility(false);
  }

#if UNITY_EDITOR
  private void OnDrawGizmos()
  {
    if (time != null) {
      UnityEditor.Handles.Label(transform.position + Vector3.right, "" + time.getTime());
    }
  }
#endif

  public override string toString()
  {
    string ct = base.toString();

    ct += "\n(time)";
    ct += "\n  running ? " + time.isRunning();
    ct += "\n  time ? "+ time.getTime();

    return ct;
  }
}
