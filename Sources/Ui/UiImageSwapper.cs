using System.Collections;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// pour facilement changer l'image d'un <Image> basé sur une liste
/// </summary>

public class UiImageSwapper : EngineObject {

  public Sprite[] frames;

  protected override VisibilityMode getVisibilityType()
  {
    return VisibilityMode.UI;
  }

  public void swap(string endName)
  {
    for (int i = 0; i < frames.Length; i++)
    {
      if(frames[i].name.EndsWith(endName))
      {
        swap(i);
        return;
      }
    }
  }
  public void swap(int frameIdx)
  {
    (visibility as HelperVisibleUi).setSymbol(frames[frameIdx]);
  }

}
