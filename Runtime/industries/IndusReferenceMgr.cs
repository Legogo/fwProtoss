using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// facebook wrapper
/// need to specify compatible types
/// </summary>
static public class IndusReferenceMgr
{
    /// <summary>
    /// it knows EVERYBODY
    /// </summary>
    static private Dictionary<Type, List<IIndusReference>> facebook = new Dictionary<Type, List<IIndusReference>>();

    static private Type[] possibleTypes = null;

    static public void injectTypes(Type[] types)
    {
        if (possibleTypes == null) possibleTypes = new Type[0];

        List<Type> output = new List<Type>();
        for (int i = 0; i < types.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < possibleTypes.Length; j++)
            {
                if (found) continue;
                if (types[i] == possibleTypes[j]) found = true;
            }
            if (!found) output.Add(types[i]);
        }

        possibleTypes = output.ToArray();
    }

    /// <summary>
    /// called by tiny booter
    /// </summary>
    static public void boot()
    {
        facebook.Clear();

        refreshAll(possibleTypes);
    }

    static public void edRefresh()
    {
        Debug.LogWarning("editor refresh of facebook");
        boot(); // ed refresh
    }

    static private Type getTypeByDicoIndex(int idx)
    {
        int i = 0;
        foreach (var kp in facebook)
        {
            if (i == idx) return kp.Key;
        }
        return null;
    }

    static public bool hasAnyType() => facebook.Count > 0;

    static public Type[] getAllTypes()
    {
        List<Type> output = new List<Type>();
        foreach (var kp in facebook)
        {
            output.Add(kp.Key);
        }
        return output.ToArray();
    }

    static private bool hasGroupOfType(Type tar)
    {
        foreach (var kp in facebook)
        {
            //Debug.Log(kp.Key + " vs " + tar);
            //if (kp.Key.GetType().IsAssignableFrom(tar)) return true;
            if (tar.IsAssignableFrom(kp.Key)) return true;
        }
        return false;
    }

    static private bool hasGroupType<T>()
    {
        foreach (var kp in facebook)
        {
            if (typeof(T).IsAssignableFrom(kp.Key)) return true;
            //if (kp.Key.GetType() == typeof(T)) return true;
        }
        return false;
    }

    /// <summary>
    /// get all mono and inject all object of given type into facebook
    /// </summary>
    /// <param name="tar"></param>
    static public void refreshGroupByType(Type tar)
    {
        //Debug.Log($"~facto: called refreshing of indus refs <{tar}>");

        List<IIndusReference> output = new List<IIndusReference>();

        MonoBehaviour[] monos = GameObject.FindObjectsOfType<MonoBehaviour>();
        for (int i = 0; i < monos.Length; i++)
        {
            if (tar.IsAssignableFrom(monos[i].GetType()))
            {
                IIndusReference iref = monos[i] as IIndusReference;
                output.Add(iref);
            }
        }

        injectType(tar, output);
    }

    /// <summary>
    /// force refersh all of given types
    /// </summary>
    static public void refreshAll(Type[] tars)
    {
        if (tars == null) return;

        for (int i = 0; i < tars.Length; i++)
        {
            refreshGroupByType(tars[i]);
        }
    }

    /// <summary>
    /// refresh all existing
    /// </summary>
    static public void refreshAll()
    {
        Type[] typs = getAllTypes();
        for (int i = 0; i < typs.Length; i++)
        {
            //refreshGroupByType(getTypeByDicoIndex(i));
            refreshGroupByType(typs[i]);
        }
    }

    /// <summary>
    /// faaat at runtime
    /// </summary>
    static public void refreshGroup<T>() where T : MonoBehaviour
    {
        List<IIndusReference> output = new List<IIndusReference>();

        MonoBehaviour[] monos = GameObject.FindObjectsOfType<MonoBehaviour>(false);
        for (int i = 0; i < monos.Length; i++)
        {
            T elmt = monos[i] as T;
            if (elmt == null) continue;
            IIndusReference iref = elmt as IIndusReference;
            if (iref == null) continue;
            output.Add(iref);
        }

        if (!hasGroupType<T>())
        {
            facebook.Add(typeof(T), output);
        }
        else
        {
            facebook[typeof(T)] = output;
        }

        Debug.Log("~facto: refreshed x" + output.Count + " elmts of type " + typeof(T));
    }

    static public void injectObject(IIndusReference target)
    {
        Debug.Assert(target != null);

        if (possibleTypes == null)
        {
            Debug.LogWarning("can't inject " + target);
            return;
        }

        /*
        for (int i = 0; i < possibleTypes.Length; i++)
        {
            //if(target.GetType().IsAssignableFrom(possibleTypes[i]))
            if (possibleTypes[i].IsAssignableFrom(target.GetType()))
            {
                refreshGroupByType(possibleTypes[i]);
            }
        }
        */

        Type typ = target.GetType();

        if (!facebook.ContainsKey(typ))
        {
            Debug.LogWarning("can't add " + target + " to facebook, type is not declared");
            return;
        }

        if (facebook[typ].IndexOf(target) < 0)
        {
            facebook[typ].Add(target);
            Debug.Log($"facebook[{typ}] x{facebook[typ].Count}");
        }
    }

    /// <summary>
    /// add a specific type and its solved list to facebook
    /// </summary>
    static private void injectType(Type tar, List<IIndusReference> list)
    {

        if (!hasGroupOfType(tar))
        {
            Debug.Log($"indus:adding new group to facebook : <b>{tar}</b> (new type list count x{list.Count})");
            facebook.Add(tar, list);
        }
        else
        {
            Debug.Log("indus:override content for typ " + tar + " (x" + list.Count + ")");
            facebook[tar] = list;
        }

    }

    static public List<IIndusReference> getGroupByType(Type tar)
    {
        List<IIndusReference> output = new List<IIndusReference>();
        foreach (var kp in facebook)
        {
            if (tar == kp.Key) return kp.Value;
        }
        return output;
    }

    static public List<T> getGroup<T>() where T : IIndusReference
    {
        List<T> output = new List<T>();

        //output.AddRange(getGroupByType(typeof(T)));

        bool _checkFoundType = false;

        foreach (var kp in facebook)
        {
            //Debug.Log(kp.Key + " VS " + typeof(T));

            //if (typeof(T).IsAssignableFrom(kp.Key.GetType()))
            //if(kp.Key.GetType().IsAssignableFrom(typeof(T)))
            if (kp.Key == typeof(T))
            {
                _checkFoundType = true;

                for (int i = 0; i < kp.Value.Count; i++)
                {
                    //output.AddRange(kp.Value[i]);
                    if (kp.Value[i] != null)
                    {
                        // filter disabled gameobjects
                        MonoBehaviour mono = kp.Value[i] as MonoBehaviour;
                        if (mono != null)
                        {
                            if (!mono.gameObject.activeSelf) continue;
                        }

                        output.Add((T)kp.Value[i]);
                    }
                }
            }
        }

        if (!_checkFoundType)
        {
            Debug.LogWarning($"didn't find type:{typeof(T)} in facebook (out of x{facebook.Count})");
        }

        return output;
    }

    static public MonoBehaviour getClosestToPosition(Type tar, Vector2 position)
    {
        List<IIndusReference> refs = getGroupByType(tar);
        IIndusReference closest = null;
        float min = Mathf.Infinity;
        float dst;

        for (int i = 0; i < refs.Count; i++)
        {
            MonoBehaviour mono = refs[i] as MonoBehaviour;
            if (mono == null) continue;

            dst = Vector2.Distance(mono.transform.position, position);

            if (dst < min)
            {
                min = dst;
                closest = mono as IIndusReference;
            }
        }

        return closest as MonoBehaviour;
    }
}

public interface IIndusReference
{

}