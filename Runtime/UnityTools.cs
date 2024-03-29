﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UnityTools
{

    static public T getManager<T>(string nm, bool dontDestroy = false) where T : MonoBehaviour
    {
        GameObject obj = GameObject.Find(nm);
        T tmp = null;
        if (obj != null)
        {
            tmp = obj.GetComponent<T>();
        }

        if (tmp != null) return tmp;

        if (obj == null)
        {
            obj = new GameObject(nm, typeof(T));
            tmp = obj.GetComponent<T>();
        }
        else tmp = obj.AddComponent<T>();

        if (dontDestroy) GameObject.DontDestroyOnLoad(tmp);

        return tmp;
    }

    static public GameObject getGameObject(string nm)
    {
        GameObject obj = GameObject.Find(nm);
        if (obj == null) obj = new GameObject(nm);
        return obj;
    }

    static public bool openedSceneCategorie(string part)
    {
        Scene sc = SceneManager.GetActiveScene();
        if (sc.name.Contains(part)) return true;
        return false;
    }
}
