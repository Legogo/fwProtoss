using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.screens
{
    abstract public class ScreenMod
    {
        protected ScreenObject owner;

        public ScreenMod(ScreenObject owner)
        {
            this.owner = owner;
        }
    }
}
