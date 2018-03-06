using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiSocketSelection : EngineObject {

  Image img;
  HelperVisibleUi ui;
  public Sprite[] sprites;
  
  protected override void setup()
  {
    base.setup();
    if (ui == null) ui = visibility as HelperVisibleUi;
    img = ui.getImage();
  }

  public void setupSocket(int idx)
  {
    

    idx = Mathf.Clamp(idx, 0, sprites.Length-1);
    ui.setSprite(sprites[idx]);
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
}
