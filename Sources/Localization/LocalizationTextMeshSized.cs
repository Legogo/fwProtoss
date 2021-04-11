using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationTextMeshSized : EngineObject
{
  public string id = "";
  
  public TextMesh txt_mesh;

  protected override void setup()
  {
    base.setup();

    fetch();

    //LocalizationManager.manager.onLanguageChange += onLanguageChange;
  }
  
  protected void fetch()
  {

    if (txt_mesh == null)
    {
      txt_mesh = GetComponent<TextMesh>();
      if (txt_mesh == null) Debug.LogError("Missing textMesh on localizationPlug", this);
    }

  }


  virtual public void onLanguageChange(string newLang)
  {
    if (id == null)
    {
      Debug.LogError("no id for " + name, transform);
      return;
    }

    fetch();

    string content = LocalizationManager.getContent(id);

    content = content.Replace("\\n", "\n");
    
    if (txt_mesh != null)
    {
      txt_mesh.text = content;
    }
  }

  protected override void destroy()
  {
    base.destroy();
    //if (LocalizationManager.manager != null) LocalizationManager.manager.onLanguageChange -= onLanguageChange;
  }

}
