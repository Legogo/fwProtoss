using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleLabel : ModuleVisible {

  TextMesh txt;
  Renderer txtRender;
  Text ui_txt;

  protected override void build()
  {

    txt = transform.GetComponent<TextMesh>();
    if(txt != null)
    {
      txtRender = txt.GetComponent<MeshRenderer>();
    }
    
    ui_txt = GetComponent<Text>();

    if(name.StartsWith("s_") && ui_txt == null) Debug.LogError("no txt for " + name + " ?", gameObject);
    
    base.build();
  }

  public override void restart()
  {
    base.restart();

    if (!dontHideOnStartup)
    {
      hide();
    }
    
  }

  public void updateLabel(string content)
  {
    if (ui_txt != null)
    {
      ui_txt.text = content;
    }
    else
    {
      txt.text = content;
    }
  }

  protected void toggleLabel(bool flag)
  {
    if (ui_txt != null) ui_txt.enabled = flag;
    else if(txtRender != null) txtRender.enabled = flag;
  }
  protected bool isLabelVisible() { return txtRender.enabled; }
  
  public override void show()
  {
    base.show();
    toggleLabel(true);
  }
  public override void hide()
  {
    base.hide();
    toggleLabel(false);
  }

  public override bool isVisible()
  {
    if (ui_txt != null) return ui_txt.enabled;
    return txtRender.enabled;
  }

}
