using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

static public class HalperUi {
  
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
  
  static public Vector2 getBottomCenterPosition(this RectTransform elmt)
  {
    return elmt.anchoredPosition + (Vector2.down * elmt.sizeDelta.y) + (Vector2.right * elmt.sizeDelta.x * 0.5f);
  }

  static public float getHeight(this RectTransform elmt)
  {
    return elmt.sizeDelta.y;
  }
  static public float getWidth(this RectTransform elmt)
  {
    return elmt.sizeDelta.y;
  }

  static public void setWidth(this RectTransform elmt, float val)
  {
    Vector2 size = elmt.sizeDelta;
    size.x = val;
    elmt.sizeDelta = size;
  }

  static public void setHeight(this RectTransform elmt, float val)
  {
    Vector2 size = elmt.sizeDelta;
    size.y = val;
    elmt.sizeDelta = size;
  }
}
