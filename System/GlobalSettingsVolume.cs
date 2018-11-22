using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class GlobalSettingsVolume {

  public const string ppref_sound_master_volume = "ppref_sound_volume_master";
  public const string ppref_sound_music_volume = "ppref_sound_volume_music";
  public const string ppref_sound_fx_volume = "ppref_sound_volume_fx";
  public const string ppref_sound_muted = "ppref_sound_muted";

  static public void setupVolumes()
  {
    applyMasterVolume();
    applyMusicVolume();
    applyFxVolume();
    //Debug.Log("~SettingsManager~ setup volumes");

    PlayerPrefs.Save();
  }

  static public void applyMasterVolume() { applyVolume("global", ppref_sound_master_volume, PlayerPrefs.GetInt(ppref_sound_muted, 0) == 0 ? 0f : -80f); }
  static public void applyFxVolume() { applyVolume("fx", ppref_sound_fx_volume); }
  static public void applyMusicVolume() { applyVolume("music", ppref_sound_music_volume); }

  static public AnimationCurve volumeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
  static public float transformVolumeSliderValue(float input)
  {
    //Debug.Log("input "+input+" => "+volumeCurve.Evaluate(input));
    return Mathf.Lerp(-80f, 0f, volumeCurve.Evaluate(input));
  }

  static public void applyVolume(string category, string ppref_const, float boost = 0f)
  {
    float volume = PlayerPrefs.GetFloat(ppref_const, 1f);
    EngineManager em = GameObject.FindObjectOfType<EngineManager>();
    if (em == null) return;
    if (em.mixer == null)
    {
      Debug.LogWarning("no mixer ?");
      return;
    }
    volume = transformVolumeSliderValue(volume);
    em.mixer.SetFloat(category, volume + boost);
  }

}
