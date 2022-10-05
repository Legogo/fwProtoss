using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.screens
{
    public class UiScreen : ScreenObject
    {
        virtual public void open()
        { }

        virtual public void close()
        { }

        virtual protected void onSkip()
        { }

        public bool isLocking() => isVisible();

        static public Camera getUiCamera()
        {
            // TODO
            return null;
        }
    }
}
