using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.scaffolder
{
    /// <summary>
    /// entered a level / arena
    /// </summary>
    public interface iScaffGameplay
    {
        void gpSetup(); // called when round is setuping
        void gpRestart(); // called when round is ready and booting

        void gpUpdate(); // each frame after restart
        void gpUpdateLate();

        void gpEnd(); // when round ends
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
