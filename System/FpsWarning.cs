using UnityEngine;
using System.Collections;

public class FpsWarning : MonoBehaviour
{
  int fps = 0;

  public int frameAvgCount = 30;
  public int warningLimit = 60;

  protected int avg_idx = 0;
  protected int[] avg;
  float average = 0f;

  public Color valid;
  public Color invalid;

  string ct = "";
  GUIStyle style;

  private void Awake()
  {
    avg = new int[frameAvgCount];
    style = new GUIStyle();

    setupFont();
  }

  void setupFont()
  {

    style.normal.textColor = (average > warningLimit) ? valid : invalid;

    if (Screen.width >= 1920f) style.fontSize = 30;

    //Debug.Log(style.fontSize);
  }

  private void Update()
  {
    processGui();
  }

  void processGui()
  {
    fps = Mathf.FloorToInt(1.0f / Time.deltaTime);

    average = fps;

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
    
    ct = "FPS " + fps + " (" + average + ")";
  }

  private void OnGUI()
  {
    GUI.Label(new Rect(0, 0, 300, 300), ct, style);
  }
}