using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaObjectVisible : ArenaObject {

  public float scaleSize = 1f;

  protected ModuleVisible _modVisible;
  protected ModuleLabel _modLabel;
  protected ModuleLabelCounter _modLabelCounter;

  protected override void build()
  {
    base.build();

    transform.localScale = Vector3.one * scaleSize;

    //Debug.Log(name + " (" + GetType() + ") scale " + transform.localScale, gameObject);

    _modVisible = transform.GetComponent<ModuleVisible>();

    //enfant !
    _modLabel = transform.GetComponentInChildren<ModuleLabel>();
    _modLabelCounter = (ModuleLabelCounter)_modLabel;

    if (_modVisible == null) Debug.LogError("no mod visible for "+name, gameObject);

  }

  protected override void spawnProcess(Vector3 position)
  {
    base.spawnProcess(position);

    _modVisible.show();

    if(_modLabelCounter != null) _modLabelCounter.show();
  }

  public override void kill()
  {
    base.kill();
    _modVisible.hide();
    if(_modLabel != null) _modLabel.hide();
  }

  public void showCounter()
  {
    if (_modLabelCounter != null) _modLabelCounter.show();
  }

  // on peut pas utiliser transform.localScale a cause de la valeur qui varie quand on change de parent
  public float getSize() { return scaleSize * 0.5f; }

  private void OnDrawGizmosSelected()
  {
    if (Application.isPlaying) return;

    transform.localScale = Vector3.one;

    //symbol !
    Transform tr = transform.GetChild(0);
    tr.localScale = Vector3.one * scaleSize;
  }
  
  public Vector2 getObjectPointNearestToTransform(Transform tr)
  {
    //choppe le point au bord du cercle dans la direction du block
    Vector2 dir = tr.position - transform.position;
    dir = dir.normalized * (getSize() * 0.6f);
    dir = (Vector2)transform.position + dir;

    Debug.DrawLine(dir, transform.position, Color.green);
    return dir;
  }
}
