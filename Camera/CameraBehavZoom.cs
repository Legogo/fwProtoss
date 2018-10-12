using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using fwp.input;

public class CameraBehavZoom : MonoBehaviour {

  Camera cam;

  [Header("zoom param")]
  public float dezoomFactor = 4f; // what to add to original ortho based on pinchProgression

  [Header("camera zoom")]
  public float zoomFollowSpeed = 1f;
  public bool dampingZoomSpeed = false; // amortie de la vitesse de camera follow ortho target
  public float dampingSpeed = 10f; // distance max d'écart pour atteindre la valeur max de lerp
  public float dampingDistMax = 10f; // distance max d'écart pour atteindre la valeur max de lerp
  
  [Header("read only")]
  public float pinchMaxLimit = 0f;
  public float pinchProgression = 0f;
  public float originalOrtho = 0f;

  float previousPinch = 0f;

  IEnumerator Start()
  {
    enabled = false;

    InputTouchBridge itb = InputTouchBridge.get();

    while (itb == null)
    {
      itb = InputTouchBridge.get();
      if(itb == null) yield return null;
    }

    cam = GetComponent<Camera>();
    originalOrtho = cam.orthographicSize;

    
    itb.subscribeToScroll(onScroll);
    pinchMaxLimit = itb.scrollClampMagnitude.y;

    enabled = true;
	}
  
  void onScroll(float delta, float magnitude)
  {
    //Debug.Log(GetType() + " | " + delta + " , " + magnitude);

    //store previous pinch for comparison
    previousPinch = pinchProgression;

    //apply current delta
    if (delta != 0f)
    {
      pinchProgression = magnitude;
      pinchProgression = Mathf.Clamp(pinchProgression, 0f, pinchMaxLimit);
    }
    
    updateZoom();

    onZoomVariation(previousPinch, delta, magnitude);
  }
  
  virtual protected void onZoomVariation(float prevPinch, float pinchDelta, float pinchMagnitude)
  {}

  void updateZoom()
  {
    float targetOrtho = Mathf.InverseLerp(0f, pinchMaxLimit, pinchProgression) * dezoomFactor;

    float zoomSpeed = zoomFollowSpeed; // default

    if (dampingZoomSpeed)
    {
      float lerpDistance = Mathf.InverseLerp(0f, dampingDistMax, Mathf.Abs(targetOrtho - cam.orthographicSize));
      zoomSpeed = Mathf.Lerp(0.1f, dampingSpeed, lerpDistance);
    }

    cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, originalOrtho + targetOrtho, Time.deltaTime * zoomSpeed);
  }
  
}
