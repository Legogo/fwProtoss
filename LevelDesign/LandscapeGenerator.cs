using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandscapeGenerator : ArenaObject {

  protected GameSpace gspace;

  public DataResourceAtlas atlas;
  public List<Transform> landscapeObjects;

  protected Vector2 currentGenerationPosition;
  protected Vector2 lastGenerationPosition;

  protected override void setup()
  {
    base.setup();
    gspace = GameSpace.get();
  }

  public override void restart()
  {
    base.restart();
    
    //fill screen
    //...
  }
  
  protected void update_generation()
  {
    currentGenerationPosition = Camera.main.transform.position;

    if(currentGenerationPosition != lastGenerationPosition)
    {
      solveGeneration();
    }

    lastGenerationPosition = currentGenerationPosition;
  }

  protected void solveGeneration()
  {
    Vector2 diff = currentGenerationPosition - lastGenerationPosition;


  }

  protected void spawnTimeout()
  {
    if (atlas.list.Length <= 0) return;

    Sprite spr = atlas.list[Random.Range(0, atlas.list.Length)];
    GameObject obj = ResourceManager.getDuplicate(spr.name);

    obj.transform.position = transform.position;

    landscapeObjects.Add(obj.transform);
  }

}
