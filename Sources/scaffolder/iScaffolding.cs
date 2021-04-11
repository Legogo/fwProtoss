using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scaffolder
{
  static class Scaffolding
  {
    static public bool loading = false;
  }

  abstract public class ScaffGround : MonoBehaviour
  {
    private void Awake()
    {
      build();
    }
    virtual protected void build()
    { }

    IEnumerator Start()
    {
      enabled = false;
      
      while (Scaffolding.loading) yield return null;

      setupEarly();
      yield return null;
      setup();
      yield return null;
      setupLate();
      yield return null;
      enabled = true;
    }

    virtual protected void setupEarly()
    { }
    virtual protected void setup()
    { }
    virtual protected void setupLate()
    { }

    private void OnDestroy()
    {
      destroy();
    }
    virtual protected void destroy()
    { }

  }

  /// <summary>
  /// entered a level / arena
  /// </summary>
  public interface iScaffIngame
  {
    void ingameSetup();
    void ingameRestart();
    
    void ingameUpdate();
    void ingameUpdateLate();

    void ingameEnd();
  }

  public interface iScaffDebug
  {
    string stringify();
  }
}
