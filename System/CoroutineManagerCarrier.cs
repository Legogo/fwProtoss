using UnityEngine;
using System.Collections;

public class CoroutineManagerCarrier : MonoBehaviour, Interfaces.IDebugSelection
{
  string Interfaces.IDebugSelection.iString() { return toString(); }

  protected string toString() { return CoroutineManager.get().toString(); }
}
