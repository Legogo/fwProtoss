using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraBehavZoom : MonoBehaviour {

  float originalOrtho = 0f;
  Camera cam;

  float pinchProgression = 0f; // [0,1] progression

  [Serializable]
  public struct CamBehavZoomData
  {
    public float deltaLerpOrthoSize; // what to add to original ortho based on pinchProgression
    public float zoomFollowSpeed;
    public bool zoomFollowSpeedByDistance;
    public float zoomInputStepSpeed;
  }

  public CamBehavZoomData mobile;
  public CamBehavZoomData desktop;

  IEnumerator Start()
  {
    enabled = false;
    
    while(InputTouchBridge.get() == null) yield return null;

    cam = GetComponent<Camera>();
    originalOrtho = cam.orthographicSize;

    enabled = true;
	}

  protected CamBehavZoomData getDeviceData()
  {

    if (!InputTouchBridge.isMobile()) return desktop;
    else return mobile;

  }

  private void Update()
  {
    CamBehavZoomData data = getDeviceData();

    float pinch = pinchProgression;

    float pinchDelta = InputTouchBridge.get().deltaPinch;
    if (pinchDelta != 0f) onPinch(pinchDelta);

    float lerp = Mathf.Lerp(0f, data.deltaLerpOrthoSize, pinchProgression);
    float zoomSpeed = data.zoomFollowSpeed;
    if (data.zoomFollowSpeedByDistance)
    {
      zoomSpeed = Mathf.Lerp(0.1f, data.zoomFollowSpeed, Mathf.InverseLerp(0f, 10f, Mathf.Abs(lerp - cam.orthographicSize)));
    }

    cam.orthographicSize = Mathf.MoveTowards(cam.orthographicSize, originalOrtho + lerp, Time.deltaTime * zoomSpeed);
    



    if (pinch < data.deltaLerpOrthoSize && pinchProgression == data.deltaLerpOrthoSize)
    {
      displayMap(true);
    }
    else if (pinch > 0.95f && pinchProgression <= 0.95f)
    {
      displayMap(false);
    }


  }


  protected void displayMap(bool flag)
  {
    LabyMap map = GameObject.FindObjectOfType<LabyMap>();
    if (map == null) return;

    //Debug.Log("map display : " + flag);

    if (flag) map.show();
    else map.hide();
  }

  /// <summary>
  /// Sur mobile on recoit BEAUCOUP plus d'input que sur desktop avec une molette classique
  /// </summary>
  /// <param name="delta"></param>
  protected void onPinch(float delta)
  {
    CamBehavZoomData data = getDeviceData();

    if (!InputTouchBridge.isMobile()) // desktop
    {
      float target = Mathf.Sign(delta) > 0 ? 0f : 1f;
      pinchProgression = Mathf.MoveTowards(pinchProgression, target, Time.deltaTime * data.zoomInputStepSpeed);
    }
    else
    {
      pinchProgression += delta * data.zoomInputStepSpeed;
      pinchProgression = Mathf.Clamp01(pinchProgression);
    }

    //Debug.Log(pinchProgression);
  }

}
