using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundCarry : MonoBehaviour
{
  public DataSounds sounds;
  public AudioMixerGroup mixer;

  private void Awake()
  {
    for (int i = 0; i < sounds.clips.Length; i++)
    {
      AudioSource src = gameObject.AddComponent<AudioSource>();
      src.clip = sounds.clips[i];
      src.outputAudioMixerGroup = mixer;
    }
  }
}
