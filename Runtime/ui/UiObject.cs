using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.ui
{
    using fwp.halpers.visibility;

    public class UiObject : MonoBehaviour
    {

        protected HelperVisibleUi visirUi;

        private void Awake()
        {

            visirUi = (HelperVisibleUi)HelperVisible.createVisibility(this, VisibilityMode.UI);

            created();
        }

        virtual protected void created()
        { }

        private void Start()
        {
            setup();
        }

        virtual protected void setup()
        { }

        private void Update()
        {
            updateUi();
        }

        virtual protected void updateUi()
        { }
    }
}