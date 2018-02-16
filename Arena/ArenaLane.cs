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
  }

  protected void spawnTimeout()
  {
    if (data.obstacles.Length <= 0) return;

    Sprite spr = data.obstacles[Random.Range(0, data.obstacles.Length)];
    GameObject obj = ResourceManager.getDuplicate(spr.name);

    //Debug.Log(spr.name);
    //Debug.Log(obj, obj);

    //obj.transform.SetParent(transform);
    obj.transform.position = transform.position;

    ArenaLaneObstacle obs = new ArenaLaneObstacle();
    obs.carry = obj.transform;
    obs.speed = data.obstacleTranslateSpeed;
    if(data.obstacleSpeedRange.sqrMagnitude != 0f)
    {
      obs.speed = Random.Range(data.obstacleSpeedRange.x, data.obstacleSpeedRange.y);
    }

    obs.render = obj.GetComponent<SpriteRenderer>();

    if(obj.transform.childCount > 0)
    {
      obs.timing_bad = obj.transform.GetChild(0).GetComponent<BoxCollider2D>();
      obs.timing_good = obj.transform.GetChild(1).GetComponent<BoxCollider2D>();
    }

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
      obstacles[i].carry.Translate(Vector3.left * obstacles[i].speed * Time.deltaTime);

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

  public void checkTouchObstacle(BoxCollider2D box, Action<ArenaLaneObstacle, ArenaLaneObstacleCollision> onTouchSomething = null)
  {
    
    for (int i = 0; i < obstacles.Count; i++)
    {
      if (obstacles[i] == null) continue;

      ArenaLaneObstacle obs = obstacles[i];
      ArenaLaneObstacleCollision collisionType = ArenaLaneObstacleCollision.NONE;

      //bad
      if (obstacles[i].timing_bad != null && box.bounds.Intersects(obs.timing_bad.bounds))
      {
        collisionType = ArenaLaneObstacleCollision.BAD;
      }

      //good
      if(obs.timing_good != null && box.bounds.Intersects(obs.timing_good.bounds))
      {
        collisionType = ArenaLaneObstacleCollision.GOOD;
      }
      
      //if not touched other, check intersects with visual
      if(collisionType == ArenaLaneObstacleCollision.NONE)
      {
        if (box.bounds.Intersects(obs.render.bounds)) collisionType = ArenaLaneObstacleCollision.TOUCHED;
      }

      if (collisionType != ArenaLaneObstacleCollision.NONE) {
        solveTouchedObstacle(obs, collisionType, onTouchSomething);
        obstacles[i] = null;
      }
    }
  }
  
  virtual protected void solveTouchedObstacle(ArenaLaneObstacle obstacle, ArenaLaneObstacleCollision collisionType, Action<ArenaLaneObstacle, ArenaLaneObstacleCollision> onTouchSomething = null)
  {
    if (onTouchSomething != null) onTouchSomething(obstacle, collisionType);
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

public enum ArenaLaneObstacleCollision { NONE, TOUCHED, BAD, GOOD };

public class ArenaLaneObstacle
{
  public Transform carry;
  public float speed;
  public SpriteRenderer render;
  public BoxCollider2D timing_bad;
  public BoxCollider2D timing_good;
}