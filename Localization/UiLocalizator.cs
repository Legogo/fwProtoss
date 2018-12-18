using System.Collections;
using UnityEngine.UI;
using UnityEngine;

public class UiLocalizator : MonoBehaviour {
  
  public string id = "";
  public bool displayOnSetup = true;

  Text txt;

  private void Awake()
  {

    txt = GetComponent<Text>();
    if (txt == null) txt = GetComponentInChildren<Text>();

    if (txt == null) Debug.LogError("localization need Text");

  }

  public void event_update_fields(HalperLocalization.LocalizationLanguage newLang)
  {
    if (txt == null) return;
    
    txt.text = LocalizationManager.getLocaOfId(id);
    if (displayOnSetup && !txt.enabled) txt.enabled = true;

#if UNITY_EDITOR
    Debug.Log("Localiz | "+ id + " -> " + txt.text, txt.transform);
#endif
  }

  public void event_change_lang(HalperLocalization.LocalizationLanguage newLang)
  {
    event_update_fields(newLang);
  }

  static public void callRefreshFields()
  {
    UiLocalizator[] uil = qh.gcs<UiLocalizator>();
    for (int i = 0; i < uil.Length; i++)
    {
      uil[i].event_update_fields(getCurrentLang());
    }
  }

  static public void setNewLanguage(HalperLocalization.LocalizationLanguage lang)
  {
    HalperLocalization.setLangIndex(lang);

    UiLocalizator[] uil = qh.gcs<UiLocalizator>();
    for (int i = 0; i < uil.Length; i++)
    {
      uil[i].event_change_lang(lang);
    }
  }

  static public HalperLocalization.LocalizationLanguage getCurrentLang()
  {
    return (HalperLocalization.LocalizationLanguage)HalperLocalization.getLangIndex();
  }

  static public int getDefaultLangIndex() { return (int)HalperLocalization.LocalizationLanguage.en; }
  
}
