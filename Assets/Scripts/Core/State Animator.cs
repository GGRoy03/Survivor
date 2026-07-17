using System.Collections.Generic;
using UnityEngine;

namespace Survivor.Core
{
    [RequireComponent(typeof(Animator))]
    public abstract class StateAnimator<TInfo> : MonoBehaviour
    {
        [System.Serializable]
        private struct StateTransition
        {
            public string Name;
            public float BlendDuration;

            [System.NonSerialized] public int Hash;
        }

        [SerializeField] private StateTransition[] m_States;

        private Animator m_Animator;
        private int      m_CurrentState;

        //
        // Unity Hooks
        //

        private void Awake()
        {
            m_Animator = GetComponent<Animator>();

            //
            // TODO:
            // This is not ideal, we would rather not deal with names and
            // the hashing offline... Somehow.
            //

            for (int stateIdx = 0; stateIdx < m_States.Length; ++stateIdx)
            {
                m_States[stateIdx].Hash = Animator.StringToHash(m_States[stateIdx].Name);
            }
        }

        //
        // Animation Hooks
        //

        public void Animate(TInfo info)
        {
            int animationState = GetAnimationState(info);
            if (animationState != m_CurrentState)
            {
                foreach(var state in m_States)
                {
                    if(state.Hash == animationState)
                    {
                        m_Animator.CrossFade(animationState, state.BlendDuration, 0);
                        break;
                    }
                }

                m_CurrentState = animationState;
            }
        }

        //
        // Inheritence Interface
        //

        protected abstract int GetAnimationState(TInfo info);
    }
}