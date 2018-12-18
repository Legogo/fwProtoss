using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class HalperLocalization {

  private const string ppref_lang = "ppref_lang";

  public enum LocalizationLanguage
  {
    en,fr
  }
  
  static public int getLangIndex(LocalizationLanguage defaultLang = LocalizationLanguage.en)
  {
    return PlayerPrefs.GetInt(ppref_lang, (int)defaultLang); // usually default is en
  }
  static public void setLangIndex(LocalizationLanguage lang)
  {
    PlayerPrefs.SetInt(ppref_lang, (int)lang);
  }

}
