using UnityEngine;
using System.Collections.Generic;
using System;

public class InputObject : MonoBehaviour {

  protected bool _overring = true; // est-ce que l'objet réagit a la callback d'un doigt qui passe au dessus de lui (après touch event)
  protected bool _interactive = true; // réagit aux touch/release/overring
  protected bool _touching = false; // état press/release (besoin de pas utiliser le doigt directement pour debug possible)
  protected bool _inputLayerAtStart = false;

  public bool dontReactToCapturedFinger = false;
  public bool captureFingerOnTouch = false;

  protected InputTouchBridge _input;
  protected Collider2D[] _colliders;
  
  public Action<InputTouchFinger> cbTouch;
  public Action<InputTouchFinger, RaycastHit2D> cbTouchOver;
  public Action<InputTouchFinger> cbRelease;
  public Action<InputTouchFinger, RaycastHit2D> cbReleaseOver;
  public Action<InputTouchFinger> cbOver;
  
  void Awake()
  {
    InputTouchBridge.get();

    _inputLayerAtStart = gameObject.layer == LayerMask.NameToLayer("input");

    //_colliders = ToolsComponent.fetchComponents<BoxCollider>(transform);
    _colliders = transform.GetComponentsInChildren<Collider2D>();
    //if(_colliders.Length <= 0) Debug.LogWarning(name + " , colliders count = " + _colliders.Length);
  }

  virtual protected void Start()
  {
    
    //input est dispo QUE après le loading de common
    //only once
    if (_input == null)
    {
      _input = InputTouchBridge.get();
      if (_input == null) Debug.LogError("<EngineObject> no [input] in scene ??");

      //subscribe to manager
      _input.onTouch += eventOnTouch;
      _input.onRelease += eventOnRelease;
      _input.onOverring += eventOnOverring;
    }

  }

  virtual public void unlink() {
    _input.onTouch -= eventOnTouch;
    _input.onRelease -= eventOnRelease;
  }

  /* un doigt passe par là et y a que moi en dessous */
  private void eventOnOverring(InputTouchFinger finger) {
    if (!_interactive) return;
    if (!_overring) return;

    if (finger.isCaptured() && dontReactToCapturedFinger) return;

    RaycastHit2D? hit = getLocalMatchingColliderWithFinger(finger);
    if (hit != null)
    {
      //Debug.Log("overring "+finger.touchedObjects[0].collider.gameObject.name+" , count = "+finger.touchedObjects.Count);
      onOverring(finger);
    }
    
  }
  
  /* global touch event (everybody get this event) */
  private void eventOnTouch(InputTouchFinger finger)
  {
    //Debug.Log(name + " eventOnTouch (interactive?" + _interactive+") , finger "+finger.fingerId+" captured ? "+finger.captured, gameObject);

    if (!_interactive) return;

    if (dontReactToCapturedFinger)
    {
      if(!finger.isCapturedBy(this) && finger.isCaptured()) {
        //Debug.Log(name + " can't react to finger " + finger.fingerId + " because is captured by someone else : " + finger.captured, gameObject);
        return;
      }
    }

    //Debug.Log(finger.fingerId + " on touch "+name);

    onTouch(finger);
    
    RaycastHit2D? hit = getLocalMatchingColliderWithFinger(finger);
    
    if(hit != null) {
      _touching = true;

      //Debug.Log("{InputObject} input touch over " + hit.Value.transform.name);
      eventTouchOver(finger, hit.Value);
    }
  }

  private void eventTouchOver(InputTouchFinger finger, RaycastHit2D hit) {
    
    if (!finger.isCaptured() && captureFingerOnTouch)
    {
      //Debug.Log(name + " <b>captured</b> finger " + finger.fingerId);
      finger.captured = this;
    }

    onTouchOver(finger, hit);
  }

  private void eventOnRelease(InputTouchFinger finger)
  {
    //Debug.Log(name+" release finger "+finger.fingerId);

    if (finger.isCapturedBy(this))
    {
      //Debug.Log(name + " <b>released</b> finger " + finger.fingerId);
      finger.captured = null;
    }

    if (!_interactive) return;
    
    if (finger.isCaptured() && dontReactToCapturedFinger) return;

    //on a besoin que touching soit MAJ avant d'apl ça pour le cas ou on desactive l'interactivité de l'extérieur
    bool wasTouching = _touching;
    _touching = false;

    onRelease(finger);

    if (wasTouching)
    {
      //chopper le hit correspondant a mes colliders
      RaycastHit2D? hit = getLocalMatchingColliderWithFinger(finger);

      if (hit != null)
      {
        //Debug.Log("{InputObject} input released over " + hit.Value.transform.name);
        onReleaseOver(finger, hit.Value);
      }

    }
    
    //if (cbRelease != null) cbRelease(finger);

  }


  public void assignFinger(InputTouchFinger finger)
  {
    eventOnTouch(finger);
  }



  #region to override by children

  virtual protected void onTouch(InputTouchFinger finger) {
    //Debug.Log(name + " touch scene");
    if (cbTouch != null) cbTouch(finger);
  }
  virtual protected void onRelease(InputTouchFinger finger) {
    //Debug.Log(name + " release scene"); 
  }
  virtual protected void onOverring(InputTouchFinger finger)
  {
    //Debug.Log(name + " overring");
    if (cbOver != null) cbOver(finger);
  }
  virtual protected void onTouchOver(InputTouchFinger finger, RaycastHit2D hit) {
    //Debug.Log(name + " touch over");
    if (cbTouchOver != null) cbTouchOver(finger, hit);
  }
  virtual protected void onReleaseOver(InputTouchFinger finger, RaycastHit2D hit) {
    //Debug.Log(name + " release over");
    if (cbReleaseOver != null) cbReleaseOver(finger, hit);
  }

  #endregion




  protected RaycastHit2D? getLocalMatchingColliderWithFinger(InputTouchFinger finger) {
    List<RaycastHit2D> rCollisions = finger.touchedObjects;
    for (int i = 0; i < rCollisions.Count; i++)
    {
      //check si le collider trouvé par le doigt correspond a l'un des miens
      if (compareWithLocalCollider(rCollisions[i].collider)) return rCollisions[i];
    }
    return null;
  }
  protected bool compareWithLocalCollider(Component collider)
  {
    for (int i = 0; i < _colliders.Length; i++)
    {
      if (_colliders[i] == collider) return true;
    }
    return false;
  }

  public void setOverringCapacity(bool flag)
  {
    _overring = flag;
  }

  public virtual void setInteractive(bool flag)
  {
    _interactive = flag;

    if (!_interactive) gameObject.layer = 0;
    else {
      //reinjecte la layer input si besoin
      if(_inputLayerAtStart) gameObject.layer = LayerMask.NameToLayer("input");
    }

    //Debug.Log(name + " swap to layer " + gameObject.layer, gameObject);

    //LogHomo.interactor("{InputObject} setInteracte(" + flag+") for "+name);
  }
  public bool isInteractive() { return _interactive; }
  
	public bool isTouching()
	{
		return _touching;
	}
}
