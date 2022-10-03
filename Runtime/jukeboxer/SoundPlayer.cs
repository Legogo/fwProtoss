using UnityEngine;
using UnityEngine.Audio;

public class SoundPlayer : MonoBehaviour {
  
  public AudioClip clip;
  public AudioMixerGroup mixerChannel;

  public Vector2 cooldownRange = new Vector2(0.05f, 0.15f);
  public float volume = 1f;

  AudioSource src;

  float cooldown = 0f;

  void Awake() {
    src = gameObject.AddComponent<AudioSource>();
    src.outputAudioMixerGroup = mixerChannel;

    cooldown = 0f;
  }

  void Update() {
    if(cooldown >= 0f) {
      cooldown -= Time.deltaTime;
    }
  }

  public void play() {
    //Debug.Log("play "+cooldown+" , "+useCooldown);
    if (cooldown > 0f) return;
    
    cooldown = Random.Range(cooldownRange.x, cooldownRange.y);
    
    //Debug.Log("PLAY !");
    src.PlayOneShot(clip, volume);
  }
  
}
