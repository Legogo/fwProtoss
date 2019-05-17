using UnityEngine;
using System.Collections;

public class CoroutineManagerCarrier : MonoBehaviour
{
  protected string toString() { return CoroutineManager.get().toString(); }
}
