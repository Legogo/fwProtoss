using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleLabel : ModuleVisible {

  TextMesh txt;
  Renderer txtRender;

  protected override void build()
  {

    txt = transform.GetComponent<TextMesh>();
    txtRender = txt.GetComponent<MeshRenderer>();
    
    base.build();
    
  }

  public override void restart()
  {
    base.restart();
    hide();
  }

  public void updateLabel(string content)
  {
    txt.text = content;
  }

  protected void toggleLabel(bool flag)
  {
    txtRender.enabled = flag;
  }
  protected bool isLabelVisible() { return txtRender.enabled; }
  
  public override void show()
  {
    base.show();
    txtRender.enabled = true;
  }
  public override void hide()
  {
    base.hide();
    txtRender.enabled = false;
  }

}
