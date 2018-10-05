using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// All information for one finger
/// </summary>

public class InputTouchFinger {

  InputTouchBridge _manager;
  Camera _cam;
  
  public List<RaycastHit2D> touchedObjects = new List<RaycastHit2D>();

	public TouchPhase phase = TouchPhase.Ended;

	
  public Vector3 screenPosition = Vector3.zero; // position (X,Y) en pixel dans l'écran v3
  public Vector3 screenStartPosition = Vector3.zero;
  public Vector3 screenDeltaPosition = Vector3.zero;
  protected Vector3 screenLastPosition = Vector3.zero;

  public bool useWorldPosition = true; // optim
  public Vector3 worldPosition = Vector3.zero; // la coord dans le monde unity v3
  public Vector3 worldStartPosition = Vector3.zero;

  public Vector2 screenProportional = Vector2.zero; // la coord v2 [0,1]

	public int fingerId = -1;
  public HelperInputObject captured; // si un objet capture le doigt personne d'autre n'y aura accès

  float[] momentum = new float[10]; // permet de récup une valeur de magnitude sur les 5 dernières frames
	
	public InputTouchFinger(){

    _manager = InputTouchBridge.get();
    _cam = _manager.getInputCamera();
    if (_cam == null) Debug.LogError("finger input NEED camera to be assigned");

    reset();
    
	}
	
  public void setEnded() {
    phase = TouchPhase.Ended;
  }

  public void reset() {
    screenLastPosition = Vector3.one * -1000;
    
    //reset screen
    screenDeltaPosition = screenStartPosition = screenPosition = screenLastPosition;

    //reset world
    worldStartPosition = worldPosition = screenLastPosition;

    phase = TouchPhase.Canceled;
    fingerId = -1;
    resetMomentum();
  }

  /* update avec le framework de touch */
  public void update(Touch touch) {
    phase = touch.phase;

    fingerId = touch.fingerId;

    screenLastPosition = screenPosition;

    screenPosition = touch.position;
    
    worldPosition = _cam.ScreenToWorldPoint(screenPosition);

    solveDuplicate();
    
    if (phase == TouchPhase.Began) {
      screenStartPosition = screenPosition;
      worldStartPosition = worldPosition;
    }

    //la ended phase dure 6 frames
    if (phase != TouchPhase.Ended)
    {
      screenDeltaPosition = touch.deltaPosition;
      //worldDeltaPosition = screenPosition - screenStartPosition;

      if (screenDeltaPosition.magnitude > 0f)
      {
        addMomentum(screenDeltaPosition.magnitude);
      }
      
    }

    if (isFingerUsed()) solve_finger_selection();
  }
  
  /* update simulation de touch avec la souris */
  public void update(int idx, Vector3 mousePosition) {
    fingerId = idx;
    
    screenPosition = mousePosition; // set new position

    //Debug.Log("update " + idx + " with pos " + mousePosition+" , phase : "+phase);

    //faut éviter un delta incohérent au moment du touch
    if (phase == TouchPhase.Began || phase == TouchPhase.Canceled)
    {
      screenLastPosition = screenPosition;
    }

    // keep old position
    //if (lastPosition.magnitude == 0) lastPosition = position;

    if (useWorldPosition)
    {
      //screenPosition.z = _cam.transform.position.z;

      worldPosition = _cam.ScreenToWorldPoint(screenPosition);
      //Debug.Log(screenPosition+" , "+worldPosition);
    }

    screenDeltaPosition = screenPosition - screenLastPosition; // solve delta with old position

    screenLastPosition = screenPosition; // remove old position for next frame

    solveDuplicate();
    
    //next position
    if (phase == TouchPhase.Canceled || phase == TouchPhase.Ended)
    {
      phase = TouchPhase.Began;
      screenStartPosition = screenPosition;
      worldStartPosition = worldPosition;
    }
    else if (screenDeltaPosition.magnitude > 0)
    {
      phase = TouchPhase.Moved;
      addMomentum(screenDeltaPosition.magnitude);
    }
    else if (screenDeltaPosition.magnitude == 0)
    {
      phase = TouchPhase.Stationary;
      resetMomentum();
    }

    if (isFingerUsed()) solve_finger_selection();
  }

  protected void solveDuplicate() {
    
    screenProportional.x = screenPosition.x / Screen.width;
    screenProportional.y = screenPosition.y / Screen.height;

  }

  protected void solve_finger_selection()
  {
    touchedObjects.Clear();
    bool hitSomething = true;

    //Debug.Log("solve");
    
    Vector2 origin = worldPosition;
    Vector2 direction = Vector2.zero;

    //float distanceRay = 100f;
    //Vector2 target = origin + (Vector3.forward * distanceRay);

    RaycastHit2D _hit;

    while (hitSomething)
    {
      hitSomething = false;

      _hit = Physics2D.Raycast(origin, direction, Mathf.Infinity, _manager._layer);
      
      if(_hit.collider != null) {
      //if (Physics.Raycast(origin, Vector3.forward, out _hit, distanceRay, _manager._layer)) {
        hitSomething = true;
        
        Debug.DrawLine((Vector3)_hit.point + Vector3.forward, (Vector3)_hit.point + Vector3.back, Color.green);
        
        touchedObjects.Add(_hit);

        //desactiver le collider pour rebalancer un autre rayon au même endroit
        _hit.collider.enabled = false;

        //si le collider trouvé possède "isTrigger" ca va bloquer le raycast a cet objet et zappé les suivants
        if (_hit.collider.isTrigger) hitSomething = false;
      }
    }

    //reactiver tout les colliders raycast-ed
    for (int i = 0; i < touchedObjects.Count; i++)
    {
      touchedObjects[i].collider.enabled = true;
    }

    //debug draw ray red if nothing found
    //_hit.
    if (touchedObjects.Count <= 0) { Debug.DrawLine((Vector3)origin + Vector3.forward, (Vector3)origin + Vector3.back, Color.red); }
  }

  public bool hasTouchedSomething() {
    return touchedObjects.Count > 0;
  }

  public bool hasCollider(Collider col) {
    for (int i = 0; i < touchedObjects.Count; i++)
    {
      if (touchedObjects[i].collider == col) return true;
    }
    return false;
  }
  
  // release
  public bool isPhaseEnded() { return phase == TouchPhase.Ended; }
  public bool isPhaseCanceled() { return phase == TouchPhase.Canceled; }

  /* si le doigt a toujours un impact potentiel dans l'espace de jeu donc meme dans la frame de release ! */
  public bool isFingerUsed() {
    return phase != TouchPhase.Canceled;
  }

	public void resetMomentum(){
		for(int i = 0; i < momentum.Length; i++){
			momentum[i] = 0f;
		}
    screenDeltaPosition = Vector3.zero;
	}

	public void addMomentum(float delta){
		//shift
		for(int i = momentum.Length - 1; i > 0; i--){
			momentum[i] = momentum[i - 1];
		}
		
		momentum[0] = delta;
	}
  
  public float getMomentumAverage() {
    return getMomentum() / momentum.Length;
  }

	public float getMomentum(){
		float total = 0f;
		for(int i = 0; i < momentum.Length; i++){
			total += momentum[i];
		}
		return total;
	}
	
  public bool hasStayedNearStartTouchPosition(float distanceFromStartPx = 1f)
  {
    float dist = Vector3.Distance(screenStartPosition, screenPosition);
    //Debug.Log(dist + " < " + distanceFromStart);
    return dist < distanceFromStartPx;
  }

  public bool isCapturedBy(HelperInputObject obj) { if (!isCaptured()) return false; return obj == captured; }
  public bool isCaptured() {
    return captured != null;
  }

  public Vector3 getVectorFromStart(){
    screenPosition.z = 0f;
    screenStartPosition.z = 0f;
    return (screenPosition - screenStartPosition);
  }

	public Vector3 getDirFromStart(){
    screenPosition.z = 0f;
    screenStartPosition.z = 0f;
		return (screenPosition - screenStartPosition).normalized;
	}




  public string toString()
  {
    string ctx = "id:" + fingerId + " " + (isPhaseCanceled() ? "[CANCELED]" : "") + " position:" + screenPosition + " state:" + phase;
    ctx += "\ndir(Start):" + getDirFromStart() + ", momentum:" + getMomentum() + ", delta:" + screenDeltaPosition + "(mg:" + screenDeltaPosition.magnitude + ")";
    
    if(useWorldPosition) {
      ctx += "\norigin " + worldStartPosition + " current " + worldPosition;
    }

    ctx += "\nColliders(" + touchedObjects.Count + ")";
    for (int i = 0; i < touchedObjects.Count; i++)
    {
      if (touchedObjects[i].collider == null) continue;

      Transform pr = touchedObjects[i].collider.transform.parent;
      ctx += "\n " + touchedObjects[i].collider.transform.name;
      if (pr != null) ctx += " (" + pr.name + ")";
    }

    return ctx;
  }

  void OnDrawGizmos() {
    Gizmos.color = Color.red;
    Gizmos.DrawSphere(screenPosition, 0.5f);
  }
}
