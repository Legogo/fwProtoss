using System.Collections;
using System;
using UnityEngine;

[CreateAssetMenu(menuName = "data/SpriteAnimationData", order = 100)]
public class SpriteAnimationData : ScriptableObject
{
  public Sprite[] frames;
  public SpriteTimelineStep[] timeline;

  [Serializable]
  public struct SpriteTimelineStep
  {
    public int frameIndex;
    public float frameTime;
  }
}
