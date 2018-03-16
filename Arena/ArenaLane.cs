using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

abstract public class ArenaLane : ArenaObject {

  [Header("to setup")]
  public DataLanes data;
  public Transform spawnOffsetReferencial;

  [Header("read only")]
  public DataLane lane;
  public DataLanePattern pattern;

  protected int laneIdx = 0;
  protected int patternIdx = 0;
  
  protected float timer = 0f;
  protected int obstacleIdx = 0;
  
  protected List<ArenaLaneObstacle> obstacles = new List<ArenaLaneObstacle>();

  protected override void build()
  {
    base.build();

    //timer = gameObject.AddComponent<Timer>();
    //timer.timeout += spawnTimeout;
  }

  public override void restart()
  {
    base.restart();
    //timer.setupAndStart(data.timer);
    //Debug.Log(name + " restart");

    //clean obstacles
    while (obstacles.Count > 0)
    {
      GameObject.DestroyImmediate(obstacles[0].gameObject);
      obstacles.RemoveAt(0);
    }

    nextLane();
  }
  
  protected void nextLane()
  {
    timer = 0f;

    obstacleIdx = 0;
    patternIdx = 0;
    
    if(laneIdx > data.lanes.Length-1)
    {
      Debug.LogWarning("end of lanes patterns design");
      Debug.LogWarning("selecting random lane (endless)");
      lane = data.lanes[Random.Range(0, data.lanes.Length)].lane;
    }
    else
    {
      lane = data.lanes[laneIdx].lane;
    }

    Debug.Log("next lane : " + lane.name);

    nextPattern();

    laneIdx++;
  }

  /* including current pattern speed */
  public float getLaneSolvedSpeed()
  {
    return lane.laneSpeed * pattern.patternFactorSpeed;
  }
  
  protected int getWinPatternCountOfLane(DataLane lane)
  {
    for (int i = 0; i < data.lanes.Length; i++)
    {
      if (data.lanes[i].lane == lane) return data.lanes[i].patternWinCount;
    }
    return 1;
  }

  protected void nextPattern()
  {
    timer = 0f;
    obstacleIdx = 0;

    if(patternIdx >= getWinPatternCountOfLane(lane))
    {
      nextLane();
      return;
    }

    if (lane.randomNextPattern)
    {
      pattern = getRandomPattern();
    }
    else
    {
      if(patternIdx > lane.patterns.Length - 1)
      {
        Debug.LogWarning("asking for too many win pattern in a non random setup of lane !");
        Debug.LogWarning("returning random pattern instead");
        pattern = getRandomPattern();
      }
      else
      {
        pattern = lane.patterns[patternIdx];
      }
    }

    Debug.Log("next pattern (win : "+patternIdx+") : " + pattern.name);
    patternIdx++;
  }

  protected void nextObstacle()
  {
    spawn(pattern.obstacles[obstacleIdx]);
    timer = 0f;

    //Debug.Log("obstacle : " + obstacleIdx);

    obstacleIdx++;
  }

  protected DataLanePattern getRandomPattern()
  {
    return lane.patterns[Random.Range(0, lane.patterns.Length)];
  }

  protected Sprite getRandomObstacle()
  {
    return pattern.obstacles[Random.Range(0, pattern.obstacles.Length)];
  }

  protected void spawn(Sprite spr)
  {
    if (lane.patterns.Length <= 0) return;
    
    GameObject obj = ResourceManager.getDuplicate(spr.name);

    if (obj == null)
    {
      Debug.LogError("no object " + spr.name + " found in resources");
      return;
    }
    
    //obj.transform.SetParent(transform);
    obj.transform.position = transform.position;

    ArenaLaneObstacle obs = obj.GetComponent<ArenaLaneObstacle>();
    obs.setupOnLane(this);
    obstacles.Add(obs);
  }

  public override void updateArena()
  {
    base.updateArena();

    updateSpawnPosition();
    updateObstacles();

    //Debug.Log(timer);
    
    //end buffer
    if(obstacleIdx >= pattern.obstacles.Length)
    {
      if(timer < lane.bufferEnd.bufferTime)
      {
        timer += Time.deltaTime;
        if(timer > lane.bufferEnd.bufferTime)
        {
          //Debug.Log("buffer end !");
          nextPattern();
        }
      }
      return;
    }

    //between obstacles
    if(timer < pattern.spawnTimer)
    {
      timer += Time.deltaTime;

      //Debug.Log(timer + "/ " + pattern.spawnTimer);
      if(timer >= pattern.spawnTimer)
      {
        //Debug.Log("spawned obs");
        nextObstacle();
      }
    }
  }

  protected void update_timer_spawn() {
    updateSpawnPosition();
    updateObstacles();
  }

  protected void updateSpawnPosition()
  {

    //make spawn offset based on a referencial
    if (spawnOffsetReferencial != null)
    {
      //Debug.Log(GameSpace.get().getWidth());

      float oosPosition = spawnOffsetReferencial.position.x + GameSpace.get().getWidth();
      Vector3 pos = transform.position;
      pos.x = oosPosition;
      //pos.y = spawnOffsetReferencial.position.y - 2f;
      transform.position = pos;
    }

  }

  protected void updateObstacles()
  {

    int i = 0;
    while (i < obstacles.Count)
    {

      //remove cleared
      if (obstacles[i] == null)
      {
        obstacles.RemoveAt(i);
        //Debug.Log("removed at " + i);
        continue;
      }

      //doing something else
      if (obstacles[i].canTranslate())
      {
        //translate
        obstacles[i].transform.Translate(Vector3.left * obstacles[i].getSpeed() * Time.deltaTime);

        obstacles[i].checkOutOfScreen();
      }

      if (obstacles[i] != null && obstacles[i].canBeCollidedWith())
      {
        //check for collision after everything is done moving
        solveCollision(obstacles[i]);
      }

      //if was not removed, step fwd in array
      if (obstacles[i] != null) i++;
    }

  }

  /* bridge to describe how to solve collision for each obstacle */
  abstract protected void solveCollision(ArenaLaneObstacle obs);
  
  public void checkTouchObstacle(BoxCollider2D refCollider, Action<ArenaLaneObstacle> onTouchSomething = null, string filter = "")
  {
    
    for (int i = 0; i < obstacles.Count; i++)
    {
      if (obstacles[i] == null) continue;

      ArenaLaneObstacle obs = obstacles[i];

      if (filter.Length > 0 && obs.name.Contains(filter)) continue;
      
      if(solveCollisionWithObstacle(refCollider, obs))
      {
        //tell asking collision checker that this object was touched
        if (onTouchSomething != null) onTouchSomething(obs);

        //remove from scene
        GameObject.DestroyImmediate(obs.gameObject);
        
        if(obs == null) obstacles[i] = null; //destroyed ?
      }
    }
  }
  
  virtual protected bool solveCollisionWithObstacle(Collider2D collider, ArenaLaneObstacle obs)
  {
    if (collider.bounds.Intersects(obs.visibility.getSymbolBounds())) return true;
    return false;
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
    if (lane == null) return;

    Color col = Color.green;
    col.a = 0.1f;
    Gizmos.color = col;
    Gizmos.DrawCube(transform.position, new Vector3(100f, lane.laneHeight, 1f));
  }
  
}

