using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class LocalizationManager : MonoBehaviour {

  abstract public string getLocalizationOfId(string id);

  static public string getLocaOfId(string id)
  {
    LocalizationManager lm = qh.gc<LocalizationManager>();
    return lm.getLocalizationOfId(id);
  }
}
