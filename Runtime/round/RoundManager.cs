using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace fwp.engine.round
{
    /// <summary>
    /// 
    /// </summary>
    public class RoundManager : MonoBehaviour
    {
        static public RoundManager mgr;

        public interface RoundObject
        {
            void roundSetup();
            void roundLaunch();
            void roundUpdate();
            void roundEnd();

            bool roundCanUpdate();
        }

        public enum eRoundStates { RESTART = 0, LIVE = 1, POST_LIVE = 2, END = 3 };
        protected eRoundStates _state = eRoundStates.RESTART;

        RoundObject[] candidates;

        RoundEventsBridge eventsBridge;

        private void Awake()
        {
            mgr = this;

            eventsBridge = new RoundEventsBridge();

            refreshCandidates();
        }

        public void refreshCandidates()
        {
            candidates = fetchCandidates();
        }

        RoundObject[] fetchCandidates()
        {
            return null;
        }

        protected void round_setup()
        {
            eventsBridge.onRoundRestart?.Invoke();

            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i].roundSetup();
            }
        }

        protected void round_launch()
        {
            eventsBridge.onRoundLaunch?.Invoke();

            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i].roundLaunch();
            }
        }

        protected void round_end()
        {
            eventsBridge.onRoundEnd?.Invoke();

            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i].roundEnd();
            }
        }

        private void Update()
        {
            if(_state != eRoundStates.LIVE)
            {
                for (int i = 0; i < candidates.Length; i++)
                {
                    if(candidates[i].roundCanUpdate())
                        candidates[i].roundUpdate();
                }
            }
        }

        public eRoundStates getState() { return _state; }
    }

}
