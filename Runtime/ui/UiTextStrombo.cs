using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiTextStrombo : MonoBehaviour
{

  protected Text txt;

  public Color originColor;
  public Color targetColor;

  protected float frame = 0;

  private void Awake()
  {
    txt = GetComponent<Text>();
  }

  private void Update()
  {
    
    if(frame > 0)
    {
      frame--;
      return;
    }

    txt.color = txt.color == originColor ? targetColor : originColor;
    frame = Random.Range(0, 4);
  }
}
