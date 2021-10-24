using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scaffolder
{
    /// <summary>
    /// manager
    /// </summary>
    static class ScaffoldingMgr
    {
        static public bool loading = false;
    }

    /// <summary>
    /// entered a level / arena
    /// </summary>
    public interface iScaffGameplay
    {
        void gpSetup();
        void gpRestart();

        void gpUpdate();
        void gpUpdateLate();

        void gpEnd();
    }

    public interface iScaffMenu
    {
        void menuUpdate();
    }

    public interface iScaffDebug
    {
        string stringify();
    }


    /// <summary>
    /// obscelete~
    /// must be public, not great
    /// </summary>
    public interface ScaffoldingObject
    {
        abstract void build();

        abstract bool isEngineerWorking();

        abstract void setupEarly();
        abstract void setup();
        abstract void setupLate();

        abstract void update();
    }

}
