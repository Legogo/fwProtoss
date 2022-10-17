using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.scaffolder
{
    /// <summary>
    /// candidate for debug log display
    /// </summary>
    public interface iScaffLog
    {
        string getStamp();
        string stringify();
    }

    public interface iScaffUpdate : iListCandidate
    {
    }

}
