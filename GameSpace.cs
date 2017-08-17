using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSpace : MonoBehaviour {

  public Vector3 botLeft = Vector3.zero;
  public Vector3 topRight = Vector3.zero;

  public float width = 3f;
  public float height = 3f;

  public Transform[] borders;

  public bool matchScreen = false;

  static public GameSpace gameSpace;

  private void Awake()
  {
    gameSpace = this;
    updateSize();
  }

  void updateSize() {

    if (matchScreen)
    {
      botLeft = Camera.main.ScreenToWorldPoint(Vector2.zero);
      topRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
      return;
    }

    botLeft.x = width * -0.5f;
    botLeft.y = height * -0.5f;

    topRight.x = width * 0.5f;
    topRight.y = height * 0.5f;
  }

  public Vector2 getRandomPositionBorder(float distance)
  {
    //Vector2 pos = Random.onUnitSphere * 10f;
    Vector2 pos = Vector2.zero;


    if (Random.value > 0.5f)
    {
      //sides

      pos.y = Random.Range(botLeft.y, topRight.y);

      if (Random.value > 0.5f)
      {
        //left
        pos.x = botLeft.x - distance;
      }
      else
      {
        //right
        pos.x = topRight.x + distance;
      }
    

    }
    else
    {
      //tops

      pos.x = Random.Range(botLeft.x, topRight.x);
      
      if (Random.value > 0.5f)
      {
        //top
        pos.y = topRight.y + distance;
      }
      else
      {
        //bottom
        pos.y = botLeft.y - distance;
      }
      
    }

    return pos;
  }

  public float getWidth() { return topRight.x - botLeft.x; }
  public float getHeight() { return topRight.y - botLeft.y; }

  public Vector2 getRandomPosition(float borderGap)
  {
    Vector2 pos = Vector2.zero;
    pos.x = Random.Range(botLeft.x + borderGap, topRight.x - borderGap);
    pos.y = Random.Range(botLeft.y + borderGap, topRight.y - borderGap);
    return pos;
  }
  
  void OnDrawGizmos() {

    if(!Application.isPlaying)
    {
      updateSize();
    }
    

    Gizmos.DrawLine(botLeft, topRight);
    
    if(borders != null && borders.Length > 0)
    {
      if(borders.Length >= 1)
      {
        borders[0].position = new Vector2(-width * 0.5f,0f);
        borders[0].localScale = new Vector2(0.02f, width);
        borders[0].rotation = Quaternion.identity;
      }
      if(borders.Length >= 2)
      {
        borders[1].position = new Vector2(width * 0.5f, 0f);
        borders[1].localScale = new Vector2(0.02f, width);
        borders[1].rotation = Quaternion.identity;
      }
      if (borders.Length >= 3)
      {
        borders[2].position = new Vector2(0f, height * 0.5f);
        borders[2].localScale = new Vector2(0.02f, height);
        borders[2].rotation = Quaternion.AngleAxis(90f, Vector3.forward);
      }
      if (borders.Length >= 4)
      {
        borders[3].position = new Vector2(0f, -height * 0.5f);
        borders[3].localScale = new Vector2(0.02f, height);
        borders[3].rotation = Quaternion.AngleAxis(90f, Vector3.forward);
      }
    }
  }










  Vector2 LineIntersectionPoint(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2)
  {
    // Get A,B,C of first line - points : ps1 to pe1
    float A1 = pe1.y - ps1.y;
    float B1 = ps1.x - pe1.x;
    float C1 = A1 * ps1.x + B1 * ps1.y;

    // Get A,B,C of second line - points : ps2 to pe2
    float A2 = pe2.y - ps2.y;
    float B2 = ps2.x - pe2.x;
    float C2 = A2 * ps2.x + B2 * ps2.y;

    // Get delta and check if the lines are parallel
    float delta = A1 * B2 - A2 * B1;
    if (delta == 0)
      throw new System.Exception("Lines are parallel");

    // now return the Vector2 intersection point
    return new Vector2(
        (B2 * C1 - B1 * C2) / delta,
        (A1 * C2 - A2 * C1) / delta
    );
  }


  static public GameSpace get()
  {
    return GameObject.FindObjectOfType<GameSpace>();
  }
}
