using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using fwp.engine.scaffolder;

/// <summary>
/// objet qui a des capacités
/// il connait la liste
/// il peut fournir des capacités a ceux qui demandent
/// </summary>

abstract public class BrainBase : ScaffGroundUpdate
{
    const string STATE_PREFIX = "s_";

    protected List<KappaBase> kappas = new List<KappaBase>();
    
    bool _existOnStartup = false;

    protected override void build()
    {
        base.build();

        _existOnStartup = Time.frameCount < 5;
    }

    protected override void setup()
    {
        base.setup();

        for (int i = 0; i < kappas.Count; i++)
        {
            kappas[i].brainReady(this);
        }

        if (_existOnStartup) setupExistOnStartup();
    }

    protected override void setupLate()
    {
        base.setupLate();

        resetKappas();
    }

    virtual protected void resetKappas()
    {
        for (int i = 0; i < kappas.Count; i++)
        {
            kappas[i].reset();
        }
    }

    /// <summary>
    /// pour pouvoir faire un truc spé quand l'objet est déjà présent dans la scène
    /// quand on démarre le projet
    /// </summary>
    virtual protected void setupExistOnStartup()
    { }

    /// <summary>
    /// show all rig
    /// </summary>
    public void doMaterialize()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// hide visual
    /// </summary>
    public void doDematerialize()
    {
        gameObject.SetActive(false);
    }

    sealed override protected void scaffUpdate(float dt)
    {
        brainUpdate();

        for (int i = 0; i < kappas.Count; i++)
        {
            if (!kappas[i].canBeUpdated()) continue;
            kappas[i].brainUpdate(dt);
        }
    }

    virtual protected void brainUpdate()
    { }

    public void subKappa(KappaBase capa)
    {
        if (kappas.IndexOf(capa) > 0) return;
        kappas.Add(capa);
    }

    public void unsubKappa(KappaBase capa)
    {
        if (kappas.IndexOf(capa) < 0) return;
        kappas.Remove(capa);
    }

    /// <summary>
    /// called by any child kappa that need to bubble an event
    /// </summary>
    virtual public void kappaEvent(KappaBase kappa)
    { }
    
    /// <summary>
    /// this will only check for exiting/fetched kappas (present in buff array)
    /// during setup phase it's best to use "safe" to avoid racing conditions
    /// </summary>
    public T getCapacity<T>() where T : KappaBase
    {
        for (int i = 0; i < kappas.Count; i++)
        {
            if (kappas[i] as T) return kappas[i] as T;
        }
        return null;
    }

    /// <summary>
    /// force refresh kappas array if not found
    /// less opti, fine for boot
    /// </summary>
    public T getCapacitySafe<T>() where T : KappaBase
    {
        T tar = getCapacity<T>();

        //force subscribe
        if(tar == null)
        {
            tar = GetComponentInChildren<T>();
            if (tar != null) tar.brainReady(this);
        }

        //safe
        if (tar == null)
        {
            Debug.LogError(name + " has no capac " + typeof(T) + " (dont ask for capa in build())", transform);
        }

        return tar;
    }

    public List<KappaBase> getAllKappas()
	{
        List<KappaBase> kaps = new List<KappaBase>();
        kaps.AddRange(transform.GetComponentsInChildren<KappaBase>());
        return kaps;
	}

    public virtual void onSelected()
    {
        //TinyQueries.camera.setFollowTarget(transform);
    }

    public virtual void onUnselected()
    {
        //throw new System.NotImplementedException();
    }

    virtual public bool isSelectable() => gameObject.activeSelf;

    virtual public bool isSelected() => false;

    static public Transform toggleChangeling(MonoBehaviour parent, string changelingName)
    {
        //toggle transforms
        // search by name (ToLower)
        Transform curPivot = parent.transform.Find($"{STATE_PREFIX}{changelingName.ToLower()}");

        //Debug.Assert(curPivot != null);

        if(curPivot == null)
		{
            //Debug.LogWarning($"changeling for {parent.name}, state <b>{changelingName}</b> doesn't have matching child");
            return null;
        }

        foreach (Transform child in parent.transform)
        {
            // only states with prefix
            if (!child.name.StartsWith(STATE_PREFIX)) continue;

            if (child == curPivot)
                child.gameObject.SetActive(true);
            else
                child.gameObject.SetActive(false);
        }

        return curPivot;
    }

    public void attach(Transform tarParent, bool align = true)
	{
        transform.SetParent(tarParent);

        //reset to pivot
        if (tarParent != null && align)
        {
            transform.localPosition = Vector3.zero;
        }
    }

    public Vector2 Position
    {
        get { return transform.position; }
    }

}
