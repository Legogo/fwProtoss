using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// wrapper object to make a factory for a specific type
/// </summary>
abstract public class FactoryBase
{
    //List<FactoryObject> pool = new List<FactoryObject>();
    protected List<IFactoryObject> actives = new List<IFactoryObject>();
    List<IFactoryObject> inactives = new List<IFactoryObject>();

    System.Type _factoryTargetType;

	public FactoryBase()
	{
		_factoryTargetType = GetType();

        IndusReferenceMgr.injectTypes(new System.Type[] { _factoryTargetType });

		if (!Application.isPlaying) refresh();
	}

    public void refresh()
	{
        Debug.Log(getStamp() + " refresh");

        actives.Clear();
        inactives.Clear();

        //List<T> actives = getActives<T>();
        Object[] presents = (Object[])GameObject.FindObjectsOfType(_factoryTargetType);
        for (int i = 0; i < presents.Length; i++)
        {
            inject(presents[i] as IFactoryObject);
        }

        if (!Application.isPlaying)
        {
            Debug.Log($"[ed] x{actives.Count}");
        }
    }

    //abstract public System.Type getChildrenType();

    public bool hasCandidates() => actives.Count > 0 || inactives.Count > 0;
    public bool hasCandidates(int countCheck) => (actives.Count + inactives.Count) >= countCheck;

    public List<IFactoryObject> getActives()
    {
        return actives;
    }

    public List<T> getActives<T>() where T : IFactoryObject
    {
        List<T> tmp = new List<T>();
        for (int i = 0; i < actives.Count; i++)
        {
            T candid = (T)actives[i];
            if (candid == null) continue;
            tmp.Add(candid);
        }
		
        Debug.Log(typeof(T)+" ? candid = "+tmp.Count + " / active count = " + actives.Count);

		return tmp;
    }

    public IFactoryObject getRandomActive()
    {
        Debug.Assert(actives.Count > 0, GetType() + " can't return random one if active list is empty :: " + actives.Count + "/" + inactives.Count);

        return actives[Random.Range(0, actives.Count)];
    }
    public IFactoryObject getNextActive(IFactoryObject curr)
    {
        int idx = actives.IndexOf(curr);
        if (idx > -1)
        {
            if (idx + 1 < actives.Count) return actives[idx + 1];
            return actives[0]; // loop
        }

        Debug.LogError(curr + " is not in factory ?");

        return null;
    }

    /// <summary>
    /// générer un nouveau element dans le pool
    /// </summary>
    protected IFactoryObject create(string subType)
    {
        string path = System.IO.Path.Combine(getObjectPath(), subType);
        Object obj = Resources.Load(path);
        Debug.Assert(obj != null, $"{GetType()}&{_factoryTargetType} no object to load at path : " + path);

        obj = GameObject.Instantiate(obj);

        GameObject go = obj as GameObject;
        
        //Debug.Log("newly created object " + go.name, go);

        IFactoryObject candidate = go.GetComponent<IFactoryObject>();
        Debug.Assert(candidate != null, $"no candidate on {go} ?? generated object is not factory compatible", go);

        inactives.Add(candidate);

        //for refs list
        //IndusReferenceMgr.refreshGroupByType(factoryTargetType);
        //IndusReferenceMgr.injectObject(candidate);

        return candidate;
    }
    abstract protected string getObjectPath();

    /// <summary>
    /// demander a la factory de filer un element dispo
    /// subType est le nom du prefab dans le dossier correspondant
    /// </summary>
    public IFactoryObject extract(string subType)
    {
        //will add an item in inactive
        //and go on
        if (inactives.Count <= 0)
        {
            Debug.LogWarning(getStamp()+" extract:: recycling possible ? <b>nope</b> , creating a new one");
            create(subType);
        }

        IFactoryObject obj = null;

        for (int i = 0; i < inactives.Count; i++)
        {
            if(inactives[i].factoGetCandidateName() == subType)
            {
                obj = inactives[i];
            }
        }

        if(obj == null)
        {
            obj = create(subType);
        }

        inject(obj);

        //va se faire tout seul au setup()
        //obj.materialize();

        return obj;
    }

    public T extract<T>(string subType)
    {
        IFactoryObject icand = extract(subType);
        Component com = icand as Component;
        return com.GetComponent<T>();
    }

    /// <summary>
    /// indiquer a la factory qu'un objet a changé d'état de recyclage
    /// </summary>
    public void recycle(IFactoryObject candid)
    {
        Debug.Assert(actives.IndexOf(candid) > -1);
        actives.Remove(candid);

        Debug.Assert(inactives.IndexOf(candid) < 0);
        inactives.Add(candid);

        // move recycled object into facto scene
        MonoBehaviour comp = candid as MonoBehaviour;
        if (comp != null)
        {
            comp.transform.SetParent(null);

            /*
            //https://docs.unity3d.com/ScriptReference/SceneManagement.SceneManager.MoveGameObjectToScene.html
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(
                comp.gameObject,
                UnityEngine.SceneManagement.SceneManager.GetSceneByName(TinyConst.scene_resources_facto));
            */
        }

        Debug.Log(getStamp() + " :: recycle :: " + candid + " :: ↑" + actives.Count + "/ ↓" + inactives.Count);
    }

    /// <summary>
    /// quand un objet est déclaré comme utilisé par le systeme
    /// généralement cette méthode est appellé a la création d'un objet lié a la facto
    /// </summary>
    public void inject(IFactoryObject candid)
    {
        inactives.Remove(candid);

        if (actives.IndexOf(candid) < 0)
        {
            actives.Add(candid);
            Debug.Log(getStamp() + " :: inject :: " + candid + " :: ↑" + actives.Count + "/ ↓" + inactives.Count);
        }
    }

    /// <summary>
    /// called by a destroyed object
    /// </summary>
    public void destroy(IFactoryObject candid)
    {
        if (actives.IndexOf(candid) > -1) actives.Remove(candid);
        if (inactives.IndexOf(candid) > -1) inactives.Remove(candid);
    }

    public void recycleAll()
	{
        Debug.Log(getStamp() + " recycleAll");

        List<IFactoryObject> cands = new List<IFactoryObject>();
        cands.AddRange(actives);

		for (int i = 0; i < cands.Count; i++)
		{
            cands[i].factoRecycle();
		}

        Debug.Assert(actives.Count <= 0);
	}

    string getStamp() => "<color=#3333aa>" + GetType() + "</color>";

}

//public interface IFactory{}

public interface IFactoryObject : IIndusReference, ISaveSerializable
{

    string factoGetCandidateName();

    /// <summary>
    /// describe recycling process
    /// +must tell factory
    /// </summary>
    void factoRecycle();

    /// <summary>
    /// describe activation
    /// +must tell factory
    /// </summary>
    void factoMaterialize();

    //string serialize();
}

public interface ISaveSerializable
{
    object generateSerialData(); // generate an objet to be saved

    void mergeSerialData(object data); // use a deserialized object to get data

    //void mergeId(TinyIdentity datId);
}
