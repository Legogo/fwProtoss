using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class ArenaLane : ArenaObject {
  
  public DataLane data;
  protected Timer timer;

  public bool snapToCameraOnStartup;

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

    if (snapToCameraOnStartup)
    {
      transform.SetParent(Camera.main.transform);
    }
    //Debug.Log(name + " restart");
  }

  protected void spawnTimeout()
  {
    if (data.obstacles.Length <= 0) return;

    Sprite spr = data.obstacles[Random.Range(0, data.obstacles.Length)];
    GameObject obj = ResourceManager.getDuplicate(spr.name);

    //Debug.Log(spr.name);
    //Debug.Log(obj, obj);

    obj.transform.SetParent(transform);
    obj.transform.localPosition = Vector3.zero;

    ArenaLaneObstacle obs = new ArenaLaneObstacle();
    obs.carry = obj.transform;
    obs.speed = data.obstacleTranslateSpeed;
    if(data.obstacleSpeedRange.sqrMagnitude != 0f)
    {
      obs.speed = Random.Range(data.obstacleSpeedRange.x, data.obstacleSpeedRange.y);
    }
    obs.render = obj.GetComponent<SpriteRenderer>();

    obstacles.Add(obs);
  }
  
  public override void updateArena()
  {
    base.updateArena();

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
      obstacles[i].carry.Translate(Vector3.left * data.obstacleTranslateSpeed * Time.deltaTime);

      //remove on ooscreen
      if (obstacles[i].carry.position.x < -12f)
      {
        GameObject.DestroyImmediate(obstacles[i].carry.gameObject);
        obstacles[i] = null;
        continue;
      }
      
      i++;
    }

  }

  public void checkTouchObstacle(BoxCollider2D box, Action<ArenaLaneObstacle> onTouchSomething = null)
  {
    for (int i = 0; i < obstacles.Count; i++)
    {
      if (obstacles[i] == null) continue;
      if (box.bounds.Intersects(obstacles[i].render.bounds))
      {
        solveTouchedObstacle(obstacles[i], onTouchSomething);
        obstacles[i] = null;
      }
    }
  }

  public void checkTouchObstacle(ArenaLaneObstacle target, Action<ArenaLaneObstacle> onTouchSomething = null)
  {
    for (int i = 0; i < obstacles.Count; i++)
    {
      if (obstacles[i] == null) continue;
      if (Vector2.Distance(obstacles[i].carry.position, target.carry.position) < 2f)
      {
        solveTouchedObstacle(obstacles[i], onTouchSomething);
        obstacles[i] = null;
      }
    }
  }

  virtual protected void solveTouchedObstacle(ArenaLaneObstacle obstacle, Action<ArenaLaneObstacle> onTouchSomething = null)
  {
    if (onTouchSomething != null) onTouchSomething(obstacle);
    GameObject.DestroyImmediate(obstacle.carry.gameObject);
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

public class ArenaLaneObstacle
{
  public Transform carry;
  public float speed;
  public SpriteRenderer render;
}