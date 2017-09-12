using UnityEngine;
using System.Collections;

public class FpsWarning : MonoBehaviour
{
  int fps = 0;

  public int frameAvgCount = 30;
  public int warningLimit = 60;

  protected int avg_idx = 0;
  protected int[] avg;

  GUIStyle style;

  public Color valid;
  public Color invalid;

  private void Awake()
  {
    avg = new int[frameAvgCount];

    if (style == null)
    {
      style = new GUIStyle();
    }
  }

  void OnGUI()
  {
    fps = Mathf.FloorToInt(1.0f / Time.deltaTime);

    float average = fps;

    if (avg.Length > 0)
    {
      average = 0f;

      avg[avg_idx] = fps;
      avg_idx++;
      if (avg_idx >= avg.Length) avg_idx = 0;
      
      for (int i = 0; i < avg.Length; i++)
      {
        average += avg[i];
      }
      average /= avg.Length;
    }

    //style.normal.textColor = (fps < monitorValue) ? invalid : valid;
    style.normal.textColor = (average > warningLimit) ? valid : invalid;
    style.fontSize = 30;

    GUI.Label(new Rect(10, 10, 400, 400), "FPS " + fps+" ("+ average + ")", style);
  }
}