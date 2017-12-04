using UnityEngine;
using System.Collections;

public class CoroutineManagerCarrier : MonoBehaviour, Interfaces.IDebugSelection
{
  string Interfaces.IDebugSelection.toStringDebug() { return toString(); }

  protected string toString() { return CoroutineManager.get().toString(); }
}
