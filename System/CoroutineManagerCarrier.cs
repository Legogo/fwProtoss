using UnityEngine;
using System.Collections;

public class CoroutineManagerCarrier : MonoBehaviour, DebugSelection.iDebugSelection
{
  string DebugSelection.iDebugSelection.toDebug() { return toString(); }

  protected string toString() { return CoroutineManager.get().toString(); }
}
