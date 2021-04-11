using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using halper.visibility;

namespace ui
{
  public class UiSocketSelection : UiObject
  {
    Image img;
    public Sprite[] sprites;

    protected override void setup()
    {
      base.setup();

      img = visirUi.getImage();
    }

    public void setupSocket(int idx)
    {
      idx = Mathf.Clamp(idx, 0, sprites.Length - 1);

      visirUi.getImage().enabled = sprites[idx] != null;
      visirUi.setSymbol(sprites[idx]);
    }

    [ContextMenu("apply default")]
    protected void apply()
    {
      img = GetComponent<Image>();
      img.sprite = sprites[0];
      Vector2 size = img.rectTransform.sizeDelta;
      size.x = img.sprite.rect.width;
      size.y = img.sprite.rect.height;
      img.rectTransform.sizeDelta = size;
    }

    public Sprite getCurrentSprite()
    {
      return img.sprite;
    }
  }

}
