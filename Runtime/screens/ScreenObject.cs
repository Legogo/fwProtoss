﻿using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace fwp.engine.screens
{
    using fwp.engine.scaffolder;

    /// <summary>
    /// show,hide
    /// updateVisible,updateNotVisible
    /// 
    /// visibility is based on activation/deactivation of first child of this component
    /// to use canvases see ScreenUi
    /// </summary>
    public class ScreenObject : ScaffGround, iScaffLog
    {
        public ScreensManager.ScreenType type;
        public ScreensManager.ScreenTags tags;

        [Tooltip("won't be hidden for specific ingame situations")]
        public bool sticky = false; // can't be hidden

        public bool dontHideOtherOnShow = false; // won't close other non sticky screen when showing

        ScreenNav nav;

        protected override void build()
        {
            base.build();

            ScreensManager.subScreen(this);
        }

        public void subNavDirection(Action down, Action up, Action left, Action right)
        {
            if (nav == null) nav = new ScreenNav();

            if (down != null) nav.onPressedDown += down;
            if (up != null) nav.onPressedUp += up;
            if (left != null) nav.onPressedLeft += left;
            if (right != null) nav.onPressedRight += right;
        }

        public void subSkip(Action skip)
        {
            if (nav == null) nav = new ScreenNav();
            nav.onBack += skip;
        }

        virtual public void reset()
        { }

        /// <summary>
        /// update entry point
        /// </summary>
        virtual public void menuUpdate()
        {
            if (isVisible()) updateScreenVisible();
            else updateScreenNotVisible();
        }

        virtual protected void updateScreenNotVisible() { }
        virtual protected void updateScreenVisible()
        {
            nav?.update();
        }

        virtual protected void action_back() { }

        virtual protected void toggleVisible(bool flag)
        {
            transform.GetChild(0).gameObject.SetActive(flag);
        }

        [ContextMenu("show instant")]
        protected void ctxm_show() { showInstant(); }

        [ContextMenu("hide")]
        protected void ctxm_hide() { forceHide(); }

        /// <summary>
        /// when loaded
        /// </summary>
        virtual public void onScreenLoaded()
        {
            showInstant(); // default is opening
        }

        public void show() => showInstant();
        public void hide() => hideInstant();

        /// <summary>
        /// when already loaded but asking to be shown
        /// </summary>
        public void showInstant()
        {
            //Debug.Log(getStamp() + " show " + name);
            nav?.resetTimerNoInteraction();

            transform.position = Vector3.zero;

            toggleVisible(true);

            //Debug.Log(name + " -> show");
        }

        /// <summary>
        /// this is virtual, another screen might do something different
        /// </summary>
        public void hideInstant()
        {
            //Debug.Log("  <color=white>hide()</color> <b>" + name + "</b>");

            if (sticky)
            {
                //Debug.LogWarning("    can't hide " + name + " because is setup as sticky");
                return;
            }

            forceHide();
        }

        /// <summary>
        /// returns true if actually toggled
        /// </summary>
        /// <returns></returns>
        public bool forceHide()
        {
            if (isVisible())
            {
                //dans le cas où y a pas que des canvas
                //ou qu'il y a une seule camera ppale et qu'il faut aligner les choses à 0f
                transform.position = Vector3.down * 3000f;

                toggleVisible(false);

                return true;
            }

            return false;
        }

        public void unload() => unload(false);
        public void unload(bool force = false)
        {
            if (!force && sticky)
            {
                Debug.LogWarning("can't unload sticky scenes : " + gameObject.scene.name);
                return;
            }

            Debug.Log(getStamp()+ " unloading <b>" + gameObject.scene.name + "</b>");

            SceneManager.UnloadSceneAsync(gameObject.scene.name);
        }

        public bool isInteractive() => nav != null;

        virtual public bool isVisible()
        {
            return transform.GetChild(0).gameObject.activeSelf;
        }

        public void act_button(Button clickedButton)
        {
            process_button_press(clickedButton.name);
        }

        virtual protected void process_button_press(string buttonName)
        { }

        virtual public void act_call_home()
        {
            Debug.Log(getStamp() + " calling <b>home screen</b>");

            ScreensManager.open(ScreensManager.ScreenNameGenerics.home);
        }

        public string extractName()
        {
            string[] split = name.Split('_'); // (screen_xxx)
            return split[1].Substring(0, split[1].Length - 1); // remove ')'
        }

        public bool isScreenOfSceneName(string nm)
        {
            return gameObject.scene.name.EndsWith(nm);
        }

        private void OnDestroy()
        {
            if (!Application.isPlaying) return;

            onScreenDestruction();
            ScreensManager.unsubScreen(this);
        }

        virtual protected void onScreenDestruction()
        { }

        override public string stringify()
        {
            return "\n  isVisible ? " + isVisible();
        }

        protected override string solveStampColor() => "white";

    }
}