using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour {
  
  AudioSource src;
  public AnimationCurve curve;

  public bool useFade = true;
  float fadeSpeed = 0.75f;

  void Start() {
    src = gameObject.GetComponent<AudioSource>();
  }

  public void play() {
    if(useFade) {
      src.volume = 0f;
      fadeSpeed = Mathf.Abs(fadeSpeed);
    }

    src.Play();
  }

  public void stop() {
    if(src.isPlaying) src.Stop();
  }

  public void fadeOut() {
    if (!useFade) return;

    fadeSpeed = Mathf.Abs(fadeSpeed) * -1f;
    if (!src.isPlaying) src.Play();
  }

  void Update() {
    if (src == null) return;
    if (!src.isPlaying) return;

    if(useFade) {
      //float target = fadeSpeed > 0 ? 1f : 0f;

      float vol = src.volume;
      vol += Time.deltaTime * fadeSpeed;
      src.volume = vol;
      //Mathf.MoveTowards(src.volume, target, Time.deltaTime * fadeSpeed);

      src.volume = vol;
    }

    //Debug.Log("vol ? "+src.volume + " / "+vol+" , target ? " + target+" , speed ? "+fadeSpeed);
  }

  public bool fadedOut() { 
    if (!useFade) return true;
    return src.volume <= 0f && fadeSpeed < 0;
  }

  static public MusicPlayer fetchPlayer(string name) {
    MusicPlayer[] players = GameObject.FindObjectsOfType<MusicPlayer>();

    for (int i = 0; i < players.Length; i++)
    {
      if (players[i].name.Contains(name)) return players[i];
    }
    
    return null;
  }
}
