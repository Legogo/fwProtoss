using UnityEngine;
using System.Collections;

static public class ScreenLimits
{
  static public Vector3 levelBottomLeft; // top left of screen
  static public Vector3 levelTopRight; // bottom right of screen

  static private Camera refCamera;
  
  static public GameObject left;
  static public GameObject right;
  
  // sera apl au premier resize()
  static public void setup(){
    
    if (refCamera == null) refCamera = Camera.main;
    
    if (left == null)
    {
      left = GameObject.CreatePrimitive(PrimitiveType.Cube);
      left.name = "limit_left";
      left.transform.SetParent(refCamera.transform);
    }

    if (right == null)
    {
      right = GameObject.CreatePrimitive(PrimitiveType.Cube);
      right.name = "limit_right";
      right.transform.SetParent(refCamera.transform);
    }

  }
  
  /* habituellement c'est la camera qui apl le resize */
  static public void resize()
  {
    if (refCamera == null) setup();
    
    float height = refCamera.orthographicSize;
    float width = refCamera.aspect * (refCamera.orthographicSize);

    //Debug.Log(width+","+height);

    Vector3 camPos = refCamera.transform.position;
    levelBottomLeft = new Vector3(camPos.x - width, camPos.y - height, camPos.z);
    levelTopRight = new Vector3(camPos.x + width, camPos.y + height, camPos.z);

    if (left != null) left.transform.position = levelBottomLeft;
    if (right != null) right.transform.position = levelTopRight;
    
  }

}
