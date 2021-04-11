using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace brainer
{
  /// <summary>
  /// Le bridge qui gère et update les capacities
  /// </summary>
  public abstract class BrainerLogics
  {
    //must manage coroutines
    public MonoBehaviour owner; // get
    public Transform tr; // pivot ?

    //capacities will subscribe to this List on their constructor
    protected List<BrainerLogicCapacity> capacities = new List<BrainerLogicCapacity>();

    public BrainerLogics(MonoBehaviour owner)
    {
      this.owner = owner;
      tr = owner.transform;

      //fetch all capac
      capacities.Clear();
      capacities.AddRange(owner.GetComponentsInChildren<BrainerLogicCapacity>());

      //setup
      for (int i = 0; i < capacities.Count; i++)
      {
        capacities[i].assign(this);
      }
    }

    virtual public void ingameSetup()
    {
      setupCapacities();
    }

    public void ingameRestart()
    {
      for (int i = 0; i < capacities.Count; i++) capacities[i].restartCapacity();
    }

    //public Transform getBrainTransform() => owner.transform;

    /* after scenes load */
    protected void setupCapacities()
    {
      //Debug.Log(GetType() + " , "+ name+" , setup capacs !");
      for (int i = 0; i < capacities.Count; i++) capacities[i].setupCapacityEarly();
      for (int i = 0; i < capacities.Count; i++) capacities[i].setupCapacity();
    }

    virtual public void clean()
    {
      for (int i = 0; i < capacities.Count; i++) capacities[i].clean();
    }

    virtual protected void update()
    {
      for (int i = 0; i < capacities.Count; i++)
      {
        capacities[i].updateCapacity();
      }
    }

    virtual protected void updateLate()
    {
      for (int i = 0; i < capacities.Count; i++)
      {
        capacities[i].updateCapacityLate();
      }
    }

    public void subCapacity(BrainerLogicCapacity capa)
    {
      capacities.Add(capa);
    }

    public void unsubCapacity(BrainerLogicCapacity capa)
    {
      capacities.Remove(capa);
    }

    public T getCapacity<T>() where T : BrainerLogicCapacity
    {
      return (T)capacities.FirstOrDefault(x => x != null && typeof(T).IsAssignableFrom(x.GetType()));
    }

    virtual public void ingameUpdate()
    { }

    virtual public void ingameUpdateLate()
    { }

    virtual public void ingameEnd()
    { }

    /*
    public void forceWithinBounds(Rect boundsClamp)
    {
      if (visibility == null) Debug.LogWarning("asking for bounds clamping but no visible module");

      Rect localRec = visibility.getWorldBounds();
      float gap = 0f;

      //Debug.Log(localRec);
      //Debug.Log(boundsClamp);

      gap = boundsClamp.xMax - localRec.xMax;
      if (gap < 0f) transform.position += Vector3.right * gap;

      gap = boundsClamp.xMin - localRec.xMin;
      //Debug.Log(boundsClamp.xMin + " - " + localRec.xMin + " = xmin " + gap);
      if (gap > 0f) transform.position += Vector3.right * gap;

      gap = boundsClamp.yMax - localRec.yMax;
      //Debug.Log(boundsClamp.yMax+" - "+localRec.yMax+" = ymax " + gap);
      if (gap < 0f) transform.position += Vector3.up * gap;

      gap = boundsClamp.yMin - localRec.yMin;
      //Debug.Log(boundsClamp.yMin + " - " + localRec.yMin + " = ymin " + gap);
      if (gap > 0f) transform.position += Vector3.up * gap;

    }
    */

  }

}
