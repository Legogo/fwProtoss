using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// meant to be setup as
/// Master (global)
///  L fx
///  L music
/// </summary>

static public class GlobalSettingsVolume {

  public const string ppref_sound_volume_prefix = "ppref_sound_volume_";
  public const string ppref_sound_mute = "ppref_sound_volume_mute";
  public enum VolumeCategory { global, fx, music };

  static public void updateVolumes()
  {
    Debug.Log("updating volumes");
    updateVolume(VolumeCategory.global);
    updateVolume(VolumeCategory.fx);
    updateVolume(VolumeCategory.music);
  }

  static public bool isMuted() { return PlayerPrefs.GetInt(ppref_sound_mute, 0) == 1; }
  static public bool muteVolume(bool mute)
  {
    PlayerPrefs.SetInt(ppref_sound_mute, mute ? 1 : 0);
    PlayerPrefs.Save();

    Debug.Log("sound is now muted ? " + mute);

    updateVolume(VolumeCategory.global); // apply new mute state to Master

    return mute;
  }
  
  static public void setVolume(VolumeCategory category, float volume)
  {
    //raw volume [0,1], no transformation
    volume = Mathf.Clamp01(volume);
    PlayerPrefs.SetFloat(ppref_sound_volume_prefix + category, volume);

    Debug.Log("mixer group : " + category + " volume : " + volume);
  }

  static public AnimationCurve volumeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);


  static public void updateVolume(VolumeCategory category) { updateVolume(category.ToString()); }

  /// <summary>
  /// apply volume stored in ppref to a exposed float of the EM mixer
  /// </summary>
  /// <param name="category"></param>
  /// <param name="ppref_const"></param>
  /// <param name="boost"></param>
  static public void updateVolume(string category)
  {
    EngineManager em = EngineManager.get();
    if (em == null) return;
    if (em.mixer == null)
    {
      Debug.LogWarning("no mixer ?");
      return;
    }

    //mute only applied to global (Master), parent of fx/music
    float muteBoost = 0f;
    if (category == VolumeCategory.global.ToString())
    {
      muteBoost = isMuted() ? -80f : 0f;
    }
    
    float rawVolume = PlayerPrefs.GetFloat(ppref_sound_volume_prefix + category, 1f); // [0f,1f]
    float evalVolume = Mathf.Lerp(-80f, 0f, volumeCurve.Evaluate(rawVolume)); // transform volume
    em.mixer.SetFloat(category, evalVolume + muteBoost);
  }

}
