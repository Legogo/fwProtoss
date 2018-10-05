using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HalperUi {
  
  static public Image setupImage(Transform tr, Sprite spr, Color tint, bool visibility = true)
  {
    Image img = tr.GetComponent<Image>();
    if (img == null) return null;

    img.sprite = spr;
    img.color = tint;

    img.enabled = visibility;
    if (img.sprite == null) img.enabled = false;

    return img;
  }
  
}
