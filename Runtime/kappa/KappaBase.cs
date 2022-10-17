using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using fwp.engine.scaffolder;

/// <summary>
/// must not be a region object, linked to brain, brain is responsible for everything
/// </summary>
abstract public class KappaBase : ScaffGround
{
    protected BrainBase _brain;

    List<MonoBehaviour> freezeQueue = new List<MonoBehaviour>();

    protected override void build()
    {
        base.build();

        checkSub(); // solve brain & sub
    }

    /// <summary>
    /// called on region change by brain
    /// </summary>
    virtual public void reset()
    {
        //Debug.Log(getStamp() + " RESET");
    }

    /// <summary>
    /// this can be called during build() of a getcapa fetch is called
    /// </summary>
    public void brainReady(BrainBase brain)
    {
        //dans le cas d'un objet généré au runtime y a besoin
        checkSub();

        Debug.Assert(brain == _brain, "pas le bon brain");

        //Debug.Log(brain, brain);
        //Debug.Log(_brain, _brain);
    }

    public void freeze(MonoBehaviour locker)
	{
        if (!freezeQueue.Contains(locker))
        {
            freezeQueue.Add(locker);
            onFreeze(true);
        }
	}
    public void unfreeze(MonoBehaviour locker)
	{
        if(freezeQueue.Contains(locker))
        {
            freezeQueue.Remove(locker);
            onFreeze(isFreezed());
        }
	}
    public bool isFreezed() => freezeQueue.Count > 0;

    /// <summary>
    /// only called when an actual change is made
    /// multiple subscription of the same object won't trigger it again
    /// </summary>
    /// <param name="freezed"></param>
    virtual protected void onFreeze(bool freezed)
    { }

    /// <summary>
    /// solve brain
    /// &
    /// sub to brain
    /// </summary>
    virtual protected void checkSub()
    {
        // pas besoin de refaire
        if (_brain != null) return;

        _brain = transform.GetComponentInParent<BrainBase>();
        Debug.Assert(_brain != null, this.name+" is brainless ? (prefab might not have assigned brain)", this);

        _brain.subKappa(this);

    }

    public void brainUpdate(float dt)
    {
        updateKappa(dt);
    }

    public void brainUpdateFixed(float fixedDeltaTime)
    {
        updateFixedKappa(fixedDeltaTime);
    }

    /// <summary>
    /// only runs if not freezed
    /// </summary>
    protected virtual void updateKappa(float dt)
    { }

    protected virtual void updateFixedKappa(float fixedDeltaTime)
    { }

    public BrainBase getParentBrain() => _brain;

    public bool isFromSameBrain(KappaBase kap)
	{
        return kap.getParentBrain() == getParentBrain();
	}

    public void setActive(bool active)
    {
        //Debug.Log("active ? " + active);
        //gameObject.SetActive(active);
        enabled = active;
    }

    public bool isActive() => gameObject.activeSelf;

    virtual public bool canBeUpdated()
    {
        if (!enabled) return false;
        if (!gameObject.activeSelf) return false;

        if (!isReady()) return false;

        if (isFreezed()) return false;

        return gameObject.activeInHierarchy && enabled;
    }

    /// <summary>
    /// interruption
    /// </summary>
    virtual public void recycle()
	{
        reset();
	}

    public override string getStamp()
	{
        return base.getStamp() + $" <b>{_brain.name}/{name}</b>";
	}

    public override string stringify()
    {
        string output = base.stringify();

        if (!canBeUpdated())
        {
            output += "\n NO UPDATE";
            output += "\n  ready ? " + isReady();
            output += "\n  freeze ? " + isFreezed();
        }
        else
        {
            output += "\n " + Time.frameCount;
        }

        return output;
    }
}
