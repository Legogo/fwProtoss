using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class ArenaLane : ArenaObject {
  
  public DataLane data;
  protected Timer timer;

  public Transform spawnOffsetReferencial;

  protected List<ArenaLaneObstacle> obstacles = new List<ArenaLaneObstacle>();

  protected override void build()
  {
    base.build();

    timer = gameObject.AddComponent<Timer>();
    timer.timeout += spawnTimeout;
  }

  public override void restart()
  {
    base.restart();
    timer.setupAndStart(data.timer);
    //Debug.Log(name + " restart");

    //clean
    while(obstacles.Count > 0)
    {
      GameObject.DestroyImmediate(obstacles[0].gameObject);
      obstacles.RemoveAt(0);
    }
  }

  protected void spawnTimeout()
  {
    if (data.obstacles.Length <= 0) return;

    Sprite spr = data.obstacles[Random.Range(0, data.obstacles.Length)];
    GameObject obj = ResourceManager.getDuplicate(spr.name);

    if (obj == null)
    {
      Debug.LogError("no object " + spr.name + " found in resources");
      return;
    }

    //Debug.Log(spr.name);
    //Debug.Log(obj, obj);

    //obj.transform.SetParent(transform);
    obj.transform.position = transform.position;

    ArenaLaneObstacle obs = obj.GetComponent<ArenaLaneObstacle>();
    obs.setupOnLane(this);
    obstacles.Add(obs);
  }
  
  public override void updateArena()
  {
    base.updateArena();

    //make spawn offset based on a referencial
    if(spawnOffsetReferencial != null)
    {
      //Debug.Log(GameSpace.get().getWidth());

      float oosPosition = spawnOffsetReferencial.position.x + GameSpace.get().getWidth();
      Vector3 pos = transform.position;
      pos.x = oosPosition;
      //pos.y = spawnOffsetReferencial.position.y - 2f;
      transform.position = pos;
    }

    int i = 0;
    while (i < obstacles.Count)
    {
      //remove cleared
      if(obstacles[i] == null)
      {
        obstacles.RemoveAt(i);
        //Debug.Log("removed at " + i);
        continue;
      }

      //translate
      obstacles[i].transform.Translate(Vector3.left * obstacles[i].getSpeed() * Time.deltaTime);

      //remove on ooscreen
      if (obstacles[i].transform.position.x < -12f)
      {
        GameObject.DestroyImmediate(obstacles[i].transform.gameObject);
        obstacles[i] = null;
        continue;
      }
      
      i++;
    }

  }

  public void checkTouchObstacle(BoxCollider2D refCollider, Action<ArenaLaneObstacle, string> onTouchSomething = null, string filter = "")
  {
    
    for (int i = 0; i < obstacles.Count; i++)
    {
      if (obstacles[i] == null) continue;

      ArenaLaneObstacle obs = obstacles[i];

      if (filter.Length > 0 && obs.name.Contains(filter)) continue;
      
      string collisionType = solveCollisionComparison(refCollider, obs);
      if(collisionType.Length > 0)
      {
        solveTouchedObstacle(obs, collisionType, onTouchSomething);
        obstacles[i] = null;
      }
    }
  }
  
  virtual protected void solveTouchedObstacle(ArenaLaneObstacle obstacle, string type, Action<ArenaLaneObstacle, string> onTouchSomething = null)
  {
    if (onTouchSomething != null) onTouchSomething(obstacle, type);
    GameObject.DestroyImmediate(obstacle.gameObject);
  }

  virtual protected string solveCollisionComparison(Collider2D collider, ArenaLaneObstacle obs)
  {
    if (collider.bounds.Intersects(obs.visibility.getSymbolBounds())) return "symbol";
    return "";
  }

  public bool positionTouching(Vector2 position) {
    float gap = 2f;

    if(position.y > transform.position.y - gap && 
      position.y < transform.position.y + gap)
    {
      return true;
    }
    return false;
  }

  public Transform getHeadPosition() {
    return transform.GetChild(0);
  }

  private void OnDrawGizmos()
  {
    Color col = Color.green;
    col.a = 0.1f;
    Gizmos.color = col;
    Gizmos.DrawCube(transform.position, new Vector3(100f, data.laneHeight, 1f));
  }
  
}

