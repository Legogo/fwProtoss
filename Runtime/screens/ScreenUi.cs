using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.screens
{
    /// <summary>
    /// a screen using canvases
    /// </summary>
    public class ScreenUi : ScreenAnimated
    {

        [Tooltip("a camera somewhere need to be named 'camera-ui' to be used as canvas context")]
        public bool useUiCamera = false;

        protected Canvas[] _canvas;
        protected RectTransform _rt;

        protected override void build()
        {
            base.build();

            _rt = GetComponent<RectTransform>();
        }

        protected override void setupEarly()
        {
            base.setupEarly();

            if (useUiCamera)
            {
                Camera uiCam = qh.gc<Camera>("camera-ui");
                Canvas canvas = getDefaultCanvas();
                if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    canvas.renderMode = RenderMode.ScreenSpaceCamera;
                    canvas.worldCamera = uiCam;
                }
            }

        }

        public Canvas getDefaultCanvas()
        {
            if (_canvas == null)
            {
                _canvas = gameObject.GetComponentsInChildren<Canvas>();
            }

            if (_canvas != null)
            {
                return _canvas[0];
            }

            return null;
        }
        public Canvas getCanvas(string nm)
        {
            for (int i = 0; i < _canvas.Length; i++)
            {
                if (_canvas[i].name.Contains(nm)) return _canvas[i];
            }
            Debug.LogWarning("~ScreenObject~ getCanvas() no canvas named <b>" + nm + "</b>");
            return null;
        }

        public void setCanvasVisibility(string nm, bool flag)
        {
            for (int i = 0; i < _canvas.Length; i++)
            {
                if (_canvas[i].name.Contains(nm))
                {
                    //Debug.Log("  L found canvas '"+nm+"' => visibility to "+flag);
                    _canvas[i].enabled = flag;
                }
            }
        }

        protected override void toggleVisible(bool flag)
        {
            //base.toggleVisible(flag);

            //si le scriptorder fait que le ScreenObject arrive après le Screenmanager ...
            if (_canvas == null) setup();

            if (_canvas == null) Debug.LogError("no canvas ? for " + name, gameObject);

            //Debug.Log("toggle screen " + name + " visibility to " + flag + " | " + _canvas.Length + " canvas");

            //Debug.Log(name + " visibility ? " + flag+" for "+_canvas.Length+" canvas");

            //show all canvas of screen
            for (int i = 0; i < _canvas.Length; i++)
            {
                //Debug.Log(name + "  " + _canvas[i].name);
                if (_canvas[i].enabled != flag)
                {
                    //Debug.Log("  L canvas " + _canvas[i].name + " toggle to " + flag);
                    _canvas[i].enabled = flag;
                }
            }

            //Debug.Log(name+" , "+flag, gameObject);
        }

        public override bool isVisible()
        {
            //base.isVisible();

            for (int i = 0; i < _canvas.Length; i++)
            {
                if (_canvas[i].enabled) return true;
            }
            return false;
        }

        public override string stringify()
        {
            return base.stringify() + "\n  canvas count ? " + _canvas.Length;
        }

        static public Canvas getCanvas(string screenName, string canvasName)
        {
            ScreenUi screen = (ScreenUi)ScreensManager.getScreen(screenName);
            
            Debug.Assert(screen != null, screenName + " is not ui related");

            return screen.getCanvas(canvasName);
        }

    }

}
